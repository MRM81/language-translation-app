# Sprint 002 Builder Handoff Prompt

**Project:** My Translation App
**Sprint:** 002 — Implementation Architecture
**Date:** 2026-05-28

---

Copy this prompt into the Builder session to start Sprint 002.

---

You are the Builder for My Translation App.

You are working on Sprint 002: Implementation Architecture.

Follow the 120x Architect / Builder methodology.

Do not write production code. This sprint is documentation-only.

Read these files first:

1. AGENTS.md
2. planning/STATE.md
3. planning/DECISIONS.md
4. planning/RISKS.md
5. planning/QUESTIONS.md
6. planning/FILE_INVENTORY.md
7. docs/ARCHITECTURE.md
8. docs/API.md
9. docs/VALIDATION.md
10. planning/sprints/001-discovery-architecture/requirements.md
11. planning/sprints/001-discovery-architecture/blueprint.md
12. planning/sprints/001-discovery-architecture/acceptance.md
13. planning/sprints/001-discovery-architecture/handoff-prompt.md
14. architect-pack-002-implementation-architecture.md

Confirmed decisions:

- Frontend: React
- Backend: .NET
- Database: PostgreSQL later if needed
- Platform: browser-first web app
- Translation: Azure Speech + Azure Translator recommended
- Workflow: push-to-talk translation, not realtime continuous listening
- Inputs: typed text + live microphone recording
- Outputs: translated text + optional TTS playback
- Accounts: none for MVP
- Storage: no persistent audio storage
- Architecture: backend proxy only; no frontend API keys

Your instructions:

1. Do not write production code.
2. First perform a dry run.
3. Summarise what this sprint is building.
4. Summarise what is explicitly out of scope.
5. List files you expect to create or modify.
6. List assumptions, risks, and ambiguities.
7. List validation checks you plan to run.
8. Wait for explicit approval before applying changes.

After approval, create or update:

- planning/STATE.md — Sprint 001 complete, Sprint 002 active
- planning/DECISIONS.md — Append D-019 through D-026
- planning/RISKS.md — Append R-011 through R-015
- planning/QUESTIONS.md — Append Q-008 through Q-014
- planning/FILE_INVENTORY.md — Add Sprint 002 files and updated doc entries
- docs/ARCHITECTURE.md — Full implementation architecture
- docs/API.md — Endpoints, DTOs, versioning guidance, provider boundaries. No /api/health.
- docs/VALIDATION.md — Validation boundaries, MIME types, logging constraints, test strategy
- planning/sprints/002-implementation-architecture/requirements.md
- planning/sprints/002-implementation-architecture/blueprint.md
- planning/sprints/002-implementation-architecture/acceptance.md
- planning/sprints/002-implementation-architecture/handoff-prompt.md

Architecture documentation must define:

- Frontend project structure (components, hooks, services, types, utils, validation)
- Backend project structure (Api, Application, Infrastructure, Domain, Shared)
- Service boundaries
- Provider abstraction interfaces — Application layer only, not Infrastructure
- Configuration and environment variable strategy
- Security boundaries
- Request flows (text translation, audio translation, optional TTS)
- Frontend state management (local component state, React Context for session state, service layer for API calls)
- Audio processing flow (MediaRecorder, blob upload, MIME type handling)
- Future PostgreSQL integration strategy (Infrastructure layer only)
- Implementation sequencing for Sprint 003+
- Test strategy for Sprint 003+

Required corrections from project owner:

1. Mark Sprint 001 complete and Sprint 002 active in STATE.md.
2. Do not add /api/health to API documentation.
3. Logging rules: no raw audio, no source text, no translated text may be logged. Correlation IDs and timing metrics are allowed.
4. Provider abstraction interfaces belong to the Application layer, not Infrastructure.
5. Add a browser MediaRecorder MIME type compatibility risk.
6. Frontend state: local UI state in components, session state in React Context, API calls via a lightweight service layer. No Redux/Zustand in MVP.
7. DTO versioning: no URL-based API versioning initially. DTO structure must support future safe additive extension without breaking changes.

Constraints:

- No production code.
- No realtime websocket architecture.
- No database schema generation.
- No authentication.
- No infrastructure deployment.
- No secrets or credentials in any file.
- Keep architecture simple and extensible.
- Follow security-first and SOLID principles.
- Do not invent business rules.
- Do not change Sprint 001 scope.
