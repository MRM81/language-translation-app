# Sprint 005 Requirements — Azure Provider Integration

**Project:** My Translation App
**Sprint:** 005
**Date:** 2026-05-28
**Status:** Complete

---

## Sprint Goal

Implement Azure Translator and Azure Speech-to-Text provider adapters in the backend Infrastructure layer, wire provider selection through safe configuration, and document local setup using .NET User Secrets or environment variables without committing secrets.

---

## Requirements

| ID | Requirement | Priority | Status |
|---|---|---|---|
| R-005-001 | Add Azure text translation provider | Must | Done |
| R-005-002 | Add Azure speech-to-text provider | Must | Done |
| R-005-003 | Preserve all existing public API contracts | Must | Done |
| R-005-004 | Preserve provider abstraction boundaries | Must | Done |
| R-005-005 | Add configuration-based provider selection | Must | Done |
| R-005-006 | Do not commit Azure keys/secrets | Must | Done |
| R-005-007 | Add safe error handling for provider failures | Must | Done |
| R-005-008 | Add tests covering provider selection and config validation | Should | Done |
| R-005-009 | Update backend documentation | Must | Done |
| R-005-010 | Keep Sprint 004 frontend unchanged | Must | Done |

---

## In Scope

- Backend Infrastructure provider implementations.
- Backend configuration options and validation.
- Dependency injection registration in Program.cs.
- Backend README/setup documentation.
- New backend tests.
- Planning and docs updates.

---

## Out Of Scope

- React frontend feature changes.
- Push-to-talk / browser microphone recording.
- Text-to-speech endpoint implementation.
- Authentication or authorization.
- Database schema, migrations, or persistence.
- WebSockets, streaming, or live transcription.
- Deployment, CI/CD, hosting, or production infrastructure.
- Creating real Azure resources.
- Committing real Azure credentials.
