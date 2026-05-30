# Architecture

**Project:** My Translation App
**Sprint:** 005 — Azure Provider Integration (updated)
**Date:** 2026-05-28
**Status:** Sprint 005 complete — Azure Translator and Azure Speech-to-Text adapters implemented. Mock and Azure provider modes available. Provider selection is configuration-driven.

---

## Overview

My Translation App is a browser-based web application that allows users to translate typed text or short recorded speech from one language to another. The MVP architecture is a React frontend, a .NET backend API, and one or more external translation/speech providers accessed through a provider abstraction layer.

No user accounts, no permanent data storage, and no real-time continuous listening are included in the MVP.

---

## Current Phase

Sprint 005 has added real Azure provider adapters to the backend. The .NET 8 backend can now run in Mock mode (default) or Azure mode (configured). The provider abstraction architecture defined in Sprint 002 is validated and working.

**Backend runtime:** .NET 8 LTS — SDK 8.0.303. All projects target `net8.0`. Decision D-034.

**Architecture dependency rule (Sprint 005 confirmed):** `Application` has zero `Infrastructure` references. `Infrastructure` implements `Application` interfaces and contains all Azure SDK references. `Api` is the composition root — the only layer that references both `Application` and `Infrastructure`.

**Provider selection:** Controlled by `Translation:Provider` in configuration (`Mock` or `Azure`). Startup validation runs for Azure mode only. Mock mode requires no credentials. See D-039 through D-046.

**Frontend runtime (Sprint 004):** React 18 + TypeScript + Vite 5. Frontend app at `src/frontend/`. All backend calls go through the `services/api/` layer. No secrets in any frontend file.

---

## System Context

```
User (Browser)
    │
    ▼
React Frontend (browser)
    │  HTTPS requests only — no secrets
    ▼
.NET Backend API (server)
    │  Provider SDK calls — secrets stay server-side
    ├─▶ Translation Provider (e.g. Azure Translator)
    └─▶ Speech Provider (e.g. Azure Speech — STT and TTS)
```

The backend is the trust boundary. All external provider API credentials are held server-side and never sent to the browser.

---

## Major Components

### React Frontend

- Renders the translation UI (language selectors, text input, record button, result display).
- Captures push-to-talk audio via browser MediaRecorder API.
- Sends translation and audio requests to the backend API via a frontend service layer.
- Displays transcript, translated text, and error states.
- Handles microphone permission errors gracefully.
- No secrets or API keys in frontend code.

### .NET Backend API

- Exposes a small set of HTTP endpoints (see docs/API.md).
- Validates all incoming requests (size, language codes, content presence, audio MIME type).
- Holds provider credentials securely in environment variables or a secrets manager.
- Routes requests to the appropriate provider via the provider abstraction layer.
- Normalises provider responses into a consistent shape before returning to the frontend.
- Returns structured, safe error responses — no internal stack traces or provider error messages exposed.
- Does not permanently store audio or translated text.
- Does not log raw audio, source text, or translated text. Correlation IDs and timing metrics are permitted in logs.

### Provider Abstraction Layer

The provider abstraction layer lives entirely within the backend Application layer. Provider interfaces define contracts that application services depend on. Concrete provider implementations (Azure adapters) live in the Infrastructure layer and are registered via dependency injection.

Provider interfaces (defined in `Application/Interfaces/`):

```
ISpeechToTextProvider
  TranscribeAsync(audio, sourceLanguage?, cancellationToken) → transcript

ITextTranslationProvider
  TranslateAsync(text, targetLanguage, sourceLanguage?, cancellationToken) → translated text

ITextToSpeechProvider  (optional — see planning/QUESTIONS.md Q-012)
  SynthesizeAsync(text, language, voice?, cancellationToken) → audio data

ILanguageCatalogService
  GetSupportedLanguagesAsync(cancellationToken) → language list
```

Application services depend only on these interfaces, not on any Azure SDK class. Azure adapters implement these interfaces and live in `Infrastructure/Providers/Azure/`.

### External Providers

| Provider | Role | Status |
|---|---|---|
| Azure Translator | Text translation | Implemented — Sprint 005. `Azure.AI.Translation.Text` 1.0.0 (Translator API v3.0). Downgraded from 2.0.0 in Sprint 005.1 — see D-047. |
| Azure Speech | Speech-to-text | Implemented — Sprint 005. `Microsoft.CognitiveServices.Speech` 1.50.0. |
| Azure Speech | Text-to-speech | Implemented — Sprint 007. `AzureTextToSpeechProvider`. MP3 output via `SpeechSynthesisOutputFormat.Audio24Khz96KBitRateMonoMp3`. |
| OpenAI | Alternative translation/transcription | Optional future |
| Google Cloud Translate + Speech | Alternative | Optional future |
| DeepL | Alternative text translation | Optional future |

**Sprint 009A Language Catalog:**

The language catalog was expanded from 10 to 37 languages in Sprint 009A. Three maps are maintained in sync:

- `StaticLanguageCatalogService` — app code → display name (served to frontend via `GET /api/languages`)
- `AzureSpeechToTextProvider.LanguageMap` — app code → Azure Speech BCP-47 locale
- `AzureTextToSpeechProvider.VoiceMap` — app code → Azure Neural voice name
- `AzureTextToSpeechProvider.LocaleMap` — app code → locale for TTS fallback path

Translation support, STT support, and TTS support are not assumed to be identical (D-071). The current catalog contains only languages with confirmed Azure support in all three areas. See `planning/sprints/009A-language-catalog-expansion/blueprint.md` for the full mapping tables.

**Sprint 009B Capability Metadata:**

`LanguageOptionDto` exposes `SupportsTextTranslation`, `SupportsSpeechToText`, and `SupportsTextToSpeech` boolean fields. All 37 current languages default to `true`. The frontend uses `supportsTextToSpeech` to conditionally disable the Play button in `ResultPanel` — `App.tsx` looks up the target language from the loaded catalog and passes the flag as a prop.

Capability badges in the language `<select>` dropdown are deferred (native `<option>` does not support nested HTML; D-077).

**Sprint 007 Azure TTS provider notes:**

- `AzureTextToSpeechProvider` wraps `SpeechSynthesizer` (Microsoft.CognitiveServices.Speech 1.50.0). Uses `SpeechConfig.FromSubscription(Key, Region)` — same credentials as STT SDK path. Output format: `Audio24Khz96KBitRateMonoMp3` → `audio/mpeg`.
- Voice selection: 10-language voice map (Neural voices). Fallback for unmapped codes: `SpeechConfig.SpeechSynthesisLanguage` (Azure default voice). Errors from unsupported languages surface as `ProviderException(PROVIDER_ERROR)`.
- `MockTextToSpeechProvider` returns a 44-byte silent PCM WAV (`audio/wav`) for integration testing without credentials.
- Correlation ID returned as `X-Correlation-ID` response header (binary response body cannot carry JSON).

**Sprint 005 Azure provider notes:**

- `AzureTextTranslationProvider` wraps `TextTranslationClient` (Azure.AI.Translation.Text **1.0.0**, Translator API v3.0). Maps `RequestFailedException` to `ProviderException(PROVIDER_ERROR)`. Empty results map to `ProviderException(TRANSLATION_FAILED)`. Note: SDK 2.0.0 (API 2026-06-06) caused 401 on standard Azure AI Services resources — see D-047.
- `AzureSpeechToTextProvider` wraps `SpeechRecognizer` (Microsoft.CognitiveServices.Speech 1.50.0) for WAV. Compressed audio (MP3, WebM/Opus, OGG/Opus) uses the Azure Speech **Fast Transcription REST API** (`/speechtotext/transcriptions:transcribe?api-version=2025-10-15`) — no GStreamer required. Maps results to typed `ProviderException` error codes. Sprint 005.2.
- Language code mapping: 2-letter codes (e.g. "en") are mapped to BCP-47 full tags (e.g. "en-US") for Azure Speech. Auto-detect defaults to "en-US".
- Supported audio formats (Azure Speech): WAV (SDK path), MP3 / WebM/Opus / OGG/Opus (Fast Transcription REST path), MP4 (not supported — returns UNSUPPORTED_AUDIO_FORMAT). See D-049.
- Config: `AzureSpeech:Key`, `AzureSpeech:Region` (WAV SDK path), `AzureSpeech:Endpoint` (Fast Transcription REST path — base URL of the Azure AI Services resource). All three required at startup in Azure mode.

---

## Frontend Project Structure

```text
frontend/
  src/
    app/                        # App entry point, routing (if needed), global providers
    components/
      translation/              # Text input, translate button, result display
      audio/                    # Push-to-talk control, recording state indicator
      layout/                   # Page shell, header, responsive layout
      feedback/                 # Error messages, loading states, empty states
    hooks/                      # Custom React hooks (useTranslation, useAudioRecorder, etc.)
    services/                   # API service layer — all backend HTTP calls isolated here
    types/                      # TypeScript type definitions and API contract types
    utils/                      # Pure helper functions (formatting, language code utilities)
    validation/                 # Frontend validation helpers (UX guard only — not a security boundary)
```

Rules:
- No secrets or credentials in any frontend file.
- `services/` is the only layer that calls the backend API. Components do not call the backend directly.
- `validation/` helpers are for UX guard only. Backend is the authoritative validation boundary.

---

## Frontend Shell (Sprint 004)

**Actual structure as built:**

```text
src/frontend/
├── src/
│   ├── api/
│   │   └── translationApi.ts     # All backend HTTP calls — fetchLanguages, translateText, translateAudio
│   ├── components/
│   │   ├── LanguageSelect.tsx    # Reusable language selector with optional auto-detect
│   │   ├── TextTranslationForm.tsx  # Text form with validation and loading state
│   │   ├── AudioTranslationForm.tsx # File upload form with MIME advisory and loading state
│   │   ├── ResultPanel.tsx       # Displays text or audio translation result
│   │   └── ErrorPanel.tsx        # Displays structured API error (errorCode, details, correlationId)
│   ├── types/
│   │   └── api.ts                # TypeScript interfaces matching backend DTOs exactly
│   ├── styles/
│   │   └── app.css               # Plain CSS — no external framework
│   ├── App.tsx                   # Language loading on mount, result/error state routing
│   └── main.tsx                  # React root mount
├── vite.config.ts                # Vite dev proxy: /api/* → http://localhost:5074
├── tsconfig.json / tsconfig.app.json / tsconfig.node.json
├── .env.example                  # Documents VITE_API_BASE_URL (no secrets)
└── package.json
```

**Frontend TypeScript types** in `api.ts` match the backend DTOs field-for-field:

| TS Interface | Backend DTO | Key Fields |
|---|---|---|
| `LanguageOption` | `LanguageOptionDto` | `code`, `name` |
| `LanguageListResponse` | `LanguageListResponseDto` | `languages[]`, `correlationId` |
| `TextTranslationResponse` | `TextTranslationResponseDto` | `translatedText`, `sourceLanguage`, `targetLanguage`, `provider`, `correlationId` |
| `AudioTranslationResponse` | `AudioTranslationResponseDto` | `transcribedText`, `translatedText`, `sourceLanguage`, `targetLanguage`, `provider`, `correlationId` |
| `ApiErrorResponse` | `ApiErrorResponseDto` | `errorCode`, `message`, `details[]`, `correlationId` |
| `ApiErrorDetail` | `ApiErrorDetailDto` | `field`, `message` |

Note: `errorCode` (not `code`) — the backend `ErrorCode` C# property serialises to `errorCode` in JSON. See D-038.

**Local development setup:**

The Vite dev server proxies `/api/*` to `http://localhost:5074`. The browser sees only `localhost:5173` — no cross-origin request is made. Minimal CORS is also registered in `Program.cs` for `localhost:5173` as belt-and-suspenders (D-036). Neither is a production CORS policy — production hosting must be designed separately (Q-018, Q-014).

**Audio upload in Sprint 004:**

Audio translation uses a standard `<input type="file" accept="audio/*">` form upload. Browser microphone recording (MediaRecorder / push-to-talk) is deferred to Sprint 006 (D-037).

---

## Backend Project Structure

```text
backend/
  src/
    Api/
      Controllers/              # HTTP endpoint handlers — map DTOs, call application services
      Middleware/               # Error handling, correlation ID injection, request lifecycle logging
      Contracts/                # Request and response DTOs only
    Application/
      Translation/              # Text translation application service
      Speech/                   # Audio transcription application service
      TextToSpeech/             # TTS application service (optional)
      Validation/               # Application-layer validation logic
      Interfaces/               # Provider abstraction interfaces (ISpeechToTextProvider, etc.)
    Infrastructure/
      Providers/
        Azure/                  # Azure Translator and Azure Speech adapter implementations
      Configuration/            # Config binding (IOptions patterns, secret loading)
      Logging/                  # Log formatting, sensitive field exclusion
    Domain/
      Models/                   # Internal domain models (not provider SDK objects)
      Errors/                   # Domain error types and error codes
    Shared/                     # Cross-cutting utilities shared across layers
```

Rules:
- Provider abstraction interfaces live in `Application/Interfaces/`, not `Infrastructure/`.
- Azure SDK classes are confined to `Infrastructure/Providers/Azure/`.
- Raw provider SDK objects are never returned to the Application or API layers.
- Configuration and secrets are bound in `Infrastructure/Configuration/`.
- Controllers call Application services only. Controllers do not import Infrastructure directly.

---

## Service Boundaries

| Layer | Responsibility | May Depend On |
|---|---|---|
| Api / Controllers | Receive HTTP requests; validate DTO structure; call application services; return response DTOs | Application layer |
| Api / Middleware | Inject correlation IDs; format safe error responses; log request lifecycle metadata | None |
| Application / Services | Orchestrate translation and speech workflows; call provider abstractions; map results to DTOs | Application interfaces, Domain models |
| Application / Interfaces | Define provider contracts; owned by Application layer | None |
| Application / Validation | Enforce authoritative business validation rules | Domain models |
| Infrastructure / Adapters | Implement provider interfaces using Azure SDKs; map provider responses to Domain models | Application interfaces |
| Infrastructure / Configuration | Bind environment variables and secrets to typed options classes | None |
| Domain / Models | Represent internal translation and speech results; provider-agnostic | None |
| Domain / Errors | Define structured error types; map to API error codes | None |

Controllers do not call Infrastructure directly. Application services do not import Azure SDK types.

---

## Configuration and Environment Strategy

- All provider credentials (Azure keys, regions, etc.) are stored in server-side environment variables only.
- During local development, secrets are managed with .NET User Secrets (`dotnet user-secrets`) or an `.env` file excluded from version control by `.gitignore`.
- No secrets appear in any committed file. An `.env.example` or similar template must contain only placeholder values with no real credentials.
- Frontend receives no provider credentials under any circumstances.
- Configuration is bound to strongly-typed .NET options classes at startup via `IOptions<T>`.
- Secret management for production is TBD pending deployment target decision (see planning/QUESTIONS.md Q-014).

---

## Security Boundaries

- Provider API keys and credentials are never sent to the browser or stored in frontend code.
- All credentials are held in server-side environment variables or a secrets manager.
- User audio is not permanently stored.
- Raw audio, source text, and translated text are not logged by default.
- Correlation IDs and timing metrics are safe to log.
- Backend returns structured, safe errors — no internal details, stack traces, or provider messages exposed.
- Request validation occurs at both frontend (UX guard) and backend (authoritative security boundary).
- Requests that fail backend validation never reach the external provider.

---

## Data Flow

### Text Translation Path

```
1. User types text and selects source + target languages.
2. Frontend validates input (non-empty, within length limit, language codes present).
3. Frontend service layer sends POST /api/translate/text to backend.
4. Backend Middleware injects correlation ID into request context.
5. Backend validates request (schema, language codes, content presence, size).
6. Backend Translation application service calls ITextTranslationProvider.
7. Azure Translator adapter performs provider request.
8. Provider response is mapped to internal Domain model in adapter.
9. Application service maps Domain model to TranslateTextResponse DTO.
10. Backend returns TranslateTextResponse to frontend.
11. Frontend displays translated text.
12. User optionally requests TTS playback via POST /api/speech/synthesize.
```

### Audio Translation Path

```
1. User selects source + target languages and presses push-to-talk.
2. Frontend requests microphone permission via MediaRecorder API.
3. If permission is denied, frontend shows a clear error state. No request is sent.
4. Browser records audio clip. Recording stops when user releases button or max duration is reached.
5. Frontend validates audio blob presence and approximate size.
6. Frontend service layer sends POST /api/translate/audio (multipart/form-data) to backend.
7. Backend Middleware injects correlation ID.
8. Backend validates request (MIME type, size, duration, language codes).
9. Backend Speech application service calls ISpeechToTextProvider.
10. Azure Speech adapter transcribes audio — returns transcript.
11. Speech service passes transcript to ITextTranslationProvider.
12. Azure Translator adapter translates transcript.
13. Application service maps results to TranslateAudioResponse DTO.
14. Backend returns transcript + translated text to frontend.
15. Frontend displays original transcript and translated text.
16. User optionally requests TTS playback.
```

### Optional TTS Playback Path

```
1. User requests playback of translated text.
2. Frontend service layer sends POST /api/speech/synthesize to backend.
3. Backend validates text and language code.
4. Backend calls ITextToSpeechProvider.
5. Azure Speech TTS adapter returns audio data.
6. Adapter maps audio result to Domain model.
7. Backend returns TextToSpeechResponse DTO to frontend.
8. Frontend plays audio.
```

---

## Frontend State Management

Frontend state is divided into three tiers:

| Tier | Location | What It Holds |
|---|---|---|
| Local UI state | Component `useState` | Input field values, recording button active state, loading indicators, inline error messages |
| Translation session state | React Context | Current translation result, current transcript, selected languages, session history (if displayed) |
| API access | `services/` layer | All HTTP calls to the backend; returns data; holds no state itself |

Rules:
- Components do not call the backend API directly. All API calls go through `services/`.
- Session state is held in React Context. It does not persist beyond the current browser session.
- Redux, Zustand, and other global state libraries are deferred unless state complexity requires them later.
- Validation helpers in `validation/` are pure functions with no side effects and no API access.

---

## Audio Processing Flow

```
1. User presses and holds the push-to-talk button.
2. Frontend requests microphone permission via navigator.mediaDevices / MediaRecorder API.
3. If permission is denied, frontend shows a clear error state (see docs/VALIDATION.md).
4. If granted, MediaRecorder begins capturing audio.
5. When user releases the button (or max duration is reached), recording stops.
6. Frontend reads the recorded Blob from MediaRecorder output.
7. Frontend validates blob presence and approximate size before sending.
8. Frontend sends audio blob + language fields as multipart/form-data to POST /api/translate/audio.
9. Backend validates MIME type, file size, and estimated duration before processing.
10. Backend calls ISpeechToTextProvider — receives transcript string.
11. Backend calls ITextTranslationProvider with transcript — receives translated text.
12. Backend returns TranslateAudioResponse containing both transcript and translated text.
```

MIME type compatibility:
- MediaRecorder output MIME type varies across browsers and operating systems (e.g. `audio/webm`, `audio/ogg`, `audio/mp4`).
- The set of accepted MIME types must be defined before Sprint 003 implementation (see planning/QUESTIONS.md Q-011).
- Backend must validate MIME type explicitly and reject unsupported formats with UNSUPPORTED_AUDIO_FORMAT error.
- See also planning/RISKS.md R-011.

---

## Future PostgreSQL Integration Strategy

PostgreSQL is deferred until user accounts, persistent history, preferences, or audit features are in scope (see planning/DECISIONS.md D-012).

When introduced:
- A new `Infrastructure/Persistence/` folder is created within the backend structure.
- Repository interfaces are defined in the Application layer, consistent with the provider abstraction pattern.
- Domain models are mapped to persistence entities in Infrastructure only.
- No Application or Domain layer code changes are required to add persistence.
- Session-only history currently held in React Context becomes persistent history only when explicitly scoped by the project owner.

---

## Implementation Sequencing

| Sprint | Focus | Notes |
|---|---|---|
| 001 | Discovery Architecture | Complete |
| 002 | Implementation Architecture | Complete |
| 003 | Backend API Skeleton | Complete — .NET 8 skeleton, DTOs, provider interfaces, mock adapters, validation, 30 tests passing. |
| 004 | Frontend MVP Shell | Complete — React 18 + TypeScript + Vite, language loading, text form, audio upload form, result/error panels, loading states, client-side validation, backend error display. |
| 005 | Azure Provider Integration | Complete — Azure Translator and Azure Speech-to-Text adapters, config-driven provider selection, startup validation, 45 tests passing. |
| 005.2 | Audio Format Compatibility Fix | Complete — Compressed audio (MP3, WebM, OGG) routed to Fast Transcription REST API. GStreamer eliminated. AzureSpeech:Endpoint added. audio/ogg validation support added. 61 tests passing. |
| 006 | Audio and Push-to-Talk | Full audio recording, upload, transcription, and translation flow end to end. |
| 007 | TTS Playback | Complete — AzureTextToSpeechProvider, POST /api/translate/tts, ResultPanel Play button. 69 tests passing. |
| 008 | UX Modernization & Portfolio | Complete — dark navy theme, CSS custom properties, responsive layout, portfolio docs. 71 tests passing. |
| 009A | Language Catalog Expansion | Complete — catalog expanded from 10 to 37 languages, STT/TTS maps updated, 44 new tests. 115 tests passing. |
| 009B | Voice & Capability Indicators | Recommended next sprint. |

Sprint numbers beyond 003 are indicative. Final scope for each sprint is defined by a future Architect Pack. Do not begin Sprint 003 until Sprint 002 is accepted.

---

## Operational Notes

- Mobile-friendly browser layout is the design priority.
- Session history (current session only) may be displayed in the UI — not persisted.
- PostgreSQL and persistent storage are deferred until accounts or history features are added.
- Hosting platform is TBD — see planning/QUESTIONS.md Q-014.
- Continuous real-time listening and WebSocket streaming are explicitly out of scope for MVP.

---

## Non-Goals

The following are explicitly not part of the MVP architecture:

- WebSocket or real-time streaming translation.
- User accounts, registration, or authentication.
- Persistent storage of audio, transcripts, or translations.
- Database schema or migrations.
- Native mobile applications.
- Offline translation models.
- Browser extensions.
- Multi-tenant or enterprise administration features.
- Billing or subscription management.
- CI/CD pipeline definition.
- Infrastructure deployment automation.
- A health check endpoint (/api/health is not included in the MVP API).
