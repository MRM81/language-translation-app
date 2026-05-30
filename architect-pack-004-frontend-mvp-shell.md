# Architect Pack: Sprint 004 - Frontend MVP Shell

## Pack Metadata

| Field | Value |
|---|---|
| Project Name | My Translation App |
| Client / Owner | Mark McLachlan |
| Project Slug | my-translation-app |
| Sprint Number | 004 |
| Sprint Name | Frontend MVP Shell |
| Created Date | 2026-05-28 |
| Architect | ChatGPT |
| Builder Target | Claude Code / Codex / Cursor / Other |
| Status | Ready For Builder |

---

## 1. Project Context

My Translation App is a web-based translation application that accepts text or audio input and translates it into a chosen target language.

Sprint 001 established discovery and project architecture.
Sprint 002 established implementation architecture documentation.
Sprint 003 created the backend API skeleton in .NET 8 with mock translation providers, structured validation, correlation IDs, and the following working endpoints:

- `POST /api/translate/text`
- `POST /api/translate/audio`
- `GET /api/languages`

Sprint 004 should build the first usable frontend shell against the mock backend. The goal is to prove the user workflow end-to-end before adding Azure or real translation providers.

The frontend must remain MVP-focused and must not overbuild.

---

## 2. Sprint Goal

Create a React frontend MVP shell that allows a user to select languages, enter text or upload an audio file, submit translation requests to the Sprint 003 backend, and view mock translation results, transcripts, validation errors, loading states, and correlation IDs.

---

## 3. Problem Being Solved

The project currently has a working backend skeleton but no user interface. Sprint 004 creates the first browser-facing experience so the core translation workflow can be manually tested end-to-end using deterministic mock backend responses.

This sprint validates:

- backend/frontend contract compatibility
- language selector workflow
- text translation request flow
- audio upload request flow
- error display behavior
- loading and empty states
- correlation ID visibility for debugging

---

## 4. Requirements

| ID | Requirement | Priority | Notes |
|---|---|---|---|
| R-004-001 | Create a React frontend app under the existing project structure. | Must | Use practical MVP setup; do not restructure backend. |
| R-004-002 | Build a clean MVP UI shell for translation workflows. | Must | Text and audio translation should be accessible from the UI. |
| R-004-003 | Load available languages from `GET /api/languages`. | Must | Do not hardcode primary UI options unless backend fetch fails gracefully. |
| R-004-004 | Implement text translation form. | Must | Calls `POST /api/translate/text`. |
| R-004-005 | Implement audio upload translation form. | Must | Calls `POST /api/translate/audio` as multipart/form-data. |
| R-004-006 | Display translated text results. | Must | Include provider and correlation ID when returned. |
| R-004-007 | Display audio transcription and translated result. | Must | Audio response should show transcript and translation separately. |
| R-004-008 | Display backend validation errors clearly. | Must | Use structured error fields where available. |
| R-004-009 | Include loading, empty, and failure states. | Must | User should understand what is happening. |
| R-004-010 | Keep frontend configuration environment-safe. | Must | No secrets. Backend base URL must be configurable. |
| R-004-011 | Add basic frontend tests or documented validation checks. | Should | Use existing or practical test setup. |
| R-004-012 | Update project documentation and planning files. | Must | STATE, DECISIONS, RISKS, QUESTIONS, FILE_INVENTORY, API/ARCHITECTURE/VALIDATION if relevant. |

---

## 5. In Scope

The Builder may work on:

- React frontend project setup
- frontend source files under `src/frontend/` or the existing agreed frontend location
- UI components for:
  - app shell
  - language selectors
  - text translation form
  - audio upload form
  - result panels
  - error messages
  - loading states
- API client/helper functions for backend calls
- frontend configuration for backend base URL
- basic tests or manual validation notes
- docs and planning updates required by the sprint

---

## 6. Out Of Scope

The Builder must not work on:

- Azure Translator integration
- Azure Speech integration
- real external translation APIs
- authentication or authorization
- database or persistence
- user accounts
- payment or subscriptions
- deployment hosting
- CI/CD
- WebSocket or realtime streaming
- text-to-speech playback
- mobile native app work
- major backend refactors
- backend endpoint contract changes unless a genuine mismatch is found and documented
- advanced design system implementation
- complex state management libraries unless already present and justified

---

## 7. Assumptions

| ID | Assumption | Confidence | Action If Wrong |
|---|---|---|---|
| A-004-001 | Backend Sprint 003 endpoints are implemented and tests pass. | High | If not, stop and report mismatch. |
| A-004-002 | The frontend can run locally while the backend runs locally. | High | Document required dev commands. |
| A-004-003 | React is the selected frontend framework. | High | If repo indicates a different selected framework, stop and report. |
| A-004-004 | Backend base URL can be stored in a non-secret frontend environment variable. | High | Use local default only if no env config exists. |
| A-004-005 | MVP styling should be clean and simple, not a full design system. | High | Avoid overbuilding. |
| A-004-006 | Audio upload should use file input first, not browser microphone recording. | Medium | Record microphone capture as future sprint unless already decided. |

---

## 8. Constraints

- No secrets, credentials, private tokens, API keys, or production connection strings.
- Do not add Azure SDKs or real provider integrations.
- Do not change backend API contracts without Architect approval.
- Keep the UI MVP-focused.
- Prefer simple React state over global state libraries.
- Use provider-neutral frontend models matching backend DTOs.
- Do not log raw source text, transcripts, or translations to console in a way that could leak user content.
- Show correlation IDs in error/result panels for debugging.
- Respect backend validation limits:
  - text max 5,000 characters
  - audio max 10 MB
  - allowed audio MIME types per backend docs
- The project folder remains the source of truth.

---

## 9. Dependencies

| Dependency | Type | Status | Notes |
|---|---|---|---|
| Sprint 003 backend API | Backend | Available | Must be treated as the current contract. |
| `GET /api/languages` | API | Available | Used for selectors. |
| `POST /api/translate/text` | API | Available | Used by text form. |
| `POST /api/translate/audio` | API | Available | Used by audio upload form. |
| React tooling | Package / framework | TBD in repo | Builder should inspect current package setup first. |
| Frontend test tooling | Package | TBD in repo | Add only minimal practical setup if not present. |

---

## 10. Files To Read First

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
11. `planning/sprints/004-frontend-mvp-shell/requirements.md`
12. `planning/sprints/004-frontend-mvp-shell/blueprint.md`
13. `planning/sprints/004-frontend-mvp-shell/acceptance.md`
14. `planning/sprints/004-frontend-mvp-shell/handoff-prompt.md`
15. Existing frontend files, if any
16. Existing package files, if any

---

## 11. Files To Create Or Modify

| Path | Action | Purpose |
|---|---|---|
| `planning/STATE.md` | Modify | Mark Sprint 003 completed/accepted and Sprint 004 active. |
| `planning/DECISIONS.md` | Modify if needed | Record frontend framework/tooling decisions. |
| `planning/RISKS.md` | Modify if needed | Add frontend/backend contract and browser upload risks. |
| `planning/QUESTIONS.md` | Modify if needed | Resolve or add open frontend questions. |
| `planning/FILE_INVENTORY.md` | Modify | Add Sprint 004 files. |
| `docs/ARCHITECTURE.md` | Modify | Add frontend shell architecture notes. |
| `docs/API.md` | Modify if needed | Confirm frontend API usage matches backend contract. |
| `docs/VALIDATION.md` | Modify | Add frontend validation/display behavior. |
| `planning/sprints/004-frontend-mvp-shell/requirements.md` | Create | Sprint requirements. |
| `planning/sprints/004-frontend-mvp-shell/blueprint.md` | Create | Builder implementation blueprint. |
| `planning/sprints/004-frontend-mvp-shell/acceptance.md` | Create | Acceptance criteria. |
| `planning/sprints/004-frontend-mvp-shell/handoff-prompt.md` | Create | Builder handoff prompt. |
| `src/frontend/` | Create or modify | React frontend implementation. |
| `src/frontend/package.json` | Create or modify | Frontend scripts and dependencies if needed. |
| `src/frontend/src/` | Create or modify | React components, API client, types, styles. |
| `src/frontend/README.md` | Create or modify | Local run instructions. |
| `tests/frontend/` or frontend colocated tests | Create if practical | Basic frontend tests or validation notes. |

The exact frontend file paths may vary if the repo already contains a frontend convention. Builder must inspect before creating a duplicate structure.

---

## 12. Blueprint

### Step 1: Inspect Existing Repo

- Read required project files.
- Confirm Sprint 003 backend structure and documented endpoints.
- Inspect whether a frontend already exists.
- Inspect package manager conventions, if any.
- Do not implement before completing dry run.

### Step 2: Create Sprint 004 Planning Files

Create:

```text
planning/sprints/004-frontend-mvp-shell/
├── requirements.md
├── blueprint.md
├── acceptance.md
└── handoff-prompt.md
```

These should reflect this Architect Pack.

### Step 3: Establish Frontend App

If no frontend exists, create a practical React app under:

```text
src/frontend/
```

Preferred simple structure:

```text
src/frontend/
├── README.md
├── package.json
├── index.html
├── vite.config.*
├── src/
│   ├── App.*
│   ├── main.*
│   ├── api/
│   │   └── translationApi.*
│   ├── components/
│   │   ├── LanguageSelect.*
│   │   ├── TextTranslationForm.*
│   │   ├── AudioTranslationForm.*
│   │   ├── ResultPanel.*
│   │   ├── ErrorPanel.*
│   │   └── LoadingButton.*
│   ├── types/
│   │   └── api.*
│   └── styles/
│       └── app.css
```

Use TypeScript if practical and consistent with tooling. If the Builder chooses JavaScript instead, it must record the reason.

### Step 4: Build API Client

Create a small frontend API client responsible for:

- reading backend base URL from config
- fetching languages
- posting text translation JSON
- posting audio translation multipart form data
- parsing structured API errors
- preserving/displaying correlation IDs

Expected default local backend base URL may be:

```text
http://localhost:5000
```

or whatever the backend docs specify. Builder must confirm from repo/config and document actual value.

### Step 5: Build UI Workflow

The UI should include:

- app title and short description
- text translation area
- audio upload area
- source language selector
- target language selector
- result display
- error display
- loading indicators
- empty state
- basic instructions for running backend + frontend locally

Minimum user flow:

```text
Open app
-> language list loads
-> select target language
-> enter text OR choose audio file
-> submit
-> loading state appears
-> mock backend response appears
-> correlation ID is visible
```

### Step 6: Frontend Validation

Implement client-side validation only as a helpful first pass:

- text required for text translation
- text max 5,000 characters
- target language required
- audio file required for audio translation
- audio file max 10 MB
- display warning for unsupported MIME type where browser provides type

Backend remains source of truth. Frontend must still display backend validation errors.

### Step 7: Result/Error Display

For successful text translation, show:

- translated text
- source language
- target language
- provider
- correlation ID

For successful audio translation, show:

- transcribed text
- translated text
- source language
- target language
- provider
- correlation ID

For errors, show:

- user-friendly message
- field errors if present
- backend error code if present
- correlation ID if present

### Step 8: Testing / Validation

Run available frontend checks, such as:

- install/build
- typecheck if TypeScript
- lint if configured
- unit tests if added
- manual browser workflow against backend

Do not add a heavy test framework if it substantially slows MVP delivery. At minimum, document manual validation steps in `src/frontend/README.md` and sprint acceptance.

### Step 9: Documentation Updates

Update docs and planning files:

- `planning/STATE.md`
- `planning/DECISIONS.md`
- `planning/RISKS.md`
- `planning/QUESTIONS.md`
- `planning/FILE_INVENTORY.md`
- `docs/ARCHITECTURE.md`
- `docs/VALIDATION.md`
- `docs/API.md` if frontend usage reveals clarification needs

---

## 13. Data Flow / Logic Flow

```text
Frontend App Loads
-> Translation API Client calls GET /api/languages
-> Language options populate selectors
-> User enters text or selects audio file
-> Frontend performs basic validation
-> Frontend sends request to backend
-> Backend validates and returns mock result or structured error
-> Frontend displays result/error and correlation ID
```

Text flow:

```text
Text Input
-> Client Validation
-> POST /api/translate/text
-> Mock Translation Response
-> Result Panel
```

Audio flow:

```text
Audio File Input
-> Client Validation
-> multipart/form-data POST /api/translate/audio
-> Mock Transcript + Mock Translation Response
-> Result Panel
```

---

## 14. UI / UX Notes

The UI should be simple, clear, and usable.

Required states:

- initial empty state
- language loading state
- language load failure state
- form validation error state
- backend validation error state
- request loading state
- success state
- network/server failure state

Suggested layout:

```text
Header
  App name
  Short description

Main
  Shared language controls
  Text translation card
  Audio translation card
  Result/error area

Footer or small debug area
  Backend base URL
  Last correlation ID
```

Design requirements:

- clean spacing
- readable labels
- obvious submit buttons
- disabled buttons while loading
- accessible form labels
- no clutter
- mobile-friendly enough for basic use

Do not build a large design system yet.

---

## 15. API / Integration Notes

### GET `/api/languages`

Expected response:

```json
{
  "languages": [
    {
      "code": "en",
      "name": "English"
    }
  ]
}
```

Frontend should use `code` as the submitted language value and display `name`.

### POST `/api/translate/text`

Expected request:

```json
{
  "sourceText": "Hello",
  "sourceLanguage": "auto",
  "targetLanguage": "es"
}
```

Expected success response:

```json
{
  "translatedText": "[mock-es] Hello",
  "sourceLanguage": "auto",
  "targetLanguage": "es",
  "provider": "mock",
  "correlationId": "..."
}
```

### POST `/api/translate/audio`

Expected multipart fields:

- `audio`: file
- `sourceLanguage`: optional or `auto`
- `targetLanguage`: required

Expected success response:

```json
{
  "transcribedText": "[mock transcript]",
  "translatedText": "[mock-es] [mock transcript]",
  "sourceLanguage": "auto",
  "targetLanguage": "es",
  "provider": "mock",
  "correlationId": "..."
}
```

### Error Response

Expected structured error shape may be:

```json
{
  "code": "VALIDATION_ERROR",
  "message": "Validation failed.",
  "correlationId": "...",
  "details": [
    {
      "field": "sourceText",
      "code": "TEXT_REQUIRED",
      "message": "Source text is required."
    }
  ]
}
```

Builder must confirm exact DTO shape from `docs/API.md` and backend code before implementing frontend parsing.

---

## 16. Validation Plan

| Check | Method | Expected Result |
|---|---|---|
| Frontend installs successfully | package manager install command | Dependencies install without secrets. |
| Frontend builds successfully | build command | Build completes. |
| Language list loads | Manual browser test against backend | Selectors populate from `GET /api/languages`. |
| Text translation works | Manual browser test | UI displays `[mock-{targetLanguage}]` result. |
| Audio upload works | Manual browser test | UI displays mock transcript and mock translation. |
| Empty text validation works | UI test/manual test | User sees helpful validation message. |
| Missing audio validation works | UI test/manual test | User sees helpful validation message. |
| Oversized audio validation works | UI test/manual test if practical | User sees helpful validation message before upload. |
| Backend validation errors display | Manual/API test | Error panel shows message/details/correlation ID. |
| No raw content logging | Code review | No unnecessary console logging of text/audio/transcripts. |
| Docs updated | File review | Planning and docs reflect frontend shell. |

---

## 17. Acceptance Criteria

Sprint is complete when:

- [ ] Sprint 004 planning folder exists with `requirements.md`, `blueprint.md`, `acceptance.md`, and `handoff-prompt.md`.
- [ ] A React frontend app exists in the agreed frontend location.
- [ ] Frontend can be run locally using documented commands.
- [ ] Frontend loads language options from `GET /api/languages`.
- [ ] Text translation form calls `POST /api/translate/text`.
- [ ] Audio upload form calls `POST /api/translate/audio`.
- [ ] Text translation success result displays translated text, provider, language codes, and correlation ID.
- [ ] Audio translation success result displays transcript, translated text, provider, language codes, and correlation ID.
- [ ] Frontend shows loading states during API calls.
- [ ] Frontend shows empty/initial states.
- [ ] Frontend shows client-side validation for required text/audio/target language.
- [ ] Frontend displays backend structured validation errors.
- [ ] Frontend handles language loading failure gracefully.
- [ ] Backend base URL is configurable without secrets.
- [ ] No secrets or credentials are added.
- [ ] No Azure SDKs or real provider integrations are added.
- [ ] No backend contract changes are made unless documented and approved.
- [ ] Documentation is updated where needed.
- [ ] `planning/STATE.md` is updated.
- [ ] New risks are added to `planning/RISKS.md`.
- [ ] Open questions are added to `planning/QUESTIONS.md`.
- [ ] `planning/FILE_INVENTORY.md` includes Sprint 004 frontend files.
- [ ] Build/test/manual validation results are reported in completion report.

---

## 18. Risks

| Risk | Impact | Likelihood | Mitigation | Owner |
|---|---|---|---|---|
| Frontend/backend DTO mismatch | Medium | Medium | Builder must read backend code and docs before implementing API client. | Builder |
| CORS blocks local frontend calls | Medium | Medium | Configure backend CORS only for local dev if not already present, and document. | Builder |
| MIME type inconsistency in browsers | Medium | Medium | Treat frontend MIME validation as advisory; backend remains source of truth. | Builder |
| Overbuilding UI/design system | Medium | Medium | Keep UI shell simple and task-focused. | Builder |
| Raw user content logged in browser console | High | Low | Avoid console logging request bodies, transcripts, source text, and audio data. | Builder |
| Backend base URL confusion | Medium | Medium | Use documented config and include clear README instructions. | Builder |
| Audio upload local testing varies by browser | Low | Medium | Validate with a common audio file and document limitations. | Builder |

---

## 19. Open Questions

| Question | Needed From | Blocking? | Notes |
|---|---|---|---|
| Q-016: Should browser microphone recording be added in a later sprint? | Architect / Project Owner | No | Sprint 004 uses file upload only. |
| Q-017: What final frontend styling direction is preferred? | Project Owner | No | Sprint 004 uses clean MVP styling. |
| Q-018: Should frontend use TypeScript if no frontend exists yet? | Architect / Builder | No | Recommended if practical; Builder should inspect repo conventions. |
| Q-019: Should CORS be configured in Sprint 004 if local frontend calls fail? | Builder / Architect | No | Allowed only as minimal local-dev support if required. |

---

## 20. State Updates Required

At the end of the sprint, update:

- `planning/STATE.md`
- `planning/DECISIONS.md` if frontend tooling decisions are made
- `planning/RISKS.md` if risks changed
- `planning/QUESTIONS.md` if questions were opened or answered
- `planning/FILE_INVENTORY.md` for frontend files
- `docs/ARCHITECTURE.md` with frontend architecture notes
- `docs/VALIDATION.md` with frontend validation behavior
- `docs/API.md` if frontend usage clarifies API details
- Sprint 004 acceptance status

---

## 21. Builder Dry Run Instructions

Before implementation, the Builder must produce a dry run summary.

The dry run must include:

- files read
- current frontend state
- current backend endpoint contract understanding
- planned frontend structure
- planned file changes
- commands to run
- assumptions
- risks or ambiguities
- validation plan
- whether implementation is safe to start

Do not implement until the dry run has been reviewed and approved.

---

## 22. Builder Handoff Prompt

Copy this prompt into the Builder chat.

```markdown
You are the Builder for My Translation App.

You are working on Sprint 004: Frontend MVP Shell.

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
11. planning/sprints/004-frontend-mvp-shell/requirements.md
12. planning/sprints/004-frontend-mvp-shell/blueprint.md
13. planning/sprints/004-frontend-mvp-shell/acceptance.md
14. planning/sprints/004-frontend-mvp-shell/handoff-prompt.md

Your instructions:

1. Do not write implementation code immediately.
2. First perform a dry run against the CURRENT repository state.
3. Summarize:
   - what this sprint builds
   - what is explicitly out of scope
   - current frontend state
   - backend endpoint contract understanding
   - files you expect to create or modify
   - commands you will run
   - assumptions
   - risks or ambiguities
   - validation plan
4. Stop and wait for approval before implementation.

Sprint goal:

Create a React frontend MVP shell that connects to the Sprint 003 mock backend endpoints:

- GET /api/languages
- POST /api/translate/text
- POST /api/translate/audio

The UI must support:

- language selection from backend
- text translation form
- audio file upload translation form
- result display
- transcript display for audio
- loading states
- empty states
- client-side validation
- backend validation error display
- correlation ID display

Implementation constraints:

- Use React.
- Prefer TypeScript if practical and consistent with repo conventions.
- Keep UI simple and MVP-focused.
- Do not add Azure SDKs.
- Do not add real translation providers.
- Do not add auth.
- Do not add database or persistence.
- Do not add WebSockets or streaming.
- Do not add text-to-speech playback.
- Do not change backend contracts unless a genuine mismatch is discovered and reported.
- Do not store secrets or credentials.
- Do not log raw source text, transcripts, translations, or audio payloads unnecessarily.

Expected frontend behavior:

1. App loads.
2. Frontend fetches languages from GET /api/languages.
3. User selects source language or auto.
4. User selects target language.
5. User enters text or uploads audio.
6. Frontend performs basic validation.
7. Frontend calls backend.
8. UI displays mock translation result or structured error.
9. UI displays correlation ID where available.

Validation expectations:

- Run install/build/typecheck/test commands where available.
- Manually validate text translation against backend.
- Manually validate audio upload against backend.
- Confirm no secrets were added.
- Confirm docs and planning files were updated.

After dry run, stop and wait for approval.
```

---

## 23. Completion Report Template

When finished, the Builder should report:

```markdown
# Sprint 004 Completion Report

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

## 24. Suggested Sprint 004 File Contents

The Builder should create the sprint planning files by splitting this pack into the four sprint files.

### `planning/sprints/004-frontend-mvp-shell/requirements.md`

Include:

- Sprint goal
- problem being solved
- requirements table
- in scope
- out of scope
- assumptions
- constraints
- dependencies

### `planning/sprints/004-frontend-mvp-shell/blueprint.md`

Include:

- files to read
- implementation steps
- frontend structure
- API client plan
- UI workflow
- validation plan
- documentation update plan

### `planning/sprints/004-frontend-mvp-shell/acceptance.md`

Include:

- all acceptance criteria from section 17
- test/validation commands
- manual validation checklist

### `planning/sprints/004-frontend-mvp-shell/handoff-prompt.md`

Include:

- the Builder handoff prompt from section 22
