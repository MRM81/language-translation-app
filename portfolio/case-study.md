# Case Study — My Translation App

## 1. Project Overview

My Translation App is a browser-based translation tool that lets users translate typed text or short spoken recordings between languages. The project was built sprint-by-sprint from architecture through to a polished, portfolio-ready MVP.

**Core capabilities:**
- Text translation via typed input
- Audio translation via push-to-talk recording (Chrome, Edge, Firefox)
- Audio translation via file upload (fallback)
- Text-to-speech playback of translated output
- Ten supported languages at launch

**Target context:** Real-world in-person communication — a traveller or professional needing quick, spoken translation without installing a native app.

---

## 2. Technical Stack

| Layer | Technology |
|---|---|
| Frontend | React 18 + TypeScript + Vite 5 |
| Styling | Plain CSS with custom properties (no UI framework) |
| Backend | .NET 8 LTS (ASP.NET Core) |
| Text translation | Azure Translator (SDK 1.0.0, API v3.0) |
| Speech-to-text | Azure Speech SDK 1.50.0 (WAV path) + Fast Transcription REST API (MP3/WebM/OGG) |
| Text-to-speech | Azure Speech SDK 1.50.0 (`Audio24Khz96KBitRateMonoMp3`) |
| Testing | xUnit (.NET) — 71 tests |
| Build | Vite (`npm run build`), `dotnet build` |

---

## 3. Architecture Highlights

### Provider Abstraction Layer

The backend is built around a provider abstraction layer. Three interfaces live in the Application layer:

```
ISpeechToTextProvider   — transcribes audio to text
ITextTranslationProvider — translates text between languages
ITextToSpeechProvider   — synthesizes speech from text
```

Azure adapters implement these interfaces in the Infrastructure layer. The Application layer has zero Azure SDK references. This means a future provider swap (OpenAI, DeepL, Google) requires only Infrastructure changes — no Application or API changes.

### Configuration-Driven Provider Switching

Provider selection is controlled by a single config key: `Translation:Provider = "Mock" | "Azure"`. Mock mode requires no credentials and is the safe default. Azure mode is opted into explicitly via User Secrets or environment variables.

### Security Posture

- All provider credentials are server-side only. The frontend holds no API keys.
- User audio, source text, and translated text are never logged.
- Correlation IDs and timing metrics are safe to log.
- Structured error envelope — no stack traces or provider messages exposed to the browser.

### Frontend Architecture

- React Context for session state
- `services/api/` layer isolates all backend HTTP calls — components never call the backend directly
- TypeScript interfaces in `types/api.ts` mirror backend DTOs field-for-field
- Vite dev proxy eliminates CORS in development

---

## 4. Key Technical Challenges

### GStreamer Dependency Eliminated (Sprint 005.2)

The Azure Speech SDK's compressed-audio push-stream path requires GStreamer on Windows — unavailable on standard dev machines. The solution was to route all compressed audio (MP3, WebM, OGG) through the Azure Speech **Fast Transcription REST API** (`/speechtotext/transcriptions:transcribe?api-version=2025-10-15`) which handles codec decoding server-side. WAV remained on the SDK path. No native codec dependencies remain.

### Azure SDK Version Conflict (Sprint 005.1)

`Azure.AI.Translation.Text` 2.0.0 targets API version `2026-06-06`, which is not exposed by standard Azure AI Services multi-service resources. These resources only expose Translator at the v3.0 path. The fix was to pin to SDK 1.0.0 (Translator API v3.0), which is compatible with all standard Azure resources.

### Binary TTS Response — Correlation ID via Header

The TTS endpoint returns raw binary audio in its response body. A JSON correlation ID cannot be embedded in a binary response body. The solution was to return the correlation ID in the `X-Correlation-ID` response header — consistent with the existing middleware convention for inbound correlation IDs.

### Content-Type Guard on Fetch Error Paths

A post-Sprint-007 defect: the `synthesizeSpeech` fetch helper called `res.json()` unconditionally in the error path. When the error response body was non-JSON (proxy error, empty body), `res.json()` threw a browser `SyntaxError` that leaked through to the UI as a raw JS engine error string. The fix: check `Content-Type` header before calling `res.json()` in the error path. The `ResultPanel` catch block was hardened to discriminate on `apiErr?.errorCode` before writing any message to the UI.

---

## 5. UX Decisions

| Decision | Rationale |
|---|---|
| Push-to-talk (press to start, press to stop) | Lower complexity than hold-to-record. No continuous listening. Aligns with the MVP domain model. |
| On-demand TTS playback | Avoids generating speech audio on every translation (cost and latency). Gives users control. |
| Runtime MIME detection | `MediaRecorder.isTypeSupported()` selects the best format per browser at runtime. No hardcoded format. |
| File upload as fallback tab | Useful for testing with known audio files and for users with pre-recorded content. Adds negligible scope. |
| Structured error display | Correlation ID shown in error panel lets users report specific failures for support. |

---

## 6. Sprint Journey

| Sprint | Delivered |
|---|---|
| 001 | Discovery Architecture — MVP scope, domain model, decisions, risks, questions |
| 002 | Implementation Architecture — folder structures, provider interfaces, API boundaries, sequencing |
| 003 | Backend API Skeleton — .NET 8, DTOs, mock providers, validation, 30 tests |
| 004 | Frontend MVP Shell — React + TypeScript + Vite, language loading, text/audio forms, result/error panels |
| 005 | Azure Provider Integration — Azure Translator + Speech-to-Text adapters, config-driven switching, 45 tests |
| 005.2 | Audio Format Compatibility Fix — Fast Transcription REST API, GStreamer eliminated, 61 tests |
| 006 | Push-to-Talk — MediaRecorder, push-to-talk UX, three-state button, recording indicator and timer, 61 tests |
| 007 | Text-to-Speech Playback — Azure TTS provider, Play button, four-state UI, object URL cleanup, 71 tests |
| 008 | UX Modernization — dark theme, responsive design, accessibility, portfolio documentation |

---

## 7. Lessons Learned

See `portfolio/lessons-learned.md` for the full sprint-by-sprint retrospective.

---

## 8. Future Roadmap

Candidates for Sprint 009+:

| Feature | Rationale |
|---|---|
| Deployment (Azure App Service or Vercel + Railway) | Q-006, Q-014 — hosting target not yet decided |
| Session translation history | Q-005, Q-019 — current session display, no persistence |
| Auto language detection | Q-008, Q-026 — source language auto-detect via Azure |
| Dark mode toggle | Q-032 — user-selectable, Sprint 008 establishes the CSS variable foundation |
| Audio duration enforcement | R-016, Q-015 — exact 60s enforcement via NAudio |
| Rate limiting | R-004, R-014 — per-IP request limits to control Azure costs |
| CI/CD pipeline | Q-024 — automated tests + deployment on merge |
