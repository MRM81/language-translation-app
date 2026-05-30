# Architecture Overview — My Translation App

## System Diagram

```
┌─────────────────────────────────────────────────────────────┐
│  Browser (React 18 + TypeScript + Vite)                      │
│                                                              │
│  App.tsx                                                     │
│    ├── TextTranslationForm  ── POST /api/translate/text ──┐  │
│    ├── AudioTranslationForm ── POST /api/translate/audio ─┤  │
│    ├── ResultPanel          ── POST /api/translate/tts ───┤  │
│    └── ErrorPanel                                         │  │
│                                                           │  │
│  api/translationApi.ts  ──────────────────────────────────┘  │
│  (fetch wrapper — all HTTP calls isolated here)              │
└──────────────────────────┬──────────────────────────────────┘
                           │  HTTPS — no secrets in browser
                           ▼
┌─────────────────────────────────────────────────────────────┐
│  .NET 8 Backend (ASP.NET Core)                              │
│                                                              │
│  Api Layer                                                   │
│    TranslationController                                     │
│      POST /api/translate/text                               │
│      POST /api/translate/audio                              │
│      POST /api/translate/tts                                │
│      GET  /api/languages                                    │
│    CorrelationIdMiddleware (X-Correlation-ID)                │
│    ExceptionHandlingMiddleware (safe error envelope)         │
│                                                              │
│  Application Layer                                           │
│    TranslationService          SpeechService                 │
│    TextToSpeechService         TranslationRequestValidator   │
│    ITextTranslationProvider    ISpeechToTextProvider         │
│    ITextToSpeechProvider       ILanguageCatalogService       │
│                                                              │
│  Infrastructure Layer                                        │
│    AzureTextTranslationProvider  (Azure.AI.Translation.Text) │
│    AzureSpeechToTextProvider     (Speech SDK + REST API)     │
│    AzureTextToSpeechProvider     (Speech SDK TTS)            │
│    MockTextTranslationProvider   (dev/test)                  │
│    MockSpeechToTextProvider      (dev/test)                  │
│    MockTextToSpeechProvider      (dev/test)                  │
└────────┬──────────────────────────┬─────────────────────────┘
         │                          │
         ▼                          ▼
┌─────────────────┐     ┌──────────────────────────────────┐
│ Azure Translator│     │ Azure Speech Services             │
│ API v3.0        │     │  ├── Speech-to-Text (SDK + REST)  │
│ (text)          │     │  └── Text-to-Speech (SDK, MP3)    │
└─────────────────┘     └──────────────────────────────────┘
```

---

## Frontend

**Framework:** React 18 + TypeScript + Vite 5

**Folder structure:**
```
src/frontend/src/
├── api/
│   └── translationApi.ts      — all HTTP calls (fetch wrappers)
├── components/
│   ├── TextTranslationForm.tsx
│   ├── AudioTranslationForm.tsx
│   ├── LanguageSelect.tsx
│   ├── PushToTalkButton.tsx
│   ├── RecordingIndicator.tsx
│   ├── RecordingTimer.tsx
│   ├── ResultPanel.tsx
│   └── ErrorPanel.tsx
├── services/
│   └── AudioCaptureService.ts — MediaRecorder wrapper
├── types/
│   └── api.ts                 — TypeScript interfaces (mirrors backend DTOs)
└── styles/
    └── app.css                — all styles (no UI framework)
```

**Key design rules:**
- No secrets or credentials in any frontend file
- `api/translationApi.ts` is the only file that calls the backend
- TypeScript interfaces in `types/api.ts` match backend DTOs field-for-field
- Vite dev proxy (`/api/*` → `http://localhost:5074`) eliminates CORS in development

---

## Backend

**Framework:** .NET 8 LTS (ASP.NET Core)

**Project structure:**
```
src/backend/
├── Api/                   — Controllers, Middleware, DTOs
├── Application/           — Services, Interfaces, Validation (no Azure refs)
├── Infrastructure/        — Azure adapters, config binding
├── Domain/                — Models, error codes
└── Tests/                 — 71 xUnit tests
```

**Architecture dependency rule:** `Application` has zero `Infrastructure` references. `Infrastructure` implements `Application` interfaces. `Api` is the composition root.

---

## Azure Services

| Service | Role | SDK / API |
|---|---|---|
| Azure Translator | Text translation | `Azure.AI.Translation.Text` 1.0.0 (API v3.0) |
| Azure Speech — STT | Audio transcription (WAV) | `Microsoft.CognitiveServices.Speech` 1.50.0 |
| Azure Speech — STT | Audio transcription (MP3/WebM/OGG) | Fast Transcription REST API `2025-10-15` |
| Azure Speech — TTS | Speech synthesis (MP3 output) | `Microsoft.CognitiveServices.Speech` 1.50.0 |

**Configuration:**
- `AzureTranslator:Key`, `AzureTranslator:Region`
- `AzureSpeech:Key`, `AzureSpeech:Region`, `AzureSpeech:Endpoint`
- All via .NET User Secrets locally; environment variables in production

---

## Provider Abstraction Pattern

```
Application/Interfaces/
    ITextTranslationProvider
    ISpeechToTextProvider
    ITextToSpeechProvider
    ILanguageCatalogService

Infrastructure/Providers/Azure/
    AzureTextTranslationProvider  : ITextTranslationProvider
    AzureSpeechToTextProvider     : ISpeechToTextProvider
    AzureTextToSpeechProvider     : ITextToSpeechProvider

Infrastructure/Providers/Mock/
    MockTextTranslationProvider   : ITextTranslationProvider
    MockSpeechToTextProvider      : ISpeechToTextProvider
    MockTextToSpeechProvider      : ITextToSpeechProvider
```

Switching from Azure to a new provider (OpenAI, DeepL, Google) requires only a new `Infrastructure/Providers/` implementation and a config value change.

---

## Audio Processing Paths

```
Browser MediaRecorder
    ├── audio/webm;codecs=opus (Chrome, Edge)
    │       └─→ POST /api/translate/audio
    │               └─→ Azure Fast Transcription REST API
    ├── audio/ogg (Firefox)
    │       └─→ POST /api/translate/audio
    │               └─→ Azure Fast Transcription REST API
    └── audio/wav (file upload)
            └─→ POST /api/translate/audio
                    └─→ Azure Speech SDK (SpeechRecognizer)
```

---

## Security Boundaries

| Boundary | Rule |
|---|---|
| Credentials | Never in frontend code, committed files, or response bodies |
| Audio | Not permanently stored; not logged |
| User text | Not logged (source or translated) |
| Errors | Structured safe envelope — no stack traces, no provider messages |
| Validation | Frontend is UX guard only; backend is the authoritative security boundary |
