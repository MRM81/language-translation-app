# Architect Pack: Sprint 003 - Backend API Skeleton

## Pack Metadata

| Field | Value |
|---|---|
| Project Name | My Translation App |
| Client / Owner | Mark McLachlan |
| Project Slug | my-translation-app |
| Sprint Number | 003 |
| Sprint Name | Backend API Skeleton |
| Created Date | 2026-05-28 |
| Architect | ChatGPT Architect Layer |
| Builder Target | Claude Code / Codex / Cursor / Other |
| Status | Ready For Builder |

---

## 1. Project Context

My Translation App is a browser-first web application that helps users translate typed text or short push-to-talk microphone recordings into another language.

Sprint 001 established the MVP discovery direction.

Sprint 002 defined implementation architecture documentation, including:

- React frontend
- .NET backend
- backend proxy model
- no frontend API keys
- provider abstraction interfaces
- Azure Speech + Azure Translator as the recommended future provider family
- PostgreSQL deferred until needed
- no persistent audio storage
- push-to-talk audio, not realtime continuous listening
- typed text input and live microphone recording
- translated text output and optional future TTS playback

Sprint 003 creates the backend API skeleton only. It should establish the backend project structure, DTOs, validation shape, provider interfaces, mock providers, and translation-facing endpoints using mocks. It must not integrate real Azure services yet.

---

## 2. Sprint Goal

Create the .NET backend skeleton for the MVP translation app so future sprints can add real provider integrations without restructuring the backend. This sprint should produce runnable backend scaffolding with mock translation behavior, documented API contracts, validation boundaries, and clean SOLID service boundaries.

---

## 3. Problem Being Solved

The project needs a backend foundation before frontend or Azure integration work begins. Without a backend skeleton, the Builder may mix provider SDK code, controllers, DTOs, validation, and configuration together. Sprint 003 prevents this by creating clear layers and abstractions first.

---

## 4. Confirmed Decisions From Prior Sprints

| ID | Decision |
|---|---|
| D-001+ | Sprint 001 discovery outcomes are accepted as the MVP baseline. |
| D-019 | Frontend uses a React project structure, but no frontend work is in scope for this sprint. |
| D-020 | Backend uses a layered .NET folder/project structure. |
| D-021 | Provider interfaces belong to the backend Application layer, not Infrastructure. |
| D-022 | No URL versioning for MVP; DTOs should allow future extension. |
| D-023 | Frontend state uses component state, React Context, and a lightweight API service later. |
| D-024 | Logging must not include raw audio, source text, or translated text by default. |
| D-025 | Audio is delivered as blob upload, not streaming. |
| D-026 | PostgreSQL integration is deferred and should plug into Infrastructure later only. |

---

## 5. Sprint 003 Decisions To Apply

These are new sprint-specific decisions the Builder should record in `planning/DECISIONS.md` if not already present.

| ID | Decision | Rationale |
|---|---|---|
| D-027 | Backend skeleton should use mock providers only in Sprint 003. | Allows endpoint and validation work without secrets or Azure coupling. |
| D-028 | Text translation MVP limit is 5,000 characters. | Keeps validation simple and avoids excessive provider/cost exposure later. |
| D-029 | Audio recording MVP limit is 60 seconds. | Aligns with push-to-talk MVP and reduces upload/processing risk. |
| D-030 | Supported MVP audio MIME candidates are `audio/webm`, `audio/webm;codecs=opus`, `audio/mp4`, `audio/mpeg`, and `audio/wav`. | Covers common browser outputs while keeping validation explicit. |
| D-031 | API endpoints should return structured success/error DTOs with correlation IDs. | Supports safe debugging without logging sensitive content. |
| D-032 | Backend source structure should separate API, Application, Infrastructure, and Tests. | Supports SOLID and future provider/database integration. |
| D-033 | TTS remains optional and should not be implemented in Sprint 003 unless docs already require a stub contract only. | Keeps Sprint 003 focused on text/audio translation skeleton. |

---

## 6. Requirements

| ID | Requirement | Priority | Notes |
|---|---|---|---|
| R-003-001 | Create a .NET backend skeleton aligned to Sprint 002 architecture. | Must | No production provider integration. |
| R-003-002 | Create translation-facing API endpoints using mock providers. | Must | Text and audio translation only. |
| R-003-003 | Create DTOs for text translation request/response, audio translation response, and API errors. | Must | Names should follow Sprint 002 API conventions. |
| R-003-004 | Create provider abstraction interfaces in the Application layer. | Must | No Azure SDK references in Application. |
| R-003-005 | Create mock provider implementations in Infrastructure or a local mock provider area. | Must | Deterministic, safe mock outputs. |
| R-003-006 | Add backend validation for text length, required language fields, audio presence, audio size/duration placeholder, and accepted MIME types. | Must | Backend validation is authoritative. |
| R-003-007 | Add safe logging/correlation ID behavior. | Must | Do not log raw source text, translated text, or audio contents. |
| R-003-008 | Add basic automated tests or test placeholders for validation and service behavior. | Should | Use existing project conventions if present. |
| R-003-009 | Update planning and docs after implementation. | Must | Keep folder as source of truth. |
| R-003-010 | Keep TTS, database, auth, Azure, deployment, and frontend UI out of scope. | Must | Do not expand scope. |

---

## 7. In Scope

The Builder may work on:

- .NET backend project skeleton
- API project structure
- Application layer interfaces and services
- Infrastructure mock provider implementations
- DTOs and API response/error conventions
- text translation endpoint using mock provider
- audio translation endpoint using mock speech-to-text + mock translator provider
- validation logic for text and audio requests
- correlation ID handling
- safe logging configuration/usage
- local run instructions if appropriate
- backend tests or test skeletons
- documentation and planning file updates

---

## 8. Out Of Scope

The Builder must not work on:

- React frontend implementation
- production UI components
- Azure Speech SDK integration
- Azure Translator SDK/API integration
- real external API calls
- API keys, secrets, credentials, `.env` values, or secret files
- PostgreSQL schema, migrations, or database persistence
- authentication or user accounts
- realtime websocket or streaming architecture
- infrastructure deployment
- CI/CD
- persistent audio storage
- translation history persistence
- changing Sprint 001 or Sprint 002 decisions except to append status updates
- adding business rules not documented in prior sprint files

---

## 9. Assumptions

| ID | Assumption | Confidence | Action If Wrong |
|---|---|---|---|
| A-003-001 | The repository currently contains planning/docs scaffold and may not yet contain a .NET backend project. | High | Builder should create minimal backend skeleton. |
| A-003-002 | .NET version can be selected by Builder based on installed SDK, preferably current LTS if available. | Medium | Record selected version in docs and completion report. |
| A-003-003 | Mock providers are acceptable for Sprint 003. | High | Do not connect to Azure yet. |
| A-003-004 | Text max length is 5,000 characters for MVP. | High | Record as decision and validation rule. |
| A-003-005 | Audio max duration is 60 seconds for MVP. | High | Enforce if technically available; otherwise document placeholder and enforce file size/MIME for now. |
| A-003-006 | Accepted MIME types are the Sprint 003 candidate list. | High | Add validation rule and risk if browser output differs. |
| A-003-007 | TTS remains optional and is not required in Sprint 003. | High | Do not implement TTS endpoint unless only documenting future shape. |

---

## 10. Constraints

- Do not implement production provider integrations.
- Do not use real Azure credentials.
- Do not add secrets to repository files.
- Do not create database schema or migrations.
- Do not add authentication.
- Do not introduce realtime WebSocket or streaming design.
- Backend must be a proxy boundary; frontend never receives provider keys.
- Provider interfaces must live in Application layer.
- Infrastructure implementations must depend on Application abstractions, not the reverse.
- API DTOs must not expose Azure/provider-specific shapes.
- No raw audio, source text, or translated text should be logged by default.
- Keep MVP simple and extensible.

---

## 11. Dependencies

| Dependency | Type | Status | Notes |
|---|---|---|---|
| .NET SDK | Tooling | Required | Builder should verify installed SDK before creating project. |
| Sprint 002 docs | Documentation | Available | Source of truth for architecture. |
| Azure Speech / Translator | Future provider | Deferred | No real integration in Sprint 003. |
| PostgreSQL | Future database | Deferred | No schema/migrations in Sprint 003. |
| React frontend | Future client | Deferred | Backend contracts should be frontend-ready. |

---

## 12. Recommended Backend Project Structure

Builder should adapt to the repository, but target a simple structure like:

```text
src/
  backend/
    MyTranslationApp.Api/
      Controllers/ or Endpoints/
      Middleware/
      Program.cs
      appsettings.json
      appsettings.Development.json

    MyTranslationApp.Application/
      Interfaces/
        ISpeechToTextProvider.cs
        ITextTranslationProvider.cs
        ITextToSpeechProvider.cs
        ILanguageCatalogService.cs
      Services/
        TranslationService.cs
      DTOs/
        TextTranslationRequestDto.cs
        TranslationResponseDto.cs
        AudioTranslationResponseDto.cs
        ApiErrorResponseDto.cs
      Validation/
        TranslationValidationOptions.cs
        TranslationRequestValidator.cs

    MyTranslationApp.Infrastructure/
      Providers/
        MockSpeechToTextProvider.cs
        MockTextTranslationProvider.cs
        MockTextToSpeechProvider.cs
        StaticLanguageCatalogService.cs

tests/
  backend/
    MyTranslationApp.Application.Tests/
    MyTranslationApp.Api.Tests/
```

Notes:

- Exact .NET solution/project naming may be adjusted for valid conventions.
- If the repo already has a different backend structure, preserve existing conventions and document deviations.
- Do not create frontend source files in this sprint.
- Do not overbuild with Clean Architecture ceremony beyond what the MVP needs.

---

## 13. Provider Abstraction Interfaces

Provider interfaces should exist in the Application layer.

Minimum conceptual interfaces:

```text
ISpeechToTextProvider
- Accepts audio stream/file data and metadata.
- Returns transcribed text and optional detected language metadata.
- Does not expose provider-specific response models.

ITextTranslationProvider
- Accepts source text, source language policy, and target language.
- Returns translated text and language metadata.
- Does not expose provider-specific response models.

ITextToSpeechProvider
- Optional future interface only if included by architecture docs.
- Should not require endpoint implementation in Sprint 003.

ILanguageCatalogService
- Returns supported languages/codes.
- May be static/mock in Sprint 003.
```

Rules:

- Application layer depends on abstractions only.
- Infrastructure layer implements provider interfaces.
- API layer depends on Application services/DTOs.
- Provider SDK types must not leak into API DTOs or Application DTOs.

---

## 14. API Endpoints

Sprint 003 should implement or scaffold only translation-facing backend endpoints.

Recommended MVP endpoints:

```text
POST /api/translate/text
POST /api/translate/audio
```

Optional documentation-only future endpoint:

```text
POST /api/tts
```

Do not add `/api/health` in Sprint 003.

### POST /api/translate/text

Purpose:

Translate typed text using mock translation provider.

Request concept:

```json
{
  "sourceText": "Hello",
  "sourceLanguage": "en",
  "targetLanguage": "es"
}
```

Response concept:

```json
{
  "translatedText": "[mock-es] Hello",
  "sourceLanguage": "en",
  "targetLanguage": "es",
  "provider": "mock",
  "correlationId": "..."
}
```

Validation:

- `sourceText` required
- `sourceText` max 5,000 characters
- `targetLanguage` required
- `sourceLanguage` may be explicit or `auto`
- language codes should be basic non-empty normalized strings for now, unless language catalog is implemented

### POST /api/translate/audio

Purpose:

Accept short push-to-talk audio upload, transcribe using mock speech provider, then translate mock transcript using mock translation provider.

Request concept:

- multipart/form-data
- audio file/blob field
- targetLanguage field
- sourceLanguage optional or `auto`

Response concept:

```json
{
  "transcribedText": "[mock transcript]",
  "translatedText": "[mock-target] [mock transcript]",
  "sourceLanguage": "auto",
  "targetLanguage": "es",
  "provider": "mock",
  "correlationId": "..."
}
```

Validation:

- audio file required
- targetLanguage required
- accepted MIME types:
  - `audio/webm`
  - `audio/webm;codecs=opus`
  - `audio/mp4`
  - `audio/mpeg`
  - `audio/wav`
- maximum recording duration: 60 seconds if measurable in Sprint 003
- if duration cannot be reliably read yet, document this limitation and enforce file presence/MIME/size placeholder

---

## 15. Error Handling Strategy

Use a consistent API error response.

Conceptual shape:

```json
{
  "errorCode": "VALIDATION_ERROR",
  "message": "The request is invalid.",
  "details": [
    {
      "field": "sourceText",
      "message": "Source text is required."
    }
  ],
  "correlationId": "..."
}
```

Recommended error codes:

| Code | Meaning |
|---|---|
| VALIDATION_ERROR | Request failed validation. |
| UNSUPPORTED_AUDIO_FORMAT | Audio MIME type is not allowed. |
| AUDIO_TOO_LONG | Audio duration exceeds MVP limit. |
| TEXT_TOO_LONG | Text exceeds 5,000 characters. |
| PROVIDER_TIMEOUT | Future provider timeout abstraction. |
| PROVIDER_UNAVAILABLE | Future provider unavailable abstraction. |
| INTERNAL_ERROR | Safe fallback error. |

Rules:

- Do not leak provider raw errors to clients.
- Include correlation ID.
- Keep user-facing error messages safe and plain.
- Log technical details without sensitive payload content.

---

## 16. Configuration / Environment Strategy

Sprint 003 should create safe configuration placeholders only.

Allowed:

- validation limits
- allowed MIME types
- mock provider selection
- logging level
- app environment defaults

Not allowed:

- real Azure keys
- subscription IDs
- endpoint secrets
- database connection strings
- deployment secrets

Configuration should make it obvious how Sprint 004+ can add providers later without refactoring.

Example conceptual config keys:

```text
Translation:MaxTextCharacters = 5000
Translation:MaxAudioSeconds = 60
Translation:AllowedAudioMimeTypes = [...]
Translation:Provider = Mock
```

Do not add real secrets.

---

## 17. Logging Rules

Logging must support debugging without storing sensitive translation content.

Allowed by default:

- correlation ID
- endpoint name
- request start/end time
- request duration
- validation failure category
- provider name such as `mock`
- high-level error code

Forbidden by default:

- raw audio contents
- uploaded audio bytes
- source text
- translated text
- transcribed text
- API keys
- provider secrets
- full provider raw responses

Local development debug logging must still not log sensitive content unless explicitly approved in a future sprint.

---

## 18. Validation Rules

Authoritative validation belongs on the backend.

Minimum Sprint 003 validation:

| Rule | MVP Decision |
|---|---|
| Source text required for text endpoint | Yes |
| Source text max length | 5,000 characters |
| Target language required | Yes |
| Source language | Required or `auto` depending current docs; prefer allowing `auto` |
| Audio file required | Yes |
| Accepted audio MIME types | `audio/webm`, `audio/webm;codecs=opus`, `audio/mp4`, `audio/mpeg`, `audio/wav` |
| Max recording duration | 60 seconds |
| Persistent audio storage | Not allowed |
| Frontend validation | Future UX guard only |
| Backend validation | Authoritative |

If the Builder cannot technically enforce audio duration yet, it must:

1. document the limitation,
2. add a TODO or test placeholder,
3. enforce MIME type and file presence,
4. avoid fake validation claims.

---

## 19. Test Strategy For Sprint 003

Minimum tests or validation checks should cover:

- text request with valid input returns mock translation
- empty source text returns validation error
- text over 5,000 characters returns validation error
- missing target language returns validation error
- valid audio MIME accepted
- unsupported audio MIME rejected
- audio endpoint returns mock transcript + mock translation
- error responses include correlation ID
- provider interfaces are not provider-specific
- no Azure SDK dependency introduced

If automated tests are not practical in this sprint, Builder must explain why and provide manual validation commands/checks.

---

## 20. Files To Read First

The Builder must read these before doing work:

1. `AGENTS.md`
2. `planning/STATE.md`
3. `planning/DECISIONS.md`
4. `planning/DOMAIN.md`
5. `planning/RISKS.md`
6. `planning/QUESTIONS.md`
7. `planning/FILE_INVENTORY.md`
8. `docs/ARCHITECTURE.md`
9. `docs/API.md`
10. `docs/VALIDATION.md`
11. `planning/sprints/002-implementation-architecture/requirements.md`
12. `planning/sprints/002-implementation-architecture/blueprint.md`
13. `planning/sprints/002-implementation-architecture/acceptance.md`
14. `planning/sprints/002-implementation-architecture/handoff-prompt.md`

---

## 21. Files To Create Or Modify

| Path | Action | Purpose |
|---|---|---|
| `planning/STATE.md` | Modify | Mark Sprint 002 completed and Sprint 003 active. |
| `planning/DECISIONS.md` | Modify | Add Sprint 003 decisions D-027+. |
| `planning/RISKS.md` | Modify | Add or update backend skeleton risks. |
| `planning/QUESTIONS.md` | Modify | Resolve Q-009/Q-010/Q-011 if documented; add any new open questions. |
| `planning/FILE_INVENTORY.md` | Modify | Add Sprint 003 files and backend project files. |
| `docs/API.md` | Modify | Update with actual skeleton endpoint paths and DTOs if implemented. |
| `docs/ARCHITECTURE.md` | Modify if needed | Record actual backend structure if different from Sprint 002 plan. |
| `docs/VALIDATION.md` | Modify | Record actual validation rules implemented. |
| `planning/sprints/003-backend-api-skeleton/requirements.md` | Create | Sprint 003 requirements. |
| `planning/sprints/003-backend-api-skeleton/blueprint.md` | Create | Builder implementation plan. |
| `planning/sprints/003-backend-api-skeleton/acceptance.md` | Create | Done criteria. |
| `planning/sprints/003-backend-api-skeleton/handoff-prompt.md` | Create | Builder handoff prompt. |
| `src/backend/` | Create/Modify | Backend skeleton. |
| `tests/backend/` | Create/Modify if practical | Backend tests. |

---

## 22. Blueprint

### Step 1: Dry Run

Builder must first:

- read required files
- inspect existing repo structure
- identify whether a .NET solution/project already exists
- identify installed .NET SDK if possible
- summarize intended file changes
- list assumptions and risks
- stop for approval before applying

### Step 2: Create Sprint 003 Documentation

Create:

```text
planning/sprints/003-backend-api-skeleton/
  requirements.md
  blueprint.md
  acceptance.md
  handoff-prompt.md
```

These should mirror the approved Sprint 003 pack and become the durable project-folder sprint handoff.

### Step 3: Create Backend Skeleton

Create a minimal .NET backend solution/project structure.

Target architecture:

```text
API -> Application -> Infrastructure
```

Where:

- API owns HTTP endpoints and request binding.
- Application owns interfaces, DTOs, validation, and orchestration services.
- Infrastructure owns mock provider implementations.
- Tests verify validation and mock behavior.

### Step 4: Implement DTOs and Interfaces

Create DTOs and provider interfaces without provider-specific SDK types.

Minimum DTO concepts:

- `TextTranslationRequestDto`
- `TranslationResponseDto`
- `AudioTranslationResponseDto`
- `ApiErrorResponseDto`
- optional `ApiErrorDetailDto`

Minimum interfaces:

- `ISpeechToTextProvider`
- `ITextTranslationProvider`
- `ITextToSpeechProvider` only if used as future placeholder
- `ILanguageCatalogService`

### Step 5: Implement Mock Providers

Mock provider behavior should be deterministic and safe.

Examples:

- mock speech provider returns `[mock transcript]`
- mock translator returns `[mock-{targetLanguage}] {input}`
- static language catalog returns a small list of common language codes if needed

Do not pretend to perform real translation.

### Step 6: Implement Validation

Validation should cover:

- required fields
- text length
- target language
- allowed MIME types
- audio presence
- audio duration placeholder or enforcement
- safe structured errors

### Step 7: Implement Endpoints

Add:

- `POST /api/translate/text`
- `POST /api/translate/audio`

Do not add:

- `/api/health`
- auth endpoints
- database endpoints
- provider admin endpoints
- websocket endpoints

### Step 8: Add Tests / Validation

Run or create tests if project conventions support it.

At minimum verify:

- successful text translation mock
- validation errors
- unsupported MIME rejection
- correlation ID included
- no Azure dependency added

### Step 9: Update Documentation and Planning

Update:

- `planning/STATE.md`
- `planning/DECISIONS.md`
- `planning/RISKS.md`
- `planning/QUESTIONS.md`
- `planning/FILE_INVENTORY.md`
- `docs/API.md`
- `docs/VALIDATION.md`
- `docs/ARCHITECTURE.md` if actual structure differs

---

## 23. Data / Request Flow

### Typed Text Translation

```text
Browser client
-> POST /api/translate/text
-> API DTO binding
-> Backend validation
-> Application TranslationService
-> ITextTranslationProvider mock implementation
-> TranslationResponseDto
-> API response with correlationId
```

### Push-To-Talk Audio Translation

```text
Browser MediaRecorder blob
-> POST /api/translate/audio multipart/form-data
-> API file binding
-> MIME/file validation
-> Application TranslationService
-> ISpeechToTextProvider mock implementation
-> ITextTranslationProvider mock implementation
-> AudioTranslationResponseDto
-> API response with correlationId
```

### Future Azure Integration

```text
Application provider interface
-> Infrastructure Azure provider implementation
-> Azure SDK/API
-> normalized Application result
```

Azure must remain behind Infrastructure implementations in future sprints.

---

## 24. API / Integration Notes

- Frontend must only call internal backend endpoints.
- Backend must proxy all future provider calls.
- DTOs must be provider-neutral.
- Correlation IDs should be returned in success and error responses.
- Errors should use stable error codes.
- No external provider calls in Sprint 003.
- No frontend API keys ever.
- No raw provider responses returned to clients.

---

## 25. Validation Plan

| Check | Method | Expected Result |
|---|---|---|
| Backend skeleton exists | File inspection | API/Application/Infrastructure separation is visible. |
| Sprint files exist | File inspection | Four Sprint 003 files exist and are non-empty. |
| Text endpoint valid request | Automated or manual test | Returns deterministic mock translation. |
| Text endpoint empty text | Automated or manual test | Returns structured validation error. |
| Text endpoint over 5,000 chars | Automated or manual test | Returns text length error. |
| Audio endpoint valid MIME | Automated or manual test | Returns mock transcript and mock translation. |
| Audio endpoint invalid MIME | Automated or manual test | Returns unsupported audio format error. |
| Correlation ID | Response inspection | Success/error responses include correlationId. |
| No secrets | Search repo | No API keys/secrets/credentials added. |
| No Azure SDK | Dependency inspection | No Azure SDK dependency added. |
| No DB/auth/deployment | File inspection | No schema, auth, deployment config created. |
| Logging safety | Code/doc inspection | Sensitive payloads are not logged. |

---

## 26. Acceptance Criteria

Sprint is complete when:

- [ ] Builder performs dry run and waits for approval.
- [ ] `planning/sprints/003-backend-api-skeleton/requirements.md` exists and is complete.
- [ ] `planning/sprints/003-backend-api-skeleton/blueprint.md` exists and is complete.
- [ ] `planning/sprints/003-backend-api-skeleton/acceptance.md` exists and is complete.
- [ ] `planning/sprints/003-backend-api-skeleton/handoff-prompt.md` exists and is complete.
- [ ] Backend skeleton exists under an agreed backend path.
- [ ] API/Application/Infrastructure boundaries are clear.
- [ ] Provider interfaces exist in Application layer.
- [ ] Mock providers exist and are clearly labelled as mock.
- [ ] Text translation endpoint exists and uses mock provider only.
- [ ] Audio translation endpoint exists and uses mock providers only.
- [ ] DTOs are provider-neutral.
- [ ] Structured error DTO exists.
- [ ] Correlation ID appears in responses.
- [ ] Validation exists for required text, max text length, target language, audio presence, and MIME types.
- [ ] Audio max duration is enforced or explicitly documented as not enforceable yet with a follow-up note.
- [ ] No Azure SDK/API integration is added.
- [ ] No secrets or credentials are added.
- [ ] No database schema/migrations are added.
- [ ] No authentication is added.
- [ ] No frontend production UI is added.
- [ ] No websocket/realtime architecture is added.
- [ ] Relevant tests or manual validation checks are completed.
- [ ] `planning/STATE.md` is updated.
- [ ] `planning/DECISIONS.md` is updated.
- [ ] `planning/RISKS.md` is updated if risks changed.
- [ ] `planning/QUESTIONS.md` is updated if questions changed.
- [ ] `planning/FILE_INVENTORY.md` is updated.
- [ ] `docs/API.md` and `docs/VALIDATION.md` reflect actual implemented skeleton.
- [ ] No Sprint 001 or Sprint 002 scope is contradicted.

---

## 27. Risks

| Risk | Impact | Likelihood | Mitigation | Owner |
|---|---|---|---|---|
| Backend skeleton becomes over-engineered | Medium | Medium | Keep only API/Application/Infrastructure and tests. | Builder |
| Provider abstractions accidentally mirror Azure SDK too closely | High | Medium | Keep provider-neutral request/response types. | Builder |
| Audio duration cannot be validated without decoding libraries | Medium | Medium | Document limitation; enforce MIME/file limits now; resolve later. | Builder |
| Mock behavior mistaken for real translation | Medium | Low | Prefix mock outputs and document clearly. | Builder |
| Logging accidentally captures source/translated text | High | Medium | Review logs and code for sensitive content before completion. | Builder |
| API contract drifts from Sprint 002 docs | Medium | Medium | Update docs/API.md to match actual skeleton. | Builder |

---

## 28. Open Questions

These should be updated in `planning/QUESTIONS.md`.

| Question | Needed From | Blocking? | Notes |
|---|---|---|---|
| Should source language be manual, auto-detect, or both? | Architect / Owner | No | Sprint 003 may support `auto` as a string policy for now. |
| Should TTS be included in Sprint 004 or deferred? | Architect / Owner | No | Do not implement TTS in Sprint 003. |
| What exact hosting/deployment target will be used later? | Owner / Builder | No | Deployment is out of scope. |
| Is 60-second audio duration enforceable without additional audio parsing dependencies? | Builder | No | If not, document limitation and defer exact enforcement. |

Resolved or partially resolved by this pack:

| Prior Question | Sprint 003 Direction |
|---|---|
| Q-009 maximum text length | 5,000 characters |
| Q-010 maximum recording duration | 60 seconds |
| Q-011 accepted browser audio MIME types | `audio/webm`, `audio/webm;codecs=opus`, `audio/mp4`, `audio/mpeg`, `audio/wav` |

---

## 29. State Updates Required

At the end of Sprint 003, update:

- `planning/STATE.md`
- `planning/DECISIONS.md`
- `planning/RISKS.md`
- `planning/QUESTIONS.md`
- `planning/FILE_INVENTORY.md`
- `docs/API.md`
- `docs/VALIDATION.md`
- `docs/ARCHITECTURE.md` if actual implementation structure differs
- Sprint 003 acceptance status

Recommended `planning/STATE.md` status after successful apply:

```text
Sprint 002: Completed
Sprint 003: Active or Completed, depending stage
Current focus: Backend API skeleton with mock providers
Next likely sprint: Frontend MVP Shell or Azure Provider Integration
```

---

## 30. Builder Dry Run Instructions

Before implementation, the Builder must produce a dry run summary.

The dry run must include:

- files read
- current repo structure observed
- whether .NET project exists already
- installed .NET SDK or intended SDK version
- planned file changes
- planned commands
- endpoint plan
- DTO/interface plan
- assumptions
- risks or ambiguities
- validation/test plan
- whether implementation is safe to start

Do not implement until the dry run has been reviewed and approved.

---

## 31. Builder Handoff Prompt

Copy this prompt into the Builder chat.

```markdown
You are the Builder for My Translation App.

You are working on Sprint 003: Backend API Skeleton.

Follow the 120x Architect / Builder methodology.

Read these files first:

1. AGENTS.md
2. planning/STATE.md
3. planning/DECISIONS.md
4. planning/DOMAIN.md
5. planning/RISKS.md
6. planning/QUESTIONS.md
7. planning/FILE_INVENTORY.md
8. docs/ARCHITECTURE.md
9. docs/API.md
10. docs/VALIDATION.md
11. planning/sprints/002-implementation-architecture/requirements.md
12. planning/sprints/002-implementation-architecture/blueprint.md
13. planning/sprints/002-implementation-architecture/acceptance.md
14. planning/sprints/002-implementation-architecture/handoff-prompt.md
15. architect-pack-003-backend-api-skeleton.md

Your task is to apply Sprint 003 only after a dry run is approved.

Sprint 003 goal:

Create the .NET backend API skeleton for the MVP translation app using mock providers only.

In scope:

- .NET backend skeleton
- API/Application/Infrastructure separation
- text translation endpoint using mock provider
- audio translation endpoint using mock providers
- provider interfaces in Application layer
- mock providers in Infrastructure
- provider-neutral DTOs
- structured error DTO
- correlation IDs
- validation for:
  - required source text
  - 5,000 character text max
  - target language required
  - audio file required
  - accepted audio MIME types
  - 60 second max audio duration if feasible
- safe logging with no raw audio/source text/transcribed text/translated text
- tests or manual validation checks
- planning and docs updates

Out of scope:

- React frontend implementation
- Azure Speech integration
- Azure Translator integration
- real external API calls
- secrets or credentials
- database schema/migrations
- authentication
- deployment/CI/CD
- websocket/realtime streaming
- persistent audio storage
- TTS endpoint implementation unless only a future placeholder already exists in docs

Required dry run:

Before changing files, summarize:

1. Files read.
2. Existing repo/backend structure.
3. Whether a .NET backend already exists.
4. Installed or intended .NET SDK version.
5. Files you expect to create or modify.
6. Commands you expect to run.
7. Endpoint/DTO/interface plan.
8. Assumptions.
9. Risks or ambiguities.
10. Validation/test plan.
11. Whether implementation is safe to start.

Stop after the dry run and wait for approval.

Do not invent business rules.
Do not redefine project scope.
Do not store secrets or credentials.
Do not add Azure SDKs in Sprint 003.
Use the project files as the source of truth.
```

---

## 32. Completion Report Template

When finished, the Builder should report:

```markdown
# Sprint 003 Completion Report

## Summary

## Files Created

## Files Modified

## Commands Run

## Tests / Validation

## Endpoint Summary

## DTOs Added

## Interfaces Added

## Mock Providers Added

## Acceptance Criteria Status

## Decisions Added

## Risks Added Or Updated

## Questions Added Or Resolved

## Known Limitations

## Recommended Next Sprint
```

---

## 33. Recommended Next Sprint Options

After Sprint 003 completion, likely next sprint options are:

### Option A: Sprint 004 - Frontend MVP Shell

Build the React UI shell against mock backend endpoints.

Best when:

- backend skeleton is stable
- user wants visible app workflow quickly
- Azure integration can wait

### Option B: Sprint 004 - Azure Provider Integration

Replace mock providers with Azure Speech + Translator infrastructure implementations.

Best when:

- Azure account/service setup is available
- provider configuration decisions are resolved
- backend API skeleton tests are passing

Recommended default:

```text
Sprint 004 - Frontend MVP Shell
```

Reason: it proves the full user workflow with mocks before introducing external provider cost, secrets, and configuration complexity.
