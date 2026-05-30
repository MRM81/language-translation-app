# Sprint 002 Requirements: Implementation Architecture

**Project:** My Translation App
**Sprint:** 002 — Implementation Architecture
**Date:** 2026-05-28

---

## Goal

Define the implementation architecture for the MVP translation app without writing production application code. Create documentation that a future Builder can follow for Sprint 003 and beyond without relying on chat history.

---

## Requirements

| ID | Requirement | Priority | Notes |
|---|---|---|---|
| R-001 | Update planning/STATE.md to mark Sprint 001 complete and Sprint 002 active. | Must | |
| R-002 | Add confirmed implementation decisions to planning/DECISIONS.md (D-019+). | Must | |
| R-003 | Add implementation risks to planning/RISKS.md (R-011+). | Must | Provider, browser audio MIME compatibility, security, cost, DTO drift. |
| R-004 | Add unresolved implementation questions to planning/QUESTIONS.md (Q-008+). | Must | Language auto-detect, limits, MIME types, TTS sprint, deployment target. |
| R-005 | Update planning/FILE_INVENTORY.md with Sprint 002 files and updated docs. | Must | |
| R-006 | Define MVP frontend project structure. | Must | React browser app only. No authentication UI. |
| R-007 | Define MVP backend project structure. | Must | .NET API backend with layered folders (Api, Application, Infrastructure, Domain, Shared). |
| R-008 | Define API layer design with endpoint responsibilities and DTO conventions. | Must | Documentation only. No production controllers. No /api/health endpoint. |
| R-009 | Define provider abstraction interfaces. | Must | Interfaces belong to Application layer, not Infrastructure. |
| R-010 | Define configuration and environment variable strategy. | Must | No secrets in frontend or repository. Backend-only credentials. |
| R-011 | Define security boundaries. | Must | Backend owns provider keys. No persistent audio. No logging raw audio, source text, or translated text. |
| R-012 | Define request flows for typed text and push-to-talk audio translation. | Must | Include validation, provider call, response, and error flow. |
| R-013 | Define validation boundaries. | Must | Frontend UX guard plus backend authoritative validation. |
| R-014 | Define error handling and logging strategy. | Must | No raw audio, source text, or translated text in logs. Correlation IDs and timing metrics allowed. |
| R-015 | Define service boundaries. | Must | Api, Application, Infrastructure, Domain, Shared layers. |
| R-016 | Define future database integration strategy without generating schema. | Must | PostgreSQL plugs into Infrastructure layer only if persistence is ever approved. |
| R-017 | Define frontend state management. | Must | Three tiers: local UI state in components, session state in React Context, API calls via service layer. |
| R-018 | Define audio processing flow. | Must | Push-to-talk blob upload. Backend validates MIME type, size, duration before processing. |
| R-019 | Define implementation sequencing for Sprint 003+. | Must | Next sprints outlined without implementing them. |
| R-020 | Define test strategy for Sprint 003+. | Must | Unit, integration, contract, frontend, validation, manual audio tests. |

---

## In Scope

Documentation updates only:

- planning/STATE.md
- planning/DECISIONS.md
- planning/RISKS.md
- planning/QUESTIONS.md
- planning/FILE_INVENTORY.md
- docs/ARCHITECTURE.md
- docs/API.md
- docs/VALIDATION.md
- planning/sprints/002-implementation-architecture/ (four sprint files)

---

## Out Of Scope

- Production React or .NET application code
- Azure SDK integration code
- WebSocket or real-time continuous listening architecture
- Database schema or migrations
- Authentication or user accounts
- Infrastructure deployment or CI/CD setup
- Secrets, credentials, or API keys in any file
- Changing Sprint 001 scope
- Inventing business rules not documented in Sprint 001 or this Architect Pack

---

## Constraints

- Documentation only — no production feature code.
- Keep architecture simple and extensible.
- Follow security-first and SOLID principles.
- Do not invent business rules.
- Builder must perform a dry run and wait for explicit approval before applying changes.
