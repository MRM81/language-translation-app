# Sprint 003 — Requirements

**Project:** My Translation App
**Sprint:** 003 — Backend API Skeleton
**Date:** 2026-05-28
**Status:** Active

---

## Sprint Goal

Create the .NET 8 backend API skeleton for the MVP translation app using mock providers only. The skeleton establishes project structure, layer boundaries, DTOs, provider abstraction interfaces, validation, correlation ID handling, and mock endpoint behaviour so future sprints can add real Azure provider integrations without restructuring the backend.

---

## Requirements

| ID | Requirement | Priority | Notes |
|---|---|---|---|
| R-003-001 | Create a .NET 8 backend skeleton aligned to Sprint 002 architecture. | Must | No production provider integration. |
| R-003-002 | Create translation-facing API endpoints using mock providers. | Must | POST /api/translate/text and POST /api/translate/audio. GET /api/languages included. |
| R-003-003 | Create DTOs for text translation request/response, audio translation response, language list, and API errors. | Must | Names follow Sprint 002 API conventions. Provider-neutral. |
| R-003-004 | Create provider abstraction interfaces in the Application layer with CancellationToken support. | Must | No Azure SDK references in Application. ISpeechToTextProvider, ITextTranslationProvider, ITextToSpeechProvider (placeholder), ILanguageCatalogService. |
| R-003-005 | Create mock provider implementations in Infrastructure. | Must | Deterministic, clearly prefixed mock outputs. |
| R-003-006 | Add backend validation for text length, required language fields, audio presence, audio size, and accepted MIME types. | Must | Backend validation is authoritative. Normalise MIME type checking (strip parameters, allow audio/webm variants). |
| R-003-007 | Add correlation ID middleware. Reuse X-Correlation-ID header from request if present; otherwise generate new GUID. | Must | Return correlation ID in all success and error responses. Do not log raw audio, source text, translated text, or transcribed text. |
| R-003-008 | Add error code constants class to prevent frontend contract drift. | Must | Centralised string constants in Application layer. |
| R-003-009 | Add Kestrel and form upload size limits at server level before logical validation. | Must | Prevents large uploads reaching memory before validation. |
| R-003-010 | Add CancellationToken to all provider interface methods, service methods, and controller actions. | Must | Prevents interface churn when real SDKs are added. |
| R-003-011 | Add GET /api/languages endpoint backed by StaticLanguageCatalogService. Document in docs/API.md. | Must | Returns list of supported language codes and display names. |
| R-003-012 | Add basic automated tests for validation, service behaviour, and endpoint contracts. | Should | xUnit. Includes correlation ID presence in responses. |
| R-003-013 | Document audio duration validation limitation. | Must | Duration cannot be reliably measured from IFormFile without audio parsing library. Enforce size and MIME; defer exact duration enforcement. |
| R-003-014 | Update planning and docs after implementation. | Must | STATE, DECISIONS, QUESTIONS, RISKS, FILE_INVENTORY, API.md, VALIDATION.md, ARCHITECTURE.md. |
| R-003-015 | Verify architecture dependency direction. | Must | Application has zero Infrastructure references. Infrastructure depends on Application. Api is composition root only. |

---

## In Scope

- .NET 8 backend solution and project skeleton
- API / Application / Infrastructure / Tests project separation
- POST /api/translate/text endpoint (mock provider)
- POST /api/translate/audio endpoint (mock providers)
- GET /api/languages endpoint (static catalog)
- Provider interfaces in Application layer
- Mock providers in Infrastructure layer
- Provider-neutral DTOs
- Structured error DTO with error code constants
- Correlation ID middleware (reuse or generate)
- Kestrel and form upload size limits
- Validation for required fields, text length, MIME types, audio size
- Audio duration documented as deferred (size enforced instead)
- CancellationToken throughout interfaces, services, and controllers
- Safe logging (no raw user content)
- xUnit test project with validation, service, and controller tests
- Sprint 003 planning files
- Local development startup instructions

---

## Out of Scope

- React frontend implementation
- Azure Speech SDK integration
- Azure Translator SDK or API integration
- Real external API calls
- API keys, secrets, credentials, or .env values
- PostgreSQL schema, migrations, or database persistence
- Authentication or user accounts
- WebSocket or realtime streaming
- Infrastructure deployment or CI/CD
- Persistent audio storage
- TTS endpoint implementation (interface defined as future placeholder only)
- /api/health endpoint

---

## Validation Rules Applied

| Rule | Value |
|---|---|
| Max text characters | 5,000 |
| Max audio seconds | 60 (deferred enforcement — size limit applied instead) |
| Max audio bytes | 10,485,760 (10 MB) |
| Accepted audio MIME types | audio/webm, audio/webm;codecs=opus, audio/mp4, audio/mpeg, audio/wav |
| MIME matching strategy | Normalise by stripping parameters before comparison |

---

## Decisions Applied

D-020, D-021, D-022, D-024, D-025, D-027 through D-033.

See planning/DECISIONS.md for full decision records.

---

## Notes

- Source language accepts explicit BCP-47 codes or the string "auto" in Sprint 003. Full language code validation against the catalog is deferred.
- ITextToSpeechProvider interface is defined but no TTS endpoint is wired in this sprint.
- Audio duration validation is documented as a known limitation. It will be enforced in a future sprint when an audio parsing dependency is introduced.
