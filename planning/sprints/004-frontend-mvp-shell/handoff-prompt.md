# Sprint 004 — Frontend MVP Shell: Builder Handoff Prompt

You are the Builder for My Translation App.

You are working on Sprint 004: Frontend MVP Shell.

Follow the 120x Architect / Builder methodology.

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
11. planning/sprints/004-frontend-mvp-shell/requirements.md
12. planning/sprints/004-frontend-mvp-shell/blueprint.md
13. planning/sprints/004-frontend-mvp-shell/acceptance.md
14. planning/sprints/004-frontend-mvp-shell/handoff-prompt.md

Your instructions:

1. Do not write implementation code immediately.
2. First perform a dry run against the CURRENT repository state.
3. Summarize:
   - what this sprint builds
   - what is explicitly out of scope
   - current frontend state
   - backend endpoint contract understanding
   - files you expect to create or modify
   - commands you will run
   - assumptions
   - risks or ambiguities
   - validation plan
4. Stop and wait for approval before implementation.

Sprint goal:

Create a React frontend MVP shell that connects to the Sprint 003 mock backend endpoints:

- GET /api/languages
- POST /api/translate/text
- POST /api/translate/audio

The UI must support:

- language selection from backend
- text translation form
- audio file upload translation form (file input only — no MediaRecorder)
- result display
- transcript display for audio
- loading states
- empty states
- client-side validation
- backend validation error display
- correlation ID display

Implementation constraints:

- Use React + TypeScript + Vite.
- Keep UI simple and MVP-focused.
- Do not add Azure SDKs.
- Do not add real translation providers.
- Do not add auth.
- Do not add database or persistence.
- Do not add WebSockets or streaming.
- Do not add text-to-speech playback.
- Do not add browser microphone recording or MediaRecorder.
- Do not change backend contracts unless a genuine mismatch is discovered and reported.
- Do not store secrets or credentials.
- Do not log source text, translated text, transcripts, audio filenames, or raw API payloads.
- Use actual backend error contract: errorCode (not code), details contain field and message only.
- Backend runs at http://localhost:5074.
- Use Vite proxy for /api/* to avoid CORS in dev.
- Add minimal CORS to Program.cs as belt-and-suspenders for local dev only.

Expected frontend behaviour:

1. App loads.
2. Frontend fetches languages from GET /api/languages.
3. User selects source language (or auto-detect) and target language.
4. User enters text or uploads an audio file.
5. Frontend performs basic client-side validation.
6. Frontend calls backend via Vite proxy.
7. UI displays mock translation result or structured error.
8. UI displays correlation ID in result and error panels.

Validation expectations:

- npm install — no errors, no secrets.
- npx tsc --noEmit — no type errors.
- npm run build — build succeeds.
- Manually validate text translation against backend.
- Manually validate audio upload against backend.
- Confirm no secrets added.
- Confirm docs and planning files updated.

After dry run, stop and wait for approval.
