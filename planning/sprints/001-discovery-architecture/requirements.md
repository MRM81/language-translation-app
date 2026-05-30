# Sprint 001 Requirements: Discovery And Architecture

**Project:** My Translation App
**Sprint:** 001 — Discovery Architecture
**Date:** 2026-05-28

---

## Goal

Establish the project context, MVP scope, user workflow, system architecture, API boundaries, security posture, validation rules, risks, open questions, and Builder handoff documentation. Prepare the project for a later implementation sprint without writing production application code.

---

## Requirements

| ID | Requirement | Priority | Notes |
|---|---|---|---|
| R-001 | Define MVP as browser-based text and push-to-talk translation. | Must | Avoid real-time continuous listening in MVP. |
| R-002 | Document the primary user workflow from input to translated output. | Must | Workflow should support both typed text and recorded speech paths. |
| R-003 | Define system architecture at a high level. | Must | React frontend, .NET backend, provider abstraction, optional PostgreSQL later. |
| R-004 | Document API boundaries between frontend, backend, and translation providers. | Must | No frontend secrets. Backend proxies all external API calls. |
| R-005 | Define privacy and security requirements. | Must | Do not permanently store audio or translation content in MVP. Do not log user content by default. |
| R-006 | Define validation rules for input, language selection, audio length, and errors. | Must | Validation must prevent unsafe or excessive requests at both frontend and backend. |
| R-007 | Record risks, assumptions, and open questions. | Must | Do not hide uncertainty in chat history. Record in planning files. |
| R-008 | Create sprint files under planning/sprints/001-discovery-architecture/. | Must | requirements.md, blueprint.md, acceptance.md, handoff-prompt.md. |
| R-009 | Update planning state files. | Must | STATE.md, DOMAIN.md, DECISIONS.md, RISKS.md, QUESTIONS.md, FILE_INVENTORY.md. |
| R-010 | Create or update docs/ARCHITECTURE.md, docs/API.md, and docs/VALIDATION.md. | Must | Documentation only. No production code. |

---

## Inputs

- Architect Pack: architect-pack-001-discovery.md
- Project brief: planning/INTAKE.md
- Agent rules: AGENTS.md
- Project start context: project-start.md

---

## In Scope

Documentation only:

- planning/STATE.md
- planning/DOMAIN.md
- planning/DECISIONS.md
- planning/RISKS.md
- planning/QUESTIONS.md
- planning/FILE_INVENTORY.md
- planning/sprints/001-discovery-architecture/ (all four sprint files)
- docs/ARCHITECTURE.md
- docs/API.md
- docs/VALIDATION.md

---

## Out Of Scope

- Production React application code
- Production .NET backend code
- Database migrations or schema
- Authentication or user accounts
- Billing or subscriptions
- Continuous real-time listening or WebSocket streaming
- Native mobile apps or browser extensions
- Document or PDF translation
- Offline translation models
- Enterprise admin features
- Deployment automation
- API provider account setup
- Storing real user audio or translation data
- Secrets, credentials, or API keys in any file

---

## Constraints

- Builder must not write production application code in this sprint.
- All content must be sourced from the Architect Pack. Do not invent business rules.
- Record unknowns in planning/QUESTIONS.md rather than guessing.
- Record decisions in planning/DECISIONS.md.
- The project folder is the source of truth.
