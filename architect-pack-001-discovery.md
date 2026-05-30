# Architect Pack: Sprint 001 - Discovery Architecture

## Pack Metadata

| Field | Value |
|---|---|
| Project Name | My Translation App |
| Client / Owner | Acme Corp |
| Project Slug | my-translation-app |
| Project Type | Web App |
| Sprint Number | 001 |
| Sprint Name | Discovery Architecture |
| Created Date | 2026-05-28 |
| Architect | ChatGPT Architect Layer |
| Builder Target | Claude Code / Codex |
| Status | Ready For Builder Dry Run |

---

## 1. Project Context

My Translation App is a browser-based language translation web app intended to help people communicate when they do not speak the same language. The MVP should prioritise a simple, practical workflow: typed text translation and push-to-talk audio translation.

The project is in the discovery and architecture phase. No production code should be created in this sprint unless explicitly approved later. The goal of Sprint 001 is to establish durable project documentation, scope boundaries, architecture direction, risks, validation rules, and a Builder-ready handoff.

The project folder is:

```text
C:\Users\Mark\Documents\Marks folder\Claude\ai_architecture_system\projects\my-translation-app
```

The project folder is the source of truth, not this chat.

---

## 2. Sprint Goal

Create the initial discovery and architecture documentation for My Translation App. Define the MVP scope, user workflow, system boundaries, security posture, validation rules, risks, open questions, and first Builder handoff. This sprint should prepare the project for a later implementation sprint without writing production application code.

---

## 3. Problem Being Solved

People who speak different languages often struggle to communicate clearly. The app should allow a user to enter text or speak a short phrase, choose a source and target language, and receive a translated result. The MVP should solve the first simple communication use case without overbuilding real-time continuous interpretation, accounts, billing, enterprise administration, or document translation.

---

## 4. Recommended MVP Direction

The recommended MVP is a mobile-friendly browser app with:

- typed text translation
- push-to-talk microphone input
- speech-to-text transcription
- translated text output
- optional translated speech playback
- no user accounts initially
- no permanent storage of translated content
- backend API proxy to protect provider keys
- provider abstraction so translation services can be swapped later

Continuous real-time translation is explicitly out of scope for the MVP.

---

## 5. Requirements

| ID | Requirement | Priority | Notes |
|---|---|---|---|
| R-001 | Define MVP as browser-based text and push-to-talk translation. | Must | Avoid real-time continuous listening in MVP. |
| R-002 | Document the primary user workflow from input to translated output. | Must | Workflow should support typed text and recorded speech. |
| R-003 | Define system architecture at a high level. | Must | React frontend, .NET backend, provider abstraction, optional PostgreSQL later. |
| R-004 | Document API boundaries between frontend, backend, and translation providers. | Must | No frontend secrets. Backend proxies external API calls. |
| R-005 | Define privacy and security requirements. | Must | Do not permanently store audio or translation content in MVP. |
| R-006 | Define validation rules for input, language selection, audio length, and errors. | Must | Validation must prevent unsafe or excessive requests. |
| R-007 | Record risks, assumptions, and open questions. | Must | Do not hide uncertainty in chat history. |
| R-008 | Create sprint files under planning/sprints/001-discovery-architecture/. | Must | requirements.md, blueprint.md, acceptance.md, handoff-prompt.md. |
| R-009 | Update planning state files. | Must | STATE, DOMAIN, DECISIONS, RISKS, QUESTIONS, FILE_INVENTORY. |
| R-010 | Create or update docs/ARCHITECTURE.md, docs/API.md, and docs/VALIDATION.md. | Must | Documentation only; no production code. |

---

## 6. In Scope

The Builder may create or update documentation only:

- `planning/STATE.md`
- `planning/DOMAIN.md`
- `planning/DECISIONS.md`
- `planning/RISKS.md`
- `planning/QUESTIONS.md`
- `planning/FILE_INVENTORY.md`
- `planning/sprints/001-discovery-architecture/requirements.md`
- `planning/sprints/001-discovery-architecture/blueprint.md`
- `planning/sprints/001-discovery-architecture/acceptance.md`
- `planning/sprints/001-discovery-architecture/handoff-prompt.md`
- `docs/ARCHITECTURE.md`
- `docs/API.md`
- `docs/VALIDATION.md`

---

## 7. Out Of Scope

The Builder must not work on:

- production React application code
- production .NET backend code
- database migrations
- authentication
- billing or subscriptions
- user accounts
- continuous real-time listening
- websocket streaming translation
- native mobile apps
- browser extensions
- document or PDF translation
- offline translation models
- enterprise admin features
- deployment automation
- API provider account setup
- storing real user audio or translation data

---

## 8. Assumptions

| ID | Assumption | Confidence | Action If Wrong |
|---|---|---|---|
| A-001 | MVP should be a browser-based web app. | High | Update architecture and platform scope. |
| A-002 | MVP should support typed text and push-to-talk audio input. | High | Adjust workflow and validation docs. |
| A-003 | Full real-time continuous interpretation is not required for MVP. | High | Create a separate future sprint if required. |
| A-004 | No user accounts are needed for MVP. | Medium | Add auth, user model, and storage requirements later. |
| A-005 | Translation history should be session-only in MVP. | Medium | Add persistence and database schema later if required. |
| A-006 | Backend should protect provider API keys. | High | Must not expose secrets in frontend. |
| A-007 | Azure Speech + Azure Translator is the recommended first provider direction. | Medium | Keep provider abstraction to allow OpenAI, DeepL, or Google later. |
| A-008 | PostgreSQL is not needed for Sprint 001 implementation planning unless persistence is added later. | Medium | Revisit if accounts/history/settings become in scope. |

---

## 9. Constraints

- Do not write production code in Sprint 001.
- Do not invent unapproved business rules.
- Do not store secrets, API keys, tokens, or credentials in project files.
- Keep MVP simple and practical.
- Backend must be the boundary for external provider keys.
- Avoid continuous listening or realtime architecture in MVP.
- Record unknowns in `planning/QUESTIONS.md`.
- Record durable product and architecture decisions in `planning/DECISIONS.md`.
- Project folder is the source of truth.
- Builder must dry-run before applying changes.

---

## 10. Dependencies

| Dependency | Type | Status | Notes |
|---|---|---|---|
| React | Frontend | Recommended | Used for browser UI. |
| .NET | Backend | Recommended | Used as API proxy/service layer. |
| PostgreSQL | Database | Deferred | Not required unless persistence is added. |
| Azure Translator | External API | Recommended / TBD | Candidate translation provider. |
| Azure Speech | External API | Recommended / TBD | Candidate speech-to-text/text-to-speech provider. |
| OpenAI APIs | External API | Optional future candidate | Useful if natural conversational translation becomes priority. |
| API credentials | Secret | Missing | Must not be stored in docs or frontend. |

---

## 11. Files To Read First

The Builder must read these before doing work:

1. `AGENTS.md`
2. `project-start.md`
3. `planning/STATE.md`
4. `planning/INTAKE.md`
5. `planning/DOMAIN.md`
6. `planning/FILE_INVENTORY.md`
7. `CLAUDE.md`
8. `CODEX.md`

If any expected files are missing, the Builder should report that during dry run.

---

## 12. Files To Create Or Modify

| Path | Action | Purpose |
|---|---|---|
| `planning/STATE.md` | Modify | Record current phase, active sprint, status, next action. |
| `planning/DOMAIN.md` | Modify | Capture project domain, users, workflow, MVP scope. |
| `planning/DECISIONS.md` | Create/Modify | Record durable MVP and architecture decisions. |
| `planning/RISKS.md` | Create/Modify | Record product, technical, privacy, cost, and API risks. |
| `planning/QUESTIONS.md` | Create/Modify | Record unresolved discovery questions. |
| `planning/FILE_INVENTORY.md` | Modify | Track created/updated sprint and docs files. |
| `planning/sprints/001-discovery-architecture/requirements.md` | Create/Modify | Sprint requirements. |
| `planning/sprints/001-discovery-architecture/blueprint.md` | Create/Modify | Sprint implementation/documentation plan. |
| `planning/sprints/001-discovery-architecture/acceptance.md` | Create/Modify | Sprint acceptance criteria. |
| `planning/sprints/001-discovery-architecture/handoff-prompt.md` | Create/Modify | Builder prompt for this sprint. |
| `docs/ARCHITECTURE.md` | Create/Modify | High-level architecture. |
| `docs/API.md` | Create/Modify | API boundary and contract notes. |
| `docs/VALIDATION.md` | Create/Modify | Validation rules and test expectations. |

---

## 13. Blueprint

### Step 1: Inspect Current Project Files

- Read root operating files and planning files.
- Confirm current scaffold state.
- Identify missing docs or planning files.
- Do not modify anything before dry-run summary.

### Step 2: Update Project State And Domain

- Update `planning/STATE.md` with Sprint 001 active status.
- Update `planning/DOMAIN.md` with business problem, users, MVP workflow, out-of-scope items, and terminology.
- Mark unknowns clearly.

### Step 3: Record Durable Decisions

Add decisions such as:

- MVP is browser-based.
- MVP supports typed text and push-to-talk audio.
- MVP does not include continuous real-time translation.
- MVP does not require accounts.
- Backend protects provider secrets.
- Audio and translations are not permanently stored in MVP.
- Azure Speech + Translator is recommended first provider direction, with abstraction for provider swapping.

### Step 4: Record Risks And Questions

Risks should include:

- API cost escalation.
- translation accuracy limitations.
- speech transcription errors.
- privacy exposure from audio/text.
- overbuilding realtime features too early.
- provider lock-in.
- browser microphone permission issues.
- unclear supported languages.

Questions should include:

- final provider choice
- required supported language list
- maximum audio length
- whether text-to-speech is required for MVP or should be deferred
- whether session history is enough
- hosting preference
- target launch environment

### Step 5: Create Sprint Files

Create or update:

- `requirements.md`
- `blueprint.md`
- `acceptance.md`
- `handoff-prompt.md`

Each file must be clear enough for a Builder to resume work from the folder alone.

### Step 6: Create Technical Docs

Create or update:

- `docs/ARCHITECTURE.md`
- `docs/API.md`
- `docs/VALIDATION.md`

These should remain high-level and implementation-neutral. Do not invent exact endpoint code, package names, or provider credentials.

### Step 7: Update File Inventory

Update `planning/FILE_INVENTORY.md` to include all created or modified files and note their purpose.

---

## 14. Data Flow / Logic Flow

Recommended MVP flow:

```text
Text input path:
User typed text
→ frontend validates text and selected languages
→ frontend sends request to backend translation endpoint
→ backend validates request
→ backend calls translation provider
→ backend normalises response
→ frontend displays translated text
→ optional text-to-speech playback

Audio input path:
User presses record
→ browser captures short audio clip
→ frontend validates duration and file size
→ frontend sends audio to backend speech endpoint
→ backend validates request
→ backend calls speech-to-text provider
→ backend receives transcript
→ backend sends transcript to translation provider
→ backend returns transcript + translated text
→ frontend displays transcript and translation
→ optional text-to-speech playback
```

---

## 15. UI / UX Notes

Initial UX should be simple:

- source language selector
- target language selector
- text input box
- translate button
- microphone record button
- transcript display for audio input
- translated result display
- optional play-audio button
- clear/reset button
- loading state while translating
- error state for permission, provider, validation, and network failures

Mobile-friendly layout is preferred because translation tools are often used while travelling or speaking in person.

---

## 16. API / Integration Notes

Recommended backend API boundaries for future implementation:

| Endpoint Concept | Purpose |
|---|---|
| `POST /api/translate/text` | Translate typed text. |
| `POST /api/translate/audio` | Accept short recorded audio, transcribe, then translate. |
| `GET /api/languages` | Return supported source/target languages. |
| `POST /api/speech/synthesize` | Optional future endpoint for translated text-to-speech. |

API rules:

- Backend must validate request size, language codes, and content presence.
- Backend must not expose provider secrets.
- Provider-specific responses should be normalised before returning to frontend.
- Errors should be structured and safe.
- Do not log full user audio or translated text by default.

---

## 17. Validation Plan

| Check | Method | Expected Result |
|---|---|---|
| File structure | Inspect project folder | Required planning and docs files exist. |
| MVP scope | Review requirements and domain docs | Scope is text + push-to-talk translation only. |
| Out-of-scope protection | Review sprint docs | Realtime, accounts, billing, document translation are excluded. |
| Security posture | Review architecture and validation docs | No frontend secrets, no permanent content storage, backend proxy defined. |
| API boundaries | Review `docs/API.md` | Conceptual endpoints and validation responsibilities are documented. |
| Risks/questions | Review planning files | Known risks and unknowns are visible. |
| Builder handoff | Review handoff prompt | Builder is told to dry-run and wait for approval. |

---

## 18. Acceptance Criteria

Sprint 001 is complete when:

- [ ] `planning/STATE.md` identifies Sprint 001 as active or completed and explains the current project status.
- [ ] `planning/DOMAIN.md` describes the business problem, users, MVP workflow, and out-of-scope features.
- [ ] `planning/DECISIONS.md` records the durable MVP decisions.
- [ ] `planning/RISKS.md` records relevant product, technical, API, privacy, and implementation risks.
- [ ] `planning/QUESTIONS.md` records unresolved questions without blocking the documentation sprint.
- [ ] `planning/FILE_INVENTORY.md` lists created/updated files.
- [ ] Sprint folder `planning/sprints/001-discovery-architecture/` exists.
- [ ] Sprint folder contains `requirements.md`, `blueprint.md`, `acceptance.md`, and `handoff-prompt.md`.
- [ ] `docs/ARCHITECTURE.md` describes the recommended MVP architecture.
- [ ] `docs/API.md` describes conceptual API boundaries.
- [ ] `docs/VALIDATION.md` describes validation rules.
- [ ] No production application code is added.
- [ ] No secrets, credentials, private tokens, or unsafe sensitive data are added.
- [ ] Builder provides a dry-run summary before applying changes.

---

## 19. Risks

| Risk | Impact | Likelihood | Mitigation | Owner |
|---|---|---|---|---|
| Realtime translation is overbuilt too early. | High | Medium | Keep MVP push-to-talk only. | Architect |
| Translation quality varies by language pair. | Medium | High | Validate common language pairs and document limitations. | Builder / Architect |
| Speech-to-text errors reduce trust. | Medium | High | Display transcript before or alongside translation. | Builder |
| API costs escalate with audio usage. | High | Medium | Add duration limits and rate limits in future implementation. | Architect |
| Provider lock-in. | Medium | Medium | Define provider abstraction. | Architect |
| User privacy exposure through audio/text. | High | Medium | Avoid permanent storage and unsafe logging. | Builder |
| Browser microphone permissions cause UX failures. | Medium | Medium | Document permission error states. | Builder |
| Supported languages are undefined. | Medium | High | Record as open question and use provider language list later. | Architect |

---

## 20. Open Questions

| Question | Needed From | Blocking? | Notes |
|---|---|---|---|
| Which provider should be used first: Azure, OpenAI, Google, DeepL, or another? | Project Owner | No | Azure recommended for MVP, but final choice can happen before implementation. |
| What language pairs must be supported at launch? | Project Owner | No | Common initial set may include English, Spanish, French, Mandarin, Arabic, Japanese, Korean. |
| Should translated speech playback be required in MVP or deferred? | Project Owner | No | Recommended as should-have, not blocker. |
| What maximum recording duration should MVP allow? | Architect / Project Owner | No | Suggested future default: 30-60 seconds. |
| Should session translation history be visible during the session? | Project Owner | No | Recommended yes, session-only. |
| Where will the app be hosted? | Project Owner / Builder | No | TBD. |
| Will this be a portfolio/demo app or a production business product? | Project Owner | No | Affects polish, deployment, privacy, and rate limiting. |

---

## 21. State Updates Required

At the end of this sprint, update:

- `planning/STATE.md`
- `planning/DOMAIN.md`
- `planning/DECISIONS.md`
- `planning/RISKS.md`
- `planning/QUESTIONS.md`
- `planning/FILE_INVENTORY.md`
- `planning/sprints/001-discovery-architecture/acceptance.md`

---

## 22. Builder Dry Run Instructions

Before implementation, the Builder must produce a dry-run summary.

The dry run must include:

- files read
- current project state understood
- sprint goal understood
- planned file changes
- commands, if any
- assumptions
- risks or ambiguities
- validation checks
- confirmation that no production code will be written

Do not apply changes until the dry run has been reviewed and approved.

---

## 23. Builder Handoff Prompt

```markdown
# Claude Code / Codex Builder Prompt — Pack 001 Discovery

You are the Builder layer for this project.

Do not write any code until you have read the Architect Pack and received explicit approval.

Project: My Translation App
Project Folder: C:\Users\Mark\Documents\Marks folder\Claude\ai_architecture_system\projects\my-translation-app
Architect Pack: architect-pack-001-discovery.md

## Steps

1. Read the Architect Pack at: C:\Users\Mark\Documents\Marks folder\Claude\ai_architecture_system\projects\my-translation-app\architect-pack-001-discovery.md
2. Read these files in order:
   - AGENTS.md
   - project-start.md
   - planning/STATE.md
   - planning/INTAKE.md
   - planning/DOMAIN.md
3. Run a dry-run. Do not apply any changes yet.
4. Summarise what will change: files, operations, risks, open questions.
5. Wait for explicit approval before applying anything.
6. Apply the pack only after approval is given.
7. Update planning/STATE.md and planning/FILE_INVENTORY.md after applying.

## Rules

- Do not invent business rules or requirements.
- Do not skip the dry-run step.
- Stop and ask when anything is unclear.
- The project folder is the source of truth, not the chat history.
- Work sprint-by-sprint. Do not jump ahead.
```

---

## 24. Completion Report Template

When finished, the Builder should report:

```markdown
# Sprint 001 Completion Report

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
