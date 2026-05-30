# Sprint 003 — Builder Handoff Prompt

**Project:** My Translation App
**Sprint:** 003 — Backend API Skeleton

---

You are the Builder for My Translation App.

You are working on Sprint 003: Backend API Skeleton.

Follow the 120x Architect / Builder methodology.

The project folder is the source of truth.

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
11. planning/sprints/003-backend-api-skeleton/requirements.md
12. planning/sprints/003-backend-api-skeleton/blueprint.md
13. planning/sprints/003-backend-api-skeleton/acceptance.md
14. architect-pack-003-backend-api-skeleton.md

Your job is to build the backend API skeleton only.

Sprint 003 Goals:

- Create .NET 8 backend solution structure (Api / Application / Infrastructure / Tests)
- Create Application layer: provider interfaces, DTOs, validation, services
- Create Infrastructure layer: mock provider implementations, static language catalog
- Create API layer: controllers, middleware, Program.cs, appsettings
- Create Tests project: validation tests, service tests, controller integration tests
- Create correlation ID middleware (reuse X-Correlation-ID if present; generate GUID if not)
- Create server-level size limits (Kestrel + FormOptions)
- Create centralised error code constants
- Create MIME normalisation logic (strip parameters before comparison)
- Add CancellationToken to all provider interfaces, service methods, and controller actions
- Update planning and docs files after completion

Important Constraints (do not violate):

- No real Azure credentials
- No Azure SDK references in any project
- No real Azure API integration
- No frontend production UI
- No database or PostgreSQL
- No authentication or authorization
- No deployment infrastructure or CI/CD
- No WebSockets or realtime streaming
- No persistent audio storage
- No secrets or API keys in any file
- Application layer has zero Infrastructure references
- Do not invent business rules
- Do not expand Sprint 003 scope

Approved MVP Validation Rules:

- Text limit: 5,000 characters
- Audio duration limit: 60 seconds (duration enforcement deferred — file size limit enforced instead)
- Max audio file size: 10 MB
- Accepted MIME types: audio/webm, audio/webm;codecs=opus, audio/mp4, audio/mpeg, audio/wav
- MIME matching: normalise by stripping parameters before comparison

Required Interfaces (Application layer, CancellationToken on all methods):

- ISpeechToTextProvider
- ITextTranslationProvider
- ITextToSpeechProvider (future placeholder — no endpoint wired)
- ILanguageCatalogService

Required Endpoints:

- POST /api/translate/text
- POST /api/translate/audio
- GET /api/languages

Do NOT add:
- /api/health
- /api/tts (interface exists but no endpoint)
- Auth endpoints
- Database endpoints

Before implementation:

1. Run a dry run first.
2. Summarise what this sprint is building, what is out of scope, files to create/modify, assumptions, and risks.
3. Wait for approval before implementing.

Do not implement until the dry run is reviewed and approved.
