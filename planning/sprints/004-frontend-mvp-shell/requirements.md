# Sprint 004 — Frontend MVP Shell: Requirements

**Project:** My Translation App
**Sprint:** 004
**Sprint Name:** Frontend MVP Shell
**Date:** 2026-05-28
**Status:** In Progress

---

## Sprint Goal

Create a React frontend MVP shell that allows a user to select languages, enter text or upload an audio file, submit translation requests to the Sprint 003 backend, and view mock translation results, transcripts, validation errors, loading states, and correlation IDs.

---

## Problem Being Solved

The project has a working backend skeleton but no user interface. Sprint 004 creates the first browser-facing experience so the core translation workflow can be manually tested end-to-end using deterministic mock backend responses.

This sprint validates:

- backend/frontend contract compatibility
- language selector workflow
- text translation request flow
- audio upload request flow
- error display behaviour
- loading and empty states
- correlation ID visibility for debugging

---

## Requirements

| ID | Requirement | Priority | Notes |
|---|---|---|---|
| R-004-001 | Create a React frontend app under `src/frontend/`. | Must | Vite + React + TypeScript. |
| R-004-002 | Build a clean MVP UI shell for translation workflows. | Must | Text and audio translation accessible from the UI. |
| R-004-003 | Load available languages from `GET /api/languages`. | Must | Do not hardcode primary UI options unless backend fetch fails gracefully. |
| R-004-004 | Implement text translation form. | Must | Calls `POST /api/translate/text`. |
| R-004-005 | Implement audio file upload translation form. | Must | Calls `POST /api/translate/audio` as multipart/form-data. File input only — no MediaRecorder. |
| R-004-006 | Display translated text results. | Must | Include provider and correlation ID when returned. |
| R-004-007 | Display audio transcription and translated result. | Must | Show `transcribedText` and `translatedText` separately. |
| R-004-008 | Display backend validation errors clearly. | Must | Use `errorCode`, `message`, `details`, and `correlationId` from error response. |
| R-004-009 | Include loading, empty, and failure states. | Must | User should always understand what is happening. |
| R-004-010 | Keep frontend configuration environment-safe. | Must | No secrets. Backend base URL in `.env` or `.env.local`. |
| R-004-011 | Document manual validation steps in README. | Must | No frontend test framework required in Sprint 004. |
| R-004-012 | Update project documentation and planning files. | Must | STATE, DECISIONS, RISKS, QUESTIONS, FILE_INVENTORY, ARCHITECTURE, VALIDATION. |

---

## In Scope

- React frontend project setup at `src/frontend/`
- UI components: app shell, language selectors, text translation form, audio upload form, result panel, error panel, loading state
- API client functions for backend calls
- Frontend configuration for backend base URL
- Manual validation documentation
- Docs and planning file updates

---

## Out of Scope

- Azure Translator or Azure Speech integration
- Real external translation APIs
- Authentication or authorisation
- Database or persistence
- User accounts or session history
- WebSocket or real-time streaming
- Text-to-speech playback
- Browser microphone recording or MediaRecorder
- Push-to-talk button
- Mobile native app work
- CI/CD or deployment
- Advanced design system or component library
- Complex global state management libraries (Redux, Zustand)
- Frontend test framework (Vitest, Jest — deferred to a future sprint)
- Backend endpoint contract changes unless a genuine mismatch is discovered and documented

---

## Assumptions

| ID | Assumption | Confidence | Action If Wrong |
|---|---|---|---|
| A-004-001 | Sprint 003 backend endpoints are implemented and all 30 tests pass. | High | Confirmed before this sprint started. |
| A-004-002 | Node.js and npm are available on the development machine. | High | Document prerequisite in README. |
| A-004-003 | React + Vite + TypeScript is the correct frontend tooling choice. | High | No prior frontend convention exists. Decision D-035. |
| A-004-004 | Backend base URL is `http://localhost:5074` per launchSettings.json. | High | Confirm from README and launchSettings before starting. |
| A-004-005 | Audio input in Sprint 004 is file upload only — no MediaRecorder. | High | Push-to-talk deferred to Sprint 006. |
| A-004-006 | `sourceLanguage` submitted as empty string represents auto-detection. | High | Backend accepts null/empty for sourceLanguage on both endpoints. |

---

## Constraints

- No secrets, credentials, private tokens, or production connection strings.
- Do not add Azure SDKs or real provider integrations.
- Do not change backend API contracts without Architect approval.
- Keep UI MVP-focused and simple.
- Prefer simple React component state over global state libraries.
- Use provider-neutral frontend types matching backend DTOs.
- Do not log source text, translated text, transcripts, audio filenames, or raw API payloads.
- Show correlation IDs in result and error panels.
- Respect backend validation limits: text max 5,000 characters, audio max 10 MB.
- Error response field is `errorCode` (not `code`). Error detail fields are `field` and `message` only.

---

## Dependencies

| Dependency | Type | Status |
|---|---|---|
| Sprint 003 backend API | Backend | Available — mock providers, 30 tests passing |
| `GET /api/languages` | API | Available |
| `POST /api/translate/text` | API | Available |
| `POST /api/translate/audio` | API | Available |
| Node.js / npm | Tooling | Required on development machine |
| Vite + React + TypeScript | Package | Installed as part of Sprint 004 |
