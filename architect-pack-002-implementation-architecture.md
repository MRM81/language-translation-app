# Architect Pack: Sprint 002 - Implementation Architecture

## Pack Metadata

| Field | Value |
|---|---|
| Project Name | My Translation App |
| Client / Owner | Acme Corp |
| Project Slug | my-translation-app |
| Sprint Number | 002 |
| Sprint Name | Implementation Architecture |
| Created Date | 2026-05-28 |
| Architect | ChatGPT Architect Layer |
| Builder Target | Claude Code / Codex / Cursor / Other |
| Status | Ready For Builder Dry Run |

---

## 1. Project Context

My Translation App is a browser-first web application for helping people communicate across languages. The MVP will support typed text translation and push-to-talk microphone translation. The app should return translated text and optionally play translated speech through text-to-speech.

Sprint 001 established the product direction and discovery outcomes. Sprint 002 does not implement production features. Its purpose is to turn the approved MVP decisions into durable implementation architecture documentation and sprint handoff files so future Builder work can proceed without guessing.

Confirmed Sprint 001 decisions:

- Frontend: React
- Backend: .NET
- Database: PostgreSQL later if needed
- Platform: browser-first web app
- Translation model: Azure Speech + Azure Translator recommended
- Workflow: push-to-talk translation, not realtime continuous listening
- Inputs: typed text + live microphone recording
- Outputs: translated text + optional TTS playback
- Accounts: none for MVP
- Storage: no persistent audio storage
- Architecture: backend proxy only; no frontend API keys

---

## 2. Sprint Goal

Define the implementation architecture and project structure for the MVP translation app without building production features. The Builder must create or update project documentation that explains the frontend structure, backend structure, API layer, provider abstractions, request flows, validation boundaries, security boundaries, error handling, logging, DTO conventions, service boundaries, future database strategy, frontend state management, audio processing flow, implementation sequencing, and Sprint 003+ testing strategy.

---

## 3. Problem Being Solved

The project has enough discovery to begin technical planning, but the Builder should not yet create production feature code. Without a clear implementation architecture, the next sprint could accidentally mix UI, API, provider integration, validation, secrets handling, and audio workflow decisions in one uncontrolled implementation pass.

Sprint 002 solves this by documenting a simple, secure, extensible MVP architecture before code is written.

---

## 4. Requirements

| ID | Requirement | Priority | Notes |
|---|---|---|---|
| R-001 | Update project planning state to mark Sprint 002 as the active architecture documentation sprint. | Must | Update `planning/STATE.md`. |
| R-002 | Record confirmed implementation decisions from Sprint 001 and Sprint 002. | Must | Update `planning/DECISIONS.md`. |
| R-003 | Update risks for Azure dependency, audio browser constraints, secret handling, API quota/cost, and validation limitations. | Must | Update `planning/RISKS.md`. |
| R-004 | Update unresolved questions for language list, TTS behavior, file size/audio duration limits, UI preferences, deployment assumptions, and provider fallback policy. | Must | Update `planning/QUESTIONS.md`. |
| R-005 | Update file inventory to include Sprint 002 files and docs updated. | Must | Update `planning/FILE_INVENTORY.md`. |
| R-006 | Define MVP frontend project structure. | Must | React browser app only. No authentication UI. |
| R-007 | Define MVP backend project structure. | Must | .NET API backend as proxy between frontend and Azure services. |
| R-008 | Define API layer design with endpoint responsibilities and DTO conventions. | Must | Documentation only; no production controllers yet. |
| R-009 | Define provider abstraction interfaces. | Must | Speech-to-text, text translation, and text-to-speech should be abstracted behind interfaces. |
| R-010 | Define configuration and environment strategy. | Must | No secrets in frontend or repository. Use backend configuration and local secret management. |
| R-011 | Define security boundaries. | Must | Backend owns Azure keys. Frontend sends user input only to backend. No persistent audio storage. |
| R-012 | Define request flow for typed text and push-to-talk audio. | Must | Include validation, provider call, response, and error handling flow. |
| R-013 | Define validation boundaries. | Must | Frontend UX validation plus backend authoritative validation. |
| R-014 | Define error handling and logging strategy. | Must | Avoid logging sensitive text/audio content by default. |
| R-015 | Define service boundaries. | Must | Separate API, application services, provider adapters, configuration, validation, and logging. |
| R-016 | Define future database integration strategy without generating schema. | Must | PostgreSQL later if saved history, accounts, preferences, audit, or analytics are approved. |
| R-017 | Recommend frontend state management. | Must | Keep MVP simple; React hooks/context first, no heavy global state library unless needed later. |
| R-018 | Define audio processing flow. | Must | Push-to-talk recording in browser; upload short audio blob to backend; backend calls Azure Speech. |
| R-019 | Define implementation sequencing for Sprint 003+. | Must | Identify likely next sprints without implementing them. |
| R-020 | Define test strategy for Sprint 003+. | Must | Include unit, integration, contract, frontend, validation, and manual audio tests. |

---

## 5. In Scope

The Builder may work on:

- Documentation updates only.
- `planning/STATE.md`.
- `planning/DECISIONS.md`.
- `planning/RISKS.md`.
- `planning/QUESTIONS.md`.
- `planning/FILE_INVENTORY.md`.
- `docs/ARCHITECTURE.md`.
- `docs/API.md`.
- `docs/VALIDATION.md`.
- New sprint folder: `planning/sprints/002-implementation-architecture/`.
- New sprint files:
  - `requirements.md`
  - `blueprint.md`
  - `acceptance.md`
  - `handoff-prompt.md`

---

## 6. Out Of Scope

The Builder must not work on:

- Production feature implementation.
- React production UI components.
- .NET production controllers/services.
- Azure SDK integration code.
- WebSocket or realtime continuous listening architecture.
- Database schema generation.
- Authentication or account system.
- Infrastructure deployment.
- CI/CD pipeline setup unless already required by project files.
- Secrets, credentials, API keys, or private tokens.
- Changing Sprint 001 scope.
- Inventing business rules not documented in Sprint 001 or this pack.

---

## 7. Assumptions

| ID | Assumption | Confidence | Action If Wrong |
|---|---|---|---|
| A-001 | The MVP should run as a local React frontend and local .NET backend during early development. | High | Record deployment questions if later hosting target changes. |
| A-002 | Azure Speech and Azure Translator are the preferred provider family, but code should be abstracted so providers can change later. | High | Update provider abstraction docs if another provider is selected. |
| A-003 | Audio should be short push-to-talk clips rather than continuous streams. | High | Do not design WebSockets unless explicitly approved later. |
| A-004 | The app does not need accounts or saved history for MVP. | High | Record database/auth implications if account features are requested later. |
| A-005 | The backend is responsible for all provider authentication and secret handling. | High | Stop if any requirement suggests frontend keys. |
| A-006 | PostgreSQL is not needed until persistence is approved. | High | Document future integration only; do not create schema. |

---

## 8. Constraints

- No production feature implementation yet.
- No realtime websocket architecture.
- No database schema generation yet.
- No authentication yet.
- No infrastructure deployment yet.
- No secrets or credentials.
- Keep MVP architecture simple and extensible.
- Follow security-first principles.
- Follow SOLID principles where practical.
- Backend proxy only; no provider API keys in frontend code.
- No persistent audio storage for MVP.
- Do not let Builder invent business rules.
- Builder must perform a dry run first and wait for approval before applying changes.

---

## 9. Dependencies

| Dependency | Type | Status | Notes |
|---|---|---|---|
| Sprint 001 discovery outcomes | Decision | Available | Confirmed in user request. |
| Azure Speech | External API | Recommended / not implemented | Used for speech-to-text and possible TTS. |
| Azure Translator | External API | Recommended / not implemented | Used for text translation. |
| React | Tech stack | Confirmed | Frontend app. |
| .NET | Tech stack | Confirmed | Backend API proxy. |
| PostgreSQL | Database | Later if needed | Do not design schema in Sprint 002. |

---

## 10. Files To Read First

The Builder must read these before doing work:

1. `AGENTS.md`
2. `README.md`
3. `planning/STATE.md`
4. `planning/INTAKE.md`
5. `planning/DOMAIN.md`
6. `planning/DECISIONS.md`
7. `planning/RISKS.md`
8. `planning/QUESTIONS.md`
9. `planning/FILE_INVENTORY.md`
10. `docs/ARCHITECTURE.md`
11. `docs/API.md`
12. `docs/VALIDATION.md`
13. `planning/sprints/001-discovery-architecture/requirements.md`
14. `planning/sprints/001-discovery-architecture/blueprint.md`
15. `planning/sprints/001-discovery-architecture/acceptance.md`
16. `planning/sprints/001-discovery-architecture/handoff-prompt.md`

If a listed file is missing, the Builder must report it during dry run and avoid inventing missing content.

---

## 11. Files To Create Or Modify

| Path | Action | Purpose |
|---|---|---|
| `planning/STATE.md` | Modify | Mark Sprint 002 active and describe next step. |
| `planning/DECISIONS.md` | Modify | Record durable implementation architecture decisions. |
| `planning/RISKS.md` | Modify | Record architecture and provider risks. |
| `planning/QUESTIONS.md` | Modify | Record unresolved implementation questions. |
| `planning/FILE_INVENTORY.md` | Modify | Track Sprint 002 files and documentation updates. |
| `docs/ARCHITECTURE.md` | Modify | Define implementation architecture and request flows. |
| `docs/API.md` | Modify | Define planned API endpoints, DTOs, errors, and integration boundaries. |
| `docs/VALIDATION.md` | Modify | Define validation boundaries, constraints, and test strategy. |
| `planning/sprints/002-implementation-architecture/requirements.md` | Create | Sprint requirements. |
| `planning/sprints/002-implementation-architecture/blueprint.md` | Create | Builder-facing documentation approach. |
| `planning/sprints/002-implementation-architecture/acceptance.md` | Create | Sprint done criteria. |
| `planning/sprints/002-implementation-architecture/handoff-prompt.md` | Create | Builder dry-run prompt. |

---

## 12. Blueprint

### Step 1: Dry Run Before Changes

The Builder must first inspect the project folder and produce a dry run summary. The dry run must include:

- files read
- current project state
- Sprint 001 outcomes found
- files expected to change
- proposed documentation updates
- risks or conflicts
- whether it is safe to apply the pack

The Builder must not apply changes until approved.

### Step 2: Update Planning State Files

Update the planning files as documentation-only project state:

- `planning/STATE.md`: Sprint 002 active, documentation-only, no production code.
- `planning/DECISIONS.md`: Record confirmed tech stack and architectural decisions.
- `planning/RISKS.md`: Add provider, security, browser audio, validation, quota/cost, and future persistence risks.
- `planning/QUESTIONS.md`: Add unresolved questions that should not block architecture documentation.
- `planning/FILE_INVENTORY.md`: Add Sprint 002 files and docs.

### Step 3: Update `docs/ARCHITECTURE.md`

Document the MVP architecture in a way future Builder sprints can follow.

Required sections:

- Application overview.
- Current phase.
- System boundaries.
- Frontend structure recommendation.
- Backend structure recommendation.
- Service boundaries.
- Provider abstraction strategy.
- Configuration/environment strategy.
- Security boundaries.
- Request flow: typed text translation.
- Request flow: push-to-talk audio translation.
- Optional TTS playback flow.
- Frontend state management recommendation.
- Audio processing flow.
- Future PostgreSQL integration strategy.
- Implementation sequencing.
- Operational notes.
- Explicit non-goals.

Recommended frontend structure:

```text
frontend/
  src/
    app/
    components/
      translation/
      audio/
      layout/
      feedback/
    hooks/
    services/
    types/
    utils/
    validation/
```

Recommended backend structure:

```text
backend/
  src/
    Api/
      Controllers/
      Middleware/
      Contracts/
    Application/
      Translation/
      Speech/
      TextToSpeech/
      Validation/
    Infrastructure/
      Providers/
        Azure/
      Configuration/
      Logging/
    Domain/
      Models/
      Errors/
    Shared/
```

These folders are recommendations for future implementation. Sprint 002 should document them, not necessarily create production source folders unless the existing project conventions already require empty placeholder folders.

### Step 4: Update `docs/API.md`

Document planned API contracts without implementing them.

Suggested MVP endpoints:

| Method | Path | Purpose |
|---|---|---|
| `POST` | `/api/translate/text` | Translate typed text. |
| `POST` | `/api/translate/audio` | Accept a short push-to-talk audio recording, transcribe it, then translate it. |
| `POST` | `/api/speech/tts` | Optional: return playable translated speech audio or a provider reference depending on future decision. |
| `GET` | `/api/languages` | Return supported language list from config/provider mapping. |
| `GET` | `/api/health` | Basic backend health check. |

DTO conventions:

- DTOs should be explicit and versionable.
- Use request/response types separate from provider SDK models.
- Do not expose provider SDK response objects directly to the frontend.
- Use consistent error envelope for API errors.
- Include correlation/request ID in error responses where practical.
- Avoid returning sensitive provider details.

Suggested DTO names for future implementation:

- `TranslateTextRequest`
- `TranslateTextResponse`
- `TranslateAudioRequest` or multipart contract notes
- `TranslateAudioResponse`
- `TextToSpeechRequest`
- `TextToSpeechResponse`
- `LanguageOptionDto`
- `ApiErrorResponse`

### Step 5: Update `docs/VALIDATION.md`

Document validation boundaries and acceptance checks.

Frontend validation should improve UX but not be trusted for security.
Backend validation is authoritative.

Validation topics to document:

- Required source input.
- Required target language.
- Optional source language auto-detect policy.
- Max typed text length placeholder.
- Max audio duration placeholder.
- Accepted audio MIME types placeholder.
- Empty/whitespace text handling.
- Unsupported language handling.
- Provider timeout handling.
- Provider unavailable handling.
- No persistent audio storage.
- No logging raw audio or sensitive text by default.

### Step 6: Create Sprint 002 Folder Files

Create:

- `planning/sprints/002-implementation-architecture/requirements.md`
- `planning/sprints/002-implementation-architecture/blueprint.md`
- `planning/sprints/002-implementation-architecture/acceptance.md`
- `planning/sprints/002-implementation-architecture/handoff-prompt.md`

These files should mirror this Architect Pack but be concise and usable from the project folder alone.

---

## 13. Data Flow / Logic Flow

### Typed Text Translation Flow

```text
User enters text + selects target language
-> Frontend validates basic form completeness
-> Frontend sends POST /api/translate/text to backend
-> Backend validates request authoritatively
-> Backend calls translation application service
-> Translation service calls ITextTranslationProvider
-> Azure Translator adapter performs provider request
-> Provider response mapped to internal result
-> Backend returns TranslateTextResponse
-> Frontend displays translated text
-> Optional: user requests TTS playback
```

### Push-To-Talk Audio Translation Flow

```text
User presses and holds / taps record
-> Browser requests microphone permission
-> Frontend records short audio blob
-> User stops recording
-> Frontend validates audio presence and rough duration/size
-> Frontend uploads audio blob + target language to backend
-> Backend validates file size, MIME type, target language, and request metadata
-> Backend calls speech application service
-> Speech service calls ISpeechToTextProvider
-> Azure Speech adapter transcribes audio
-> Backend sends transcript to translation service
-> Translation service calls ITextTranslationProvider
-> Backend returns transcript + translated text
-> Frontend displays original transcript and translated text
-> Optional: frontend requests TTS playback
```

### Optional TTS Flow

```text
User requests playback of translated text
-> Frontend sends translated text + language/voice preference to backend
-> Backend validates request
-> Backend calls ITextToSpeechProvider
-> Azure Speech TTS adapter returns audio result
-> Backend streams or returns playable audio response
-> Frontend plays audio
```

---

## 14. UI / UX Notes

Sprint 002 does not implement UI, but architecture should support the following MVP interface later:

- Text input area.
- Target language selector.
- Optional source language selector or auto-detect mode.
- Translate button for typed input.
- Push-to-talk recording control.
- Recording state indicator.
- Transcription display for audio input.
- Translated output display.
- Optional TTS playback button.
- Loading state while translating/transcribing.
- Clear error messages for microphone denial, invalid input, unsupported language, provider failure, and timeout.

Frontend state recommendation:

- Use React local state and custom hooks for MVP.
- Use context only for app-level configuration or shared language lists if needed.
- Avoid Redux/Zustand unless state complexity grows.
- Keep API client functions separate from UI components.
- Keep validation helpers separate from components.

---

## 15. API / Integration Notes

The backend should act as a proxy and orchestration layer, not a thin provider pass-through.

Provider abstractions to document for future implementation:

```text
ISpeechToTextProvider
- TranscribeAsync(audio, sourceLanguage?, cancellationToken)

ITextTranslationProvider
- TranslateAsync(text, targetLanguage, sourceLanguage?, cancellationToken)

ITextToSpeechProvider
- SynthesizeAsync(text, language, voice?, cancellationToken)

ILanguageProvider or ILanguageCatalogService
- GetSupportedLanguagesAsync(cancellationToken)
```

Application services should depend on these abstractions, not directly on Azure SDK classes.

Azure adapters should live in infrastructure/provider-specific folders.

Backend API should:

- own all provider credentials
- validate all incoming requests
- map request DTOs into application models
- call application services
- map application results into response DTOs
- return consistent error responses
- avoid leaking provider implementation details

---

## 16. Validation Plan

| Check | Method | Expected Result |
|---|---|---|
| Dry run performed | Builder summary before changes | Builder lists files, scope, assumptions, and proposed changes. |
| Documentation-only scope preserved | Review changed files | No production feature code added. |
| Sprint folder created | File inspection | `planning/sprints/002-implementation-architecture/` contains four required files. |
| Planning files updated | File inspection | State, decisions, risks, questions, and inventory reflect Sprint 002. |
| Architecture docs updated | File inspection | Architecture includes frontend/backend structures, flows, provider abstractions, config, security, sequencing. |
| API docs updated | File inspection | API docs include planned endpoints, DTO conventions, error strategy, and provider boundaries. |
| Validation docs updated | File inspection | Validation docs include frontend/backend boundaries, audio/text constraints, no-storage rule, and test strategy. |
| Security constraints preserved | Review docs | No secrets, no frontend API keys, no persistent audio storage. |
| Out-of-scope items avoided | Review diff | No WebSocket realtime design, no auth, no database schema, no deployment implementation. |

---

## 17. Acceptance Criteria

Sprint is complete when:

- [ ] Builder performed a dry run first.
- [ ] Builder summarized understanding and waited for approval before applying changes.
- [ ] `planning/STATE.md` is updated for Sprint 002.
- [ ] `planning/DECISIONS.md` includes confirmed implementation architecture decisions.
- [ ] `planning/RISKS.md` includes relevant architecture/security/provider risks.
- [ ] `planning/QUESTIONS.md` includes unresolved non-blocking architecture questions.
- [ ] `planning/FILE_INVENTORY.md` includes Sprint 002 file updates.
- [ ] `docs/ARCHITECTURE.md` defines MVP implementation architecture.
- [ ] `docs/API.md` defines planned API layer, DTO conventions, errors, and provider integration boundaries.
- [ ] `docs/VALIDATION.md` defines validation boundaries and future test strategy.
- [ ] `planning/sprints/002-implementation-architecture/requirements.md` exists and is complete.
- [ ] `planning/sprints/002-implementation-architecture/blueprint.md` exists and is complete.
- [ ] `planning/sprints/002-implementation-architecture/acceptance.md` exists and is complete.
- [ ] `planning/sprints/002-implementation-architecture/handoff-prompt.md` exists and is complete.
- [ ] No production feature code is implemented.
- [ ] No secrets, credentials, private tokens, or unsafe sensitive data are added.
- [ ] Sprint 001 scope is not changed.
- [ ] The resulting docs are enough for a future Builder to start Sprint 003 without relying on chat history.

---

## 18. Risks

| Risk | Impact | Likelihood | Mitigation | Owner |
|---|---|---|---|---|
| Azure provider assumptions may change. | Medium | Medium | Use provider abstractions and keep Azure details isolated. | Architect / Builder |
| Browser microphone behavior varies by browser and permissions. | Medium | Medium | Document permission, MIME type, duration, and fallback/error behavior. | Builder |
| Audio payloads can become too large. | Medium | Medium | Define backend limits before implementation. | Architect / Builder |
| Raw text/audio may contain sensitive information. | High | Medium | Do not persist audio; avoid logging raw content by default. | Builder |
| Provider costs and quotas may affect MVP behavior. | Medium | Medium | Add config and error strategy; record quota questions. | Architect |
| Overbuilding architecture could slow MVP. | Medium | Medium | Keep docs simple; avoid websockets, auth, database schema, and deployment. | Architect / Builder |
| DTOs may drift from provider models if not documented. | Medium | Low | Define DTO conventions and mapping boundaries. | Builder |

---

## 19. Open Questions

| Question | Needed From | Blocking? | Notes |
|---|---|---|---|
| What exact languages should be included in the MVP language selector? | Product owner | No | Can start with provider-supported language list strategy. |
| Should source language be manual, auto-detected, or both? | Product owner | No | Architecture should allow optional source language. |
| What is the maximum typed text length for MVP? | Architect / Product owner | No | Placeholder limit should be documented, not invented as final. |
| What is the maximum push-to-talk recording duration? | Architect / Product owner | No | Needed before implementation validation. |
| Which browser audio MIME types should be accepted? | Builder / Technical reviewer | No | Depends on target browsers and .NET/Azure handling. |
| Should TTS be in Sprint 003 or later? | Product owner | No | Optional MVP output. |
| Will the app need deployment target details soon? | Product owner | No | Deployment is out of scope for Sprint 002. |
| Should translation history ever be saved? | Product owner | No | Would trigger database and privacy design later. |

---

## 20. State Updates Required

At the end of the sprint, update:

- `planning/STATE.md`
- `planning/DECISIONS.md` if decisions were made or confirmed
- `planning/RISKS.md` if risks changed
- `planning/QUESTIONS.md` if questions were opened or answered
- `planning/FILE_INVENTORY.md` for all created/modified files
- active sprint acceptance status

Suggested `planning/STATE.md` status after applying this pack:

```markdown
# Project State

## Current Phase
Implementation architecture documentation.

## Active Sprint
Sprint 002 - Implementation Architecture.

## Current Status
Sprint 001 discovery outcomes have been accepted. Sprint 002 is documentation-only and defines the MVP implementation architecture before production feature work begins.

## Next Step
Builder must perform dry run, summarize planned documentation updates, and wait for approval before applying changes.

## Blockers
No hard blockers for Sprint 002. Several implementation details remain open questions for Sprint 003+.

## Last Updated
2026-05-28
```

---

## 21. Builder Dry Run Instructions

Before implementation, the Builder must produce a dry run summary.

The dry run must include:

- files read
- current project state understood
- Sprint 001 outcomes found
- Sprint 002 understanding
- what is in scope
- what is out of scope
- files to create or modify
- proposed documentation approach
- commands to run, if any
- assumptions
- risks or ambiguities
- whether implementation is safe to start

Do not apply changes until the dry run has been reviewed and approved.

---

## 22. Builder Handoff Prompt

Copy this prompt into the Builder chat.

```markdown
You are the Builder for My Translation App.

You are working on Sprint 002: Implementation Architecture.

Follow the 120x Architect / Builder methodology.

Do not implement production features yet.

Your task is to create and update implementation architecture documentation only.

Read these files first:

1. AGENTS.md
2. README.md
3. planning/STATE.md
4. planning/INTAKE.md
5. planning/DOMAIN.md
6. planning/DECISIONS.md
7. planning/RISKS.md
8. planning/QUESTIONS.md
9. planning/FILE_INVENTORY.md
10. docs/ARCHITECTURE.md
11. docs/API.md
12. docs/VALIDATION.md
13. planning/sprints/001-discovery-architecture/requirements.md
14. planning/sprints/001-discovery-architecture/blueprint.md
15. planning/sprints/001-discovery-architecture/acceptance.md
16. planning/sprints/001-discovery-architecture/handoff-prompt.md
17. architect-pack-002-implementation-architecture.md, if present

Confirmed decisions:

- Frontend: React
- Backend: .NET
- Database: PostgreSQL later if needed
- Platform: browser-first web app
- Translation model: Azure Speech + Azure Translator recommended
- Workflow: push-to-talk translation, not realtime continuous listening
- Inputs: typed text + live microphone recording
- Outputs: translated text + optional TTS playback
- Accounts: none for MVP
- Storage: no persistent audio storage
- Architecture: backend proxy only; no frontend API keys

Your instructions:

1. Do not write implementation code immediately.
2. First perform a dry run.
3. Summarize what this sprint is doing.
4. Summarize what is explicitly out of scope.
5. List files you expect to create or modify.
6. List assumptions, risks, and ambiguities.
7. List validation checks you plan to run.
8. Stop and wait for approval before applying changes.

After approval, create or update:

- planning/STATE.md
- planning/DECISIONS.md
- planning/RISKS.md
- planning/QUESTIONS.md
- planning/FILE_INVENTORY.md
- docs/ARCHITECTURE.md
- docs/API.md
- docs/VALIDATION.md
- planning/sprints/002-implementation-architecture/requirements.md
- planning/sprints/002-implementation-architecture/blueprint.md
- planning/sprints/002-implementation-architecture/acceptance.md
- planning/sprints/002-implementation-architecture/handoff-prompt.md

Architecture documentation must define:

- frontend project structure
- backend project structure
- API layer design
- provider abstraction interfaces
- configuration/environment strategy
- security boundaries
- request flow
- validation boundaries
- error handling strategy
- logging approach
- DTO conventions
- service boundaries
- future database integration strategy
- frontend state management recommendation
- audio processing flow
- implementation sequencing
- test strategy for Sprint 003+

Constraints:

- No production feature implementation yet.
- No realtime websocket architecture.
- No database schema generation yet.
- No authentication yet.
- No infrastructure deployment yet.
- No secrets or credentials.
- Keep the MVP architecture simple and extensible.
- Follow security-first and SOLID principles.
- Avoid inventing business rules.
- Avoid changing Sprint 001 scope.

Completion report must include:

- files created
- files modified
- commands run
- validation performed
- acceptance criteria status
- decisions added
- risks added or updated
- questions added or resolved
- known limitations
- recommended next sprint
```

---

## 23. Completion Report Template

When finished, the Builder should report:

```markdown
# Sprint 002 Completion Report

## Summary

## Files Created

## Files Modified

## Commands Run

## Tests / Validation

## Acceptance Criteria Status

## Decisions Added

## Risks Added Or Updated

## Questions Added Or Resolved

## Known Limitations

## Recommended Next Sprint
```

---

## 24. Recommended Sprint 003 Direction

Recommended next sprint after Sprint 002 is accepted:

```text
Sprint 003 - Backend API Skeleton and Provider Interfaces
```

Likely scope:

- Create .NET backend skeleton.
- Add DTO classes/contracts.
- Add provider interfaces only.
- Add mock provider implementations for local testing.
- Add validation structure.
- Add no real Azure credentials.
- Add no frontend production UI beyond minimal integration stubs unless specifically scoped.

Do not start Sprint 003 until Sprint 002 is applied, reviewed, and accepted.
