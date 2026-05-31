# Project State

## Current Status

Sprint 012 (AWS Production Deployment) is complete. Application is live on AWS. All acceptance criteria met. 133/133 tests pass. Build clean.

**Project:** My Translation App
**Client:** Acme Corp

---

## Production URLs

| Service | URL |
|---|---|
| Frontend (CloudFront HTTPS) | https://d2ftspeokj49uq.cloudfront.net |
| Backend (Elastic Beanstalk HTTP) | http://my-translation-api-prod.eba-pahyptkw.ap-southeast-2.elasticbeanstalk.com |
| Health endpoint | https://d2ftspeokj49uq.cloudfront.net/health |

---

## Active Sprint

None. Sprint 012 is complete. Recommended next sprint: Sprint 013 — Conversation Mode.

---

## Completed Sprints

### Sprint 012 — AWS Production Deployment
Status: Completed
Completed: 2026-05-31

Outcomes:
- Backend deployed to Elastic Beanstalk (`my-translation-api-prod`, ap-southeast-2, .NET 8 on AL2023, SingleInstance)
- Frontend deployed to S3 (`my-translation-app-frontend`) + CloudFront (`EOSQIHDJHIZ82`, `d2ftspeokj49uq.cloudfront.net`)
- Same-origin CloudFront proxy pattern: `/api/*` and `/health` → EB; `/*` → S3 (avoids mixed-content, eliminates CORS for browser calls)
- All production validation passed: health, 37-language catalog, translation (en→es/zh-Hans), TTS, STT round-trip
- `src/frontend/public/phase0-test.html` removed (Sprint 006 dev artifact — not appropriate for production)
- IAM role `aws-elasticbeanstalk-ec2-role` created
- D-086 to D-089 added; R-052 to R-054 added; Q-045 resolved; Q-047 to Q-048 updated
- `docs/PRODUCTION_DEPLOYMENT_REPORT.md` created
- 133/133 tests pass; TypeScript clean; build clean

### Sprint 011 — AWS Deployment Preparation
Status: Completed
Completed: 2026-05-31

Outcomes:
- `src/frontend/src/api/translationApi.ts` — `API_BASE` reads `import.meta.env.VITE_API_BASE_URL ?? ''` (1-line fix that unblocks separate-origin AWS deployment)
- `src/frontend/.env.production.example` — documents `VITE_API_BASE_URL` for production builds
- `.gitignore` — `.env.production.example` allowed
- `docs/AWS_DEPLOYMENT.md` — full AWS deployment guide (EB + S3 + CloudFront, CORS, health check, rollback, cost)
- `docs/AWS_ARCHITECTURE.md` — architecture diagram, cross-cloud rationale, environment variable reference
- `docs/DEPLOYMENT.md`, `docs/ENVIRONMENTS.md`, `docs/OPERATIONS.md` — AWS sections added
- D-082 to D-085 added; R-048 to R-051 added; Q-041 resolved; Q-045 to Q-047 added
- 133/133 tests pass; TypeScript clean; build clean

### Sprint 010 — Deployment Readiness Hardening
Status: Completed
Completed: 2026-05-31

Outcomes:
- `Program.cs` — `GET /health` endpoint; config-driven CORS (`AllowedCorsOrigins`); startup `Provider mode:` log
- `appsettings.json` — `AllowedCorsOrigins` default; `AzureSpeech:Endpoint` placeholder
- `appsettings.Production.json` — new file: `Translation:Provider=Azure`, Warning log levels
- `launchSettings.json` — stale `launchUrl: "swagger"` removed
- `src/backend/README.md` — `AzureSpeech:Endpoint` added to User Secrets setup and config table
- `HealthEndpointTests.cs` — 4 new integration tests
- `docs/DEPLOYMENT.md`, `docs/ENVIRONMENTS.md`, `docs/OPERATIONS.md` — created
- D-078 to D-081 added; R-045 to R-047 added; Q-041 to Q-044 added
- 133/133 tests pass; build clean

### Sprint 009B — Language Capability Metadata & Capability-Aware UI
Status: Completed
Completed: 2026-05-31

Outcomes:
- `LanguageOptionDto.cs` — added `SupportsTextTranslation`, `SupportsSpeechToText`, `SupportsTextToSpeech` (bool, default `true`)
- `src/frontend/src/types/api.ts` — added 3 capability fields to `LanguageOption`
- `src/frontend/src/App.tsx` — derives `targetLangSupportsTts` from loaded languages; passes to `ResultPanel`
- `src/frontend/src/components/ResultPanel.tsx` — `targetLangSupportsTts` prop; Play button disabled with "Audio unavailable" when `false`
- `LanguageCapabilityTests.cs` — 14 new integration tests via `GET /api/languages`
- Dry run correctly identified voice playback as already complete (Sprint 007); no duplicate work done
- D-074 to D-077 added; R-042 to R-044 added; Q-037 to Q-040 resolved
- 129/129 tests pass; TypeScript clean; build clean

### Sprint 009A — Language Catalog Expansion
Status: Completed
Completed: 2026-05-31

Outcomes:
- `StaticLanguageCatalogService.cs` — expanded from 10 to 37 languages, alphabetical order
- `AzureSpeechToTextProvider.cs` — `LanguageMap` expanded from 10 to 37 entries
- `AzureTextToSpeechProvider.cs` — `VoiceMap` and `LocaleMap` expanded from 10 to 37 entries
- `LanguageCatalogTests.cs` — new test file, 44 tests covering catalog size, uniqueness, all new languages
- D-070 to D-073 added; R-038 to R-041 added; Q-034 to Q-036 added
- 115/115 tests pass; build clean; no frontend changes required
- Live Azure validation pending (requires credentials)

### Sprint 008 — UX Modernization, Mobile Responsiveness & Portfolio Showcase
Status: Completed
Completed: 2026-05-30

Outcomes:
- `app.css` — full CSS overhaul: CSS custom properties at `:root` (color system, radius scale), dark navy theme (`#0d1117` body, `#161b2e` surfaces), indigo accent CTAs (`#6366f1`), responsive at 320/375/768/1024/1440px
- `LanguageSelect.tsx` — `required` prop added; `aria-required` support
- `TextTranslationForm.tsx` — language pair row with ⇄ swap button
- `AudioTranslationForm.tsx` — language pair row with ⇄ swap button
- `PushToTalkButton.tsx` — inline SVG mic icon; circular button (CSS)
- `ResultPanel.tsx` — hero translation text (1.375rem, weight 700); transcript as secondary; play button with ▶ icon; provider/correlationId as de-emphasized meta footer
- `App.tsx` — `aria-live="polite"` on results area
- `index.html` — `<meta name="description">` added
- `portfolio/case-study.md`, `portfolio/architecture-overview.md`, `portfolio/demo-script.md`, `portfolio/lessons-learned.md` created
- `design/style-guide.md`, `design/decisions.md` created
- Baseline screenshots captured: `design/screenshots/sprint-008-baseline/` (5 breakpoints)
- Final screenshots captured: `design/screenshots/sprint-008-final/` + `design/screenshots/portfolio/`
- D-063 to D-069 recorded; R-035 to R-037 added; Q-031 to Q-033 added
- 71/71 tests pass; TypeScript clean; build clean

### Sprint 007 — Text-to-Speech Playback
Status: Completed
Completed: 2026-05-30

Outcomes:
- `AzureTextToSpeechProvider` — Azure Speech SDK TTS, MP3 output, voice map for 10 languages, language-based fallback for unmapped codes
- `MockTextToSpeechProvider` — updated to return 44-byte silent PCM WAV fixture (`audio/wav`) instead of empty bytes
- `TextToSpeechService` — thin application service wrapping `ITextToSpeechProvider`
- `TtsSynthesisRequestDto` — inbound DTO for text + language
- `TranslationRequestValidator.ValidateTtsRequest` — validates non-empty text and language
- `TranslationController [HttpPost("tts")]` — `POST /api/translate/tts`, returns binary audio, correlation ID in `X-Correlation-ID` header
- `Program.cs` — `ITextToSpeechProvider` now provider-switched (`MockTextToSpeechProvider` in Mock, `AzureTextToSpeechProvider` in Azure)
- `ResultPanel.tsx` — Play button with four states (idle/loading/playing/error); object URL cleanup; duplicate playback prevention
- `synthesizeSpeech` added to `translationApi.ts`
- Play button styles added to `app.css`
- 8 new `TtsControllerTests`, 2 new `ProviderSelectionTests` — 71/71 tests pass
- D-058 to D-062 recorded; R-031 to R-034 added; Q-028 to Q-030 added; Q-029 resolved
- Manual Azure TTS validation pending user confirmation

### Sprint 006 — Audio Capture UX / Push-to-Talk
Status: Completed
Completed: 2026-05-30

Outcomes:
- `AudioCaptureService` — runtime MIME detection via `MediaRecorder.isTypeSupported()`, recording lifecycle, blob generation
- `PushToTalkButton` — three-state toggle (idle / recording / uploading)
- `RecordingIndicator` — pulsing dot shown during active recording
- `RecordingTimer` — elapsed/max time counter, turns red within 10 s of limit
- `AudioTranslationForm` rewritten — Record (push-to-talk, default) and Upload File (fallback) tabs; shared language selectors; auto-stops at 60 s; MediaRecorder cleaned up on unmount
- `app.css` updated — tab toggle, record/stop button, indicator pulse animation, timer styles
- Phase 0 live validation confirmed: Chrome 148, `audio/webm;codecs=opus`, HTTP 200, Azure Fast Transcription, Q-025 resolved
- `appsettings.json` `audio/ogg` omission fixed (Sprint 005.2 post-completion)
- `AzureSpeechProviderRoutingTests.AzureProvider_MissingSpeechEndpoint_ThrowsOnStartup` fixed — User Secrets isolation (set `AzureSpeech:Endpoint = ""` to override Phase 0 credentials)
- 61/61 tests pass; `npx tsc --noEmit` clean; `npm run build` clean
- D-052 to D-057 recorded; R-027 to R-030 added (R-030 mitigated); Q-025 resolved; Q-026/Q-027 added
- Sprint 007 (Text-to-Speech Playback) recommended as next sprint

### Sprint 001 — Discovery Architecture
Status: Completed
Completed: 2026-05-28

Outcomes:
- MVP scope defined: browser-based text and push-to-talk translation.
- Domain notes, decisions (D-001 to D-018), risks (R-001 to R-010), and open questions (Q-001 to Q-007) documented.
- High-level architecture documented in docs/ARCHITECTURE.md.
- API boundaries documented in docs/API.md.
- Validation rules documented in docs/VALIDATION.md.
- Sprint 001 folder and four sprint files created under planning/sprints/001-discovery-architecture/.

### Sprint 002 — Implementation Architecture
Status: Completed
Completed: 2026-05-28

Outcomes:
- Full implementation architecture documented.
- Frontend and backend folder structures defined.
- Provider abstraction interfaces defined.
- Service boundaries documented.
- Configuration and environment strategy documented.
- Security boundaries documented.
- Data flow sequences documented.
- Implementation sequencing defined.
- Sprint 002 folder and four sprint files created under planning/sprints/002-implementation-architecture/.
- docs/ARCHITECTURE.md, docs/API.md, docs/VALIDATION.md updated with Sprint 002 content.

### Sprint 003 — Backend API Skeleton
Status: Completed
Completed: 2026-05-28

Outcomes:
- .NET 8 solution with Api / Application / Infrastructure / Tests projects
- Provider interfaces in Application layer (ISpeechToTextProvider, ITextTranslationProvider, ITextToSpeechProvider, ILanguageCatalogService)
- Mock providers in Infrastructure layer
- DTOs and structured error envelope
- Centralised error code constants
- Validation for text, audio, and MIME types
- MIME normalisation (parameter stripping)
- CorrelationIdMiddleware (reuses or generates X-Correlation-ID)
- ExceptionHandlingMiddleware (safe error envelope)
- POST /api/translate/text endpoint
- POST /api/translate/audio endpoint
- GET /api/languages endpoint
- Kestrel and FormOptions size limits
- 30/30 tests passing
- Planning and docs files updated

### Sprint 004 — Frontend MVP Shell
Status: Completed
Completed: 2026-05-28

Outcomes:
- React + TypeScript + Vite frontend at src/frontend/
- TypeScript types matching backend DTOs exactly (src/frontend/src/types/api.ts)
- API client for all three backend endpoints (src/frontend/src/api/translationApi.ts)
- LanguageSelect, TextTranslationForm, AudioTranslationForm, ResultPanel, ErrorPanel components
- App.tsx — language loading on mount, shared state, error/result routing
- Vite dev server proxy for /api/* → http://localhost:5074 (avoids CORS in dev)
- Minimal CORS in Program.cs (localhost:5173 only, local dev intent)
- Clean MVP styles (app.css)
- .env.example documents backend URL (port 5074)
- src/frontend/README.md with prerequisites, run commands, manual validation checklist
- npm install: 114 packages, no secrets
- npx tsc --noEmit: no errors
- npm run build: succeeds (dist/ generated)
- Backend build: succeeds, 0 warnings, 0 errors

### Sprint 005.2 — Audio Format Compatibility Fix
Status: Completed
Completed: 2026-05-30

Outcomes:
- Compressed audio (MP3, WebM/Opus, OGG/Opus) routed to Azure Speech Fast Transcription REST API (api-version 2025-10-15)
- GStreamer dependency eliminated — app works on standard Windows dev machines
- `AzureSpeech:Endpoint` added as required config field; startup validation updated
- `Microsoft.Extensions.Http` 8.0.0 added to Infrastructure project
- `audio/ogg` added to `AllowedAudioMimeTypes` — Firefox MediaRecorder output now accepted
- WAV path unchanged — still uses Speech SDK `AudioConfig.FromWavFileInput`
- `AzureSpeechProviderRoutingTests` added — 15 new unit tests covering routing, request shape, response parsing, error mapping
- `ProviderSelectionTests` updated — 6 tests updated to include `AzureSpeech:Endpoint`; 1 new `MissingSpeechEndpoint` test
- `TranslationValidationTests` updated — `AllAcceptedMimeTypes_AreAllowed` updated to include `audio/ogg`
- 61 total tests, 61/61 pass
- Decisions D-049 to D-051 added
- R-023 (WebM) updated to Mitigated
- Q-022 resolved; Q-025 added (WebM live validation pending)
- Sprint 006 (push-to-talk) unblocked

Post-completion fix (2026-05-30 — found during Sprint 006 Phase 0 prep):
- `appsettings.json` was missing `audio/ogg` from `AllowedAudioMimeTypes`. Sprint 005.2 added it to
  `TranslationValidationOptions.cs` code defaults but not to the JSON config file. In .NET, a JSON
  array fully replaces code defaults at runtime, so `audio/ogg` was absent from the live validator
  despite the tests passing (tests use code defaults). Fixed: `audio/ogg` added to `appsettings.json`.

### Sprint 005 — Azure Provider Integration
Status: Completed
Completed: 2026-05-28

Outcomes:
- ProviderException added to Application layer
- ExceptionHandlingMiddleware updated to handle ProviderException (502 response with specific error code)
- ProviderName property added to ITextTranslationProvider and ISpeechToTextProvider
- TranslationService uses dynamic ProviderName in response DTOs and logs
- AzureTranslationOptions and AzureSpeechOptions config models added to Infrastructure
- Azure.AI.Translation.Text 1.0.0 (downgraded from 2.0.0 in Sprint 005.1 — see D-047) and Microsoft.CognitiveServices.Speech 1.50.0 added to Infrastructure
- AzureTextTranslationProvider implemented (TextTranslationClient, safe error mapping)
- AzureSpeechToTextProvider implemented (SpeechRecognizer, WAV temp file + compressed push stream, BCP-47 language map)
- Program.cs: config-driven provider selection (Mock / Azure), startup validation for Azure mode only
- appsettings.json: placeholder AzureTranslator and AzureSpeech sections (no real credentials)
- UserSecretsId added to Api.csproj
- 15 new provider selection / config validation tests (45 total, 45/45 pass)
- Backend README updated with Azure setup, User Secrets, manual validation, and provider switching docs
- Planning files and docs updated

---

## Next Actions

1. Sprint 013 — Conversation Mode (recommended next sprint per Architect Pack 012).
2. Custom domain (Q-046) — Route 53 + ACM certificate (optional post-012 enhancement).
3. CI/CD pipeline (Q-047) — GitHub Actions or AWS CodePipeline (optional post-012 enhancement).
4. Load-balanced EB with HTTPS (Q-048) — required for separate-origin pattern and direct HTTPS to EB.
5. SSM Parameter Store for Azure credentials (R-053 mitigation).
6. Optionally: custom dropdown with per-language capability badges (deferred from Sprint 009B).

## Last Updated

2026-05-31 (Sprint 012 complete — application deployed to AWS, all acceptance criteria met)

---

## Blockers

None. Application is live. Audio duration enforcement remains deferred (R-016, Q-015).
WebM live validation confirmed via Phase 0. MP3 and WAV live validation pending (no credentials required issue — see src/backend/README.md).
