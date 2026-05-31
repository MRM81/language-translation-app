# Case Study — My Translation App

## 1. Project Overview

My Translation App is a browser-based translation tool that lets users translate typed text or short spoken recordings between 37 languages. The project was built sprint-by-sprint across 19 sprints, from architecture through to a polished, portfolio-ready v1.0 release deployed on AWS.

**Core capabilities:**
- Text translation via typed input
- Audio translation via push-to-talk recording (Chrome, Edge, Firefox)
- Audio translation via file upload (fallback)
- Text-to-speech playback of translated output
- Conversation Mode — turn-based bilingual conversation with auto-play
- Push-To-Talk in Conversation Mode
- Multi-conversation management with naming, search, and switching
- Conversation persistence (localStorage)
- Export — TXT, JSON, clipboard
- 37 supported languages
- Landing page with product branding
- Live production deployment (AWS S3 + CloudFront + Elastic Beanstalk)

**Target context:** Real-world in-person communication — a traveller or professional needing quick, spoken translation without installing a native app.

---

## 2. Technical Stack

| Layer | Technology |
|---|---|
| Frontend | React 18 + TypeScript + Vite 5 |
| Styling | Plain CSS with custom properties (no UI framework) |
| Backend | .NET 8 LTS (ASP.NET Core) |
| Text translation | Azure Translator (SDK 1.0.0, API v3.0) |
| Speech-to-text | Azure Speech SDK 1.50.0 (WAV) + Fast Transcription REST API (MP3/WebM/OGG) |
| Text-to-speech | Azure Speech SDK 1.50.0 (`Audio24Khz96KBitRateMonoMp3`) |
| Hosting — Frontend | AWS S3 + CloudFront (HTTPS, global CDN) |
| Hosting — Backend | AWS Elastic Beanstalk (SingleInstance, .NET 8 on AL2023) |
| Testing | xUnit (.NET) — 133 tests |
| Build | Vite (`npm run build`), `dotnet build` |

---

## 3. Architecture Highlights

### Provider Abstraction Layer

The backend is built around a provider abstraction layer. Three interfaces live in the Application layer:

```
ISpeechToTextProvider    — transcribes audio to text
ITextTranslationProvider — translates text between languages
ITextToSpeechProvider    — synthesizes speech from text
```

Azure adapters implement these interfaces in the Infrastructure layer. The Application layer has zero Azure SDK references. A future provider swap (OpenAI, DeepL, Google) requires only Infrastructure changes — no Application or API changes.

### Configuration-Driven Provider Switching

Provider selection is controlled by a single config key: `Translation:Provider = "Mock" | "Azure"`. Mock mode requires no credentials. Azure mode is opted into explicitly via User Secrets or environment variables.

### AWS Deployment Pattern — Same-Origin Proxy

Browser calls go to CloudFront (HTTPS) at a single domain. CloudFront routes `/api/*` to Elastic Beanstalk and `/*` to S3. This eliminates mixed-content errors (EB is HTTP-only), avoids cross-origin CORS on browser fetch calls, and keeps the EB URL private.

```
Browser  →  CloudFront (HTTPS)
               ├─ /api/*   →  Elastic Beanstalk (HTTP, private)
               └─ /*       →  S3 (static assets)
```

### Conversation Mode Architecture

Conversation Mode is a pure frontend orchestration layer — no new backend endpoints. It reuses all three existing API calls (`translateText`, `translateAudio`, `synthesizeSpeech`) to create a turn-based bilingual conversation experience. Conversations persist in `localStorage` and survive page refresh.

### Frontend Navigation Model

Sprint 018 introduced a three-state navigation model without a routing library:

```typescript
screen: 'landing' | 'workspace'
workspaceMode: 'translate' | 'conversation'
translationInputMode: 'text' | 'audio'
```

The landing page is the default entry point. The workspace preserves all existing mode navigation.

### Security Posture

- All provider credentials are server-side only. The frontend holds no API keys.
- User audio, source text, and translated text are never logged.
- Correlation IDs and timing metrics are safe to log.
- Structured error envelope — no stack traces or provider messages exposed to the browser.

---

## 4. Key Technical Challenges

### GStreamer Dependency Eliminated (Sprint 005.2)

The Azure Speech SDK's compressed-audio push-stream path requires GStreamer on Windows. The solution was to route all compressed audio (MP3, WebM, OGG) through the Azure Speech **Fast Transcription REST API** (`api-version=2025-10-15`), which handles codec decoding server-side. WAV remained on the SDK path. No native codec dependencies remain.

### Azure SDK Version Conflict (Sprint 005.1)

`Azure.AI.Translation.Text` 2.0.0 targets API version `2026-06-06`, not available on standard Azure AI Services resources. Pinned to SDK 1.0.0 (Translator API v3.0).

### Binary TTS Response — Correlation ID via Header

The TTS endpoint returns raw binary audio. The correlation ID is returned in the `X-Correlation-ID` response header — consistent with the existing middleware convention.

### Content-Type Guard on Fetch Error Paths

A post-Sprint-007 defect: `synthesizeSpeech` called `res.json()` unconditionally in the error path. When the error body was non-JSON (proxy error, empty body), a raw JS engine `SyntaxError` leaked to the UI. Fix: check `Content-Type` before calling `.json()` in error paths.

### Conversation State Management (Sprints 013–017)

Conversation Mode introduced multi-level state: active speaker, message history, language pair, auto-play setting, conversation identity, and search filter. All state is managed in `ConversationMode.tsx` with no new backend surface. The storage migration path (single conversation → multi-conversation store) was designed to be non-destructive — legacy data is migrated once and removed only after the new store saves successfully.

### Cross-Cloud Deployment (Sprint 012)

Running Azure AI services through an AWS-hosted backend required careful configuration of CloudFront origin headers. Azure credentials are held exclusively in EB environment properties. The browser never calls Azure directly.

---

## 5. UX Decisions

| Decision | Rationale |
|---|---|
| Push-to-talk (press to start, press to stop) | Lower complexity than hold-to-record. Aligns with the MVP domain model. |
| On-demand TTS playback | Avoids generating speech on every translation. Gives users control. |
| Runtime MIME detection | `MediaRecorder.isTypeSupported()` selects the best format per browser at runtime. |
| Conversation Mode as frontend orchestration | Reuses all three existing API endpoints. Zero new backend surface. |
| Auto-play on in Conversation Mode by default | Core UX expectation for conversational translation. Toggle available. |
| localStorage persistence | Zero-account, zero-database architecture maintained through v1. |
| Landing page as first screen | Portfolio-quality first impression. Workspace is not the right entry point for a polished product. |
| Text / Audio segmented toggle | Showing both forms simultaneously creates clutter. One visible at a time is cleaner. |

---

## 6. Sprint Journey

| Sprint | Delivered |
|---|---|
| 001 | Discovery Architecture — MVP scope, domain model, decisions, risks, questions |
| 002 | Implementation Architecture — folder structures, provider interfaces, API boundaries |
| 003 | Backend API Skeleton — .NET 8, DTOs, mock providers, validation, 30 tests |
| 004 | Frontend MVP Shell — React + TypeScript + Vite, language loading, text/audio forms |
| 005 | Azure Provider Integration — Azure Translator + Speech-to-Text, config-driven switching |
| 005.2 | Audio Format Compatibility Fix — Fast Transcription REST API, GStreamer eliminated |
| 006 | Push-to-Talk — MediaRecorder, three-state button, recording indicator and timer |
| 007 | Text-to-Speech Playback — Azure TTS provider, four-state Play button |
| 008 | UX Modernization — dark theme, responsive design, accessibility |
| 009A | Language Catalog Expansion — 10 → 37 languages |
| 009B | Language Capability Metadata — per-language TTS support flags |
| 010 | Deployment Readiness Hardening — health endpoint, config-driven CORS |
| 011 | AWS Deployment Preparation — `VITE_API_BASE_URL`, deployment docs |
| 012 | AWS Production Deployment — S3 + CloudFront + Elastic Beanstalk, live |
| 013 | Conversation Mode — turn-based bilingual conversation, auto-play |
| 014 | Push-To-Talk in Conversation Mode |
| 015 | Conversation Persistence & Export — localStorage, TXT/JSON/clipboard |
| 016 | Multi-Conversation Management — named conversations, rename, delete, switch |
| 017 | Conversation Search & Demo Polish — title + full message text search, auto-title |
| 018 | UI/UX Redesign — landing page, logo, favicon, translation toggle, workspace header |
| 019 | Production Refresh & Portfolio Assets — v1.0 release |

---

## 7. Results

- **133 automated tests** — all passing at v1.0
- **37 languages** supported
- **Live production deployment** on AWS (CloudFront + Elastic Beanstalk + S3)
- **Zero third-party UI libraries** — all styling in plain CSS
- **Zero authentication** required — zero friction entry point
- **Zero database** — localStorage covers v1 persistence requirements

---

## 8. Lessons Learned

See `portfolio/lessons-learned.md` for the full sprint-by-sprint retrospective.

---

## 9. Post-v1 Roadmap

Potential future enhancements:

| Feature | Notes |
|---|---|
| Custom domain | Route 53 + ACM certificate |
| Route-based navigation | Deep-linking, back-button behaviour |
| Real-time speech translation | Continuous listening — architectural uplift required |
| Cloud sync | Requires authentication and backend persistence |
| User accounts | Authentication layer |
| CI/CD pipeline | GitHub Actions or AWS CodePipeline |
| Team conversations | Shared conversation sessions |
