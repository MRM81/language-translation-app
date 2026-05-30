# Sprint 002 Blueprint: Implementation Architecture

**Project:** My Translation App
**Sprint:** 002 — Implementation Architecture
**Date:** 2026-05-28

---

## Purpose

This blueprint defines the documentation steps the Builder follows to apply Sprint 002. No production code is written in this sprint.

---

## Steps

### Step 1: Dry Run

Before making any changes:
- Read all required files listed in the handoff prompt.
- Summarise current project state, files to create or modify, assumptions, and risks.
- Wait for explicit approval before applying any changes.

### Step 2: Update Planning State Files

- `planning/STATE.md` — Mark Sprint 001 complete and Sprint 002 active.
- `planning/DECISIONS.md` — Append confirmed implementation architecture decisions (D-019+).
- `planning/RISKS.md` — Append implementation, provider, browser audio MIME, and cost risks (R-011+).
- `planning/QUESTIONS.md` — Append unresolved implementation questions (Q-008+).
- `planning/FILE_INVENTORY.md` — Add Sprint 002 sprint files and updated documentation file entries.

### Step 3: Update docs/ARCHITECTURE.md

Expand to include:
- Current phase note (documentation only, no production code).
- Frontend project structure recommendation (components, hooks, services, types, utils, validation).
- Backend project structure recommendation (Api, Application, Infrastructure, Domain, Shared layers).
- Service boundaries table.
- Provider abstraction interfaces — defined in Application/Interfaces/, not Infrastructure.
- Configuration and environment variable strategy.
- Security boundaries (expanded: no raw audio, source text, or translated text in logs).
- Request flows: typed text translation, push-to-talk audio translation, optional TTS playback.
- Frontend state management — three tiers: local component state, React Context for session state, service layer for API calls.
- Audio processing flow including MediaRecorder and MIME type handling.
- Future PostgreSQL integration strategy — Infrastructure layer only.
- Implementation sequencing table for Sprint 003+.
- Non-goals list.

### Step 4: Update docs/API.md

Expand to include:
- Updated endpoint list: POST /api/translate/text, POST /api/translate/audio, GET /api/languages, POST /api/speech/synthesize.
- No /api/health endpoint.
- Optional sourceLanguage field (auto-detect policy pending Q-008).
- DTO naming conventions and DTO names table.
- DTO versioning guidance — no URL versioning initially; DTO structure supports future safe additive extension.
- Correlation ID field in all error responses.
- UNSUPPORTED_AUDIO_FORMAT and PROVIDER_TIMEOUT added to error code list.
- Provider boundary rules.

### Step 5: Update docs/VALIDATION.md

Expand to include:
- Source language auto-detect policy.
- Audio MIME type validation rules (placeholder pending Q-011).
- Provider timeout and unavailability handling.
- Strengthened logging constraints — no raw audio, source text, or translated text logged.
- Test strategy for Sprint 003+.
- Test cases V-015 through V-018 (MIME type, timeout, log content, correlation ID).

### Step 6: Create Sprint 002 Folder Files

Create the following under `planning/sprints/002-implementation-architecture/`:
- `requirements.md` — Sprint 002 requirements table and scope boundaries.
- `blueprint.md` — This file.
- `acceptance.md` — Done criteria checklist.
- `handoff-prompt.md` — Self-contained Builder prompt for this sprint.

---

## Validation Checks

After applying all changes, confirm:

1. All 12 required files exist and contain non-empty content.
2. No production code (.cs, .tsx, .ts, SDK imports) has been added anywhere.
3. No secrets, credentials, or API keys appear in any file.
4. No WebSocket, real-time streaming, or authentication architecture has been introduced.
5. No database schema has been created.
6. Sprint 001 documentation content is not removed or contradicted.
7. All new decisions have unique IDs continuing from D-018.
8. All new risks have unique IDs continuing from R-010.
9. All new questions have unique IDs continuing from Q-007.
10. FILE_INVENTORY.md lists all new and modified files.
11. STATE.md correctly shows Sprint 001 complete and Sprint 002 active.
12. /api/health does not appear in API documentation.
