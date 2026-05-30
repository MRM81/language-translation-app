# Sprint 001 Blueprint: Discovery And Architecture

**Project:** My Translation App
**Sprint:** 001 — Discovery Architecture
**Date:** 2026-05-28

---

## Approach

This sprint is documentation-only. No production application code is written.

---

## Step 1: Inspect Current Project Files

- Read root operating files (AGENTS.md, CLAUDE.md, CODEX.md, project-start.md).
- Read planning files (STATE.md, INTAKE.md, DOMAIN.md, FILE_INVENTORY.md).
- Read existing docs and sprint scaffold files.
- Confirm current state, identify what is TBD placeholder vs. real content.
- Do not modify anything before dry-run summary.

## Step 2: Update Project State And Domain

- Update planning/STATE.md: set Sprint 001 as active, update status and next actions.
- Update planning/DOMAIN.md: business problem, users, MVP workflow (text and audio paths), out-of-scope items, UI notes, terminology, systems and tools.

## Step 3: Record Durable Decisions

Record all relevant durable decisions in planning/DECISIONS.md, including:

- MVP is browser-based.
- MVP supports typed text translation.
- MVP supports push-to-talk audio translation.
- Continuous real-time translation is out of scope for MVP.
- MVP does not require user accounts.
- Translation history is session-only in MVP.
- Backend proxies all provider API calls (frontend never holds secrets).
- Audio and translated text are not permanently stored.
- User content is not logged by default.
- Azure Speech + Translator is the recommended first provider direction.
- Provider abstraction layer is required.
- PostgreSQL is deferred.
- React is the recommended frontend framework.
- .NET is the recommended backend framework.
- Mobile-friendly layout is the design priority.
- Backend normalises provider responses before returning to frontend.
- Errors returned to frontend must be structured and safe.
- Text-to-speech is a should-have, not a blocker.

## Step 4: Record Risks And Questions

Record all known risks in planning/RISKS.md:

- Realtime overbuilding risk.
- Translation accuracy limitations.
- Speech transcription errors.
- API cost escalation.
- Provider lock-in.
- Privacy exposure through audio/text.
- Browser microphone permission failures.
- Undefined supported language list.
- Frontend key exposure if backend proxy not enforced.
- TTS scope creep.

Record all open questions in planning/QUESTIONS.md:

- Provider choice.
- Required language pairs at launch.
- TTS in MVP or deferred.
- Maximum audio recording duration.
- Session history visibility in UI.
- Hosting preference.
- Portfolio/demo vs. production product.

## Step 5: Create Sprint Files

Update all four sprint files with project-specific content:

- requirements.md — Sprint requirements from Architect Pack.
- blueprint.md — This file (7-step documentation plan).
- acceptance.md — Sprint 001 acceptance criteria (unchecked).
- handoff-prompt.md — Builder prompt for resuming from folder alone.

## Step 6: Create Technical Documentation

Update docs with high-level architecture, API, and validation content. No implementation code or provider credentials.

- docs/ARCHITECTURE.md — MVP architecture, components, data flow, integration points.
- docs/API.md — Conceptual API endpoints, request/response shapes, error handling rules.
- docs/VALIDATION.md — Validation rules for text input, audio, language codes, and error cases.

## Step 7: Update File Inventory

Update planning/FILE_INVENTORY.md to reflect all created or updated sprint and docs files with their purpose.

---

## Files To Update

| File | Action |
|---|---|
| planning/STATE.md | Modify |
| planning/DOMAIN.md | Modify |
| planning/DECISIONS.md | Modify |
| planning/RISKS.md | Modify |
| planning/QUESTIONS.md | Modify |
| planning/FILE_INVENTORY.md | Modify |
| planning/sprints/001-discovery-architecture/requirements.md | Modify |
| planning/sprints/001-discovery-architecture/blueprint.md | Modify |
| planning/sprints/001-discovery-architecture/acceptance.md | Modify |
| planning/sprints/001-discovery-architecture/handoff-prompt.md | Modify |
| docs/ARCHITECTURE.md | Modify |
| docs/API.md | Modify |
| docs/VALIDATION.md | Modify |

---

## Notes

- Keep all documentation high-level and implementation-neutral.
- Do not invent exact endpoint code, package names, or provider credentials.
- Record every unknown as a question rather than an assumption.
- This sprint ends when all 14 acceptance criteria in acceptance.md are met.
