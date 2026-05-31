# Architecture Overview — My Translation App v1.0

## System Diagram

```
┌─────────────────────────────────────────────────────────────────────┐
│  Browser (React 18 + TypeScript + Vite)                             │
│                                                                      │
│  App.tsx  (screen: landing | workspace)                              │
│    ├── LandingPage                                                   │
│    └── Workspace                                                     │
│          ├── TextTranslationForm   ── POST /api/translate/text ──┐  │
│          ├── AudioTranslationForm  ── POST /api/translate/audio ─┤  │
│          ├── ResultPanel           ── POST /api/translate/tts ───┤  │
│          ├── ErrorPanel                                           │  │
│          └── ConversationMode                                     │  │
│               ├── ConversationManager  (multi-conversation CRUD)  │  │
│               ├── ConversationHistory  (message bubbles)          │  │
│               └── ConversationInput   (record / text / file)      │  │
│                                                                   │  │
│  api/translationApi.ts  ──────────────────────────────────────────┘  │
│  services/ConversationStorageService.ts  (localStorage)              │
│  services/AudioCaptureService.ts  (MediaRecorder)                    │
└──────────────────────────┬──────────────────────────────────────────┘
                           │  HTTPS — no secrets in browser
                           ▼
┌─────────────────────────────────────────────────────────────────────┐
│  AWS CloudFront  (HTTPS CDN — d2ftspeokj49uq.cloudfront.net)        │
│    /api/*   →  Elastic Beanstalk (HTTP, private URL)                │
│    /*       →  S3 (static frontend assets)                          │
└──────────────┬────────────────────┬────────────────────────────────┘
               │                    │
               ▼                    ▼
┌──────────────────────┐  ┌────────────────────────────────────────┐
│  AWS S3              │  │  AWS Elastic Beanstalk                  │
│  (static assets)     │  │  .NET 8 on AL2023 — ap-southeast-2     │
│                      │  │                                         │
│  index.html          │  │  .NET 8 Backend (ASP.NET Core)          │
│  logo.png            │  │    TranslationController                │
│  favicon.png         │  │      POST /api/translate/text           │
│  assets/             │  │      POST /api/translate/audio          │
└──────────────────────┘  │      POST /api/translate/tts            │
                          │      GET  /api/languages                │
                          │      GET  /health                       │
                          │    CorrelationIdMiddleware               │
                          │    ExceptionHandlingMiddleware           │
                          │                                         │
                          │  Application Layer (no Azure refs)      │
                          │    TranslationService                   │
                          │    ITextTranslationProvider             │
                          │    ISpeechToTextProvider                │
                          │    ITextToSpeechProvider                │
                          │    ILanguageCatalogService              │
                          │                                         │
                          │  Infrastructure Layer                   │
                          │    AzureTextTranslationProvider         │
                          │    AzureSpeechToTextProvider            │
                          │    AzureTextToSpeechProvider            │
                          │    StaticLanguageCatalogService (37)    │
                          └───────────┬──────────────┬─────────────┘
                                      │              │
                                      ▼              ▼
                          ┌───────────────┐  ┌──────────────────────┐
                          │ Azure         │  │ Azure Speech Services │
                          │ Translator    │  │  ├─ STT (SDK + REST)  │
                          │ API v3.0      │  │  └─ TTS (SDK, MP3)    │
                          └───────────────┘  └──────────────────────┘
```

---

## Frontend

**Framework:** React 18 + TypeScript + Vite 5

**Navigation model (no router library):**
```typescript
screen: 'landing' | 'workspace'        // default: 'landing'
workspaceMode: 'translate' | 'conversation'
translationInputMode: 'text' | 'audio' // default: 'text'
```

**Folder structure:**
```
src/frontend/src/
├── api/
│   └── translationApi.ts             — all HTTP calls (fetch wrappers)
├── components/
│   ├── LandingPage.tsx               — landing hero
│   ├── TextTranslationForm.tsx
│   ├── AudioTranslationForm.tsx
│   ├── LanguageSelect.tsx
│   ├── PushToTalkButton.tsx
│   ├── RecordingIndicator.tsx
│   ├── RecordingTimer.tsx
│   ├── ResultPanel.tsx
│   ├── ErrorPanel.tsx
│   ├── ConversationMode.tsx          — orchestration, state
│   ├── ConversationManager.tsx       — selector, rename, search
│   ├── ConversationHistory.tsx       — message list
│   ├── ConversationInput.tsx         — record / text / file tabs
│   └── ConversationMessage.tsx       — message bubble
├── services/
│   ├── AudioCaptureService.ts        — MediaRecorder wrapper
│   ├── ConversationStorageService.ts — localStorage CRUD
│   └── ConversationExportService.ts  — TXT / JSON / clipboard
├── types/
│   ├── api.ts                        — TypeScript interfaces (mirrors DTOs)
│   └── conversation.ts               — ConversationSession, ConversationStore
└── styles/
    └── app.css                       — all styles (no UI framework)
```

---

## Backend

**Framework:** .NET 8 LTS (ASP.NET Core)

**Project structure:**
```
src/backend/
├── Api/           — Controllers, Middleware, DTOs
├── Application/   — Services, Interfaces, Validation (zero Azure refs)
├── Infrastructure/— Azure adapters, static catalog, config binding
└── Tests/         — 133 xUnit tests (WebApplicationFactory)
```

**Architecture dependency rule:** `Application` → no `Infrastructure` references. `Infrastructure` implements `Application` interfaces. `Api` is the composition root.

---

## AWS Services

| Service | Role |
|---|---|
| S3 (`my-translation-app-frontend`) | Static frontend asset hosting |
| CloudFront (`EOSQIHDJHIZ82`) | HTTPS CDN, same-origin proxy, `/api/*` → EB |
| Elastic Beanstalk (`my-translation-api-prod`) | .NET 8 API, ap-southeast-2, SingleInstance |

**Same-origin proxy pattern:**
- All browser traffic flows through CloudFront at one HTTPS domain
- EB URL never exposed to browser — eliminates mixed-content errors
- No CORS required for browser-to-API calls

---

## Azure Services

| Service | Role | SDK / API |
|---|---|---|
| Azure Translator | Text translation | `Azure.AI.Translation.Text` 1.0.0 (API v3.0) |
| Azure Speech — STT | Audio transcription (WAV) | `Microsoft.CognitiveServices.Speech` 1.50.0 |
| Azure Speech — STT | Audio transcription (MP3/WebM/OGG) | Fast Transcription REST API `2025-10-15` |
| Azure Speech — TTS | Speech synthesis (MP3 output) | `Microsoft.CognitiveServices.Speech` 1.50.0 |

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
    MockTextTranslationProvider   : ITextTranslationProvider  (dev/test)
    MockSpeechToTextProvider      : ISpeechToTextProvider     (dev/test)
    MockTextToSpeechProvider      : ITextToSpeechProvider     (dev/test)
```

Switching from Azure to a new provider requires only a new `Infrastructure/Providers/` implementation and a single config value change — no Application or API changes.

---

## Audio Processing Paths

```
Browser MediaRecorder
    ├── audio/webm;codecs=opus (Chrome, Edge)  ─→ Azure Fast Transcription REST API
    ├── audio/ogg (Firefox)                    ─→ Azure Fast Transcription REST API
    └── audio/wav (file upload)                ─→ Azure Speech SDK (SpeechRecognizer)
```

---

## Conversation Flow

```
ConversationMode
    ├── Speaker A turn:
    │     ConversationInput → record / type / file
    │         → translateAudio or translateText  (existing API)
    │         → synthesizeSpeech                 (existing API, auto-play)
    │         → message appended to history
    │         → persisted to localStorage
    └── Speaker B turn:
          (same flow, reversed language direction)
```

---

## Conversation Storage Schema

```typescript
ConversationStore {
  version: 1
  activeConversationId: string
  conversations: Record<string, ConversationSession>
}

ConversationSession {
  id: string
  title: string
  isAutoTitle: boolean
  createdAt: string
  updatedAt: string
  messages: ConversationMessage[]
}
```

localStorage key: `my-translation-app-conversations`

---

## Security Boundaries

| Boundary | Rule |
|---|---|
| Credentials | Never in frontend code, committed files, or response bodies |
| Azure credentials | EB environment variables only |
| Audio | Not permanently stored; not logged |
| User text | Not logged (source or translated) |
| Errors | Structured safe envelope — no stack traces, no provider messages |
| Validation | Backend is the authoritative security boundary |
