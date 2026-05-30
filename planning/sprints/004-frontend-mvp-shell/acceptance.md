# Sprint 004 — Frontend MVP Shell: Acceptance Criteria

**Project:** My Translation App
**Sprint:** 004
**Date:** 2026-05-28

---

## Acceptance Criteria

Sprint is complete when all items below are checked.

### Planning and Documentation

- [ ] Sprint 004 planning folder exists: `requirements.md`, `blueprint.md`, `acceptance.md`, `handoff-prompt.md`.
- [ ] `planning/STATE.md` updated — Sprint 003 marked complete, Sprint 004 active.
- [ ] `planning/DECISIONS.md` updated with frontend tooling decisions (D-035+).
- [ ] `planning/RISKS.md` updated with frontend risks (R-019, R-020).
- [ ] `planning/QUESTIONS.md` updated with Q-016 through Q-019.
- [ ] `planning/FILE_INVENTORY.md` updated with all Sprint 004 frontend files.
- [ ] `docs/ARCHITECTURE.md` updated with frontend shell architecture notes.
- [ ] `docs/VALIDATION.md` updated with frontend validation and error display behaviour.

### Frontend Application

- [ ] A React + TypeScript frontend app exists at `src/frontend/`.
- [ ] Frontend can be installed with `npm install` from `src/frontend/`.
- [ ] Frontend can be run locally using `npm run dev`.
- [ ] `npm run build` completes without errors.
- [ ] `npx tsc --noEmit` passes without type errors.
- [ ] Backend base URL is configurable via `.env` or `.env.local` and documented in `.env.example`.
- [ ] No secrets or credentials are present in any committed frontend file.
- [ ] No Azure SDKs or real provider integrations are present.

### Language Loading

- [ ] App fetches languages from `GET /api/languages` on load.
- [ ] Source language selector includes an "Auto-detect" option.
- [ ] Target language selector is populated from backend response.
- [ ] Language load failure shows a visible error state (not a blank/broken page).

### Text Translation

- [ ] Text translation form calls `POST /api/translate/text`.
- [ ] Text result panel displays: translated text, source language, target language, provider, correlation ID.
- [ ] Character count is displayed and updates as user types.
- [ ] Client-side validation blocks submission when: text is empty, text exceeds 5,000 characters, target language is missing, source equals target.
- [ ] Backend validation errors display with `errorCode`, `message`, `details`, and `correlationId`.

### Audio Translation

- [ ] Audio upload form uses `<input type="file" accept="audio/*">` — no MediaRecorder.
- [ ] Audio upload calls `POST /api/translate/audio` as multipart/form-data.
- [ ] Audio result panel displays: transcript, translated text, source language, target language, provider, correlation ID.
- [ ] Client-side validation blocks submission when: no file selected, file exceeds 10 MB, target language is missing.
- [ ] Advisory message shown if selected file MIME type is not in the accepted list.
- [ ] Backend validation errors display with `errorCode`, `message`, `details`, and `correlationId`.

### UI States

- [ ] Loading state shown during API calls (button disabled, visual indicator).
- [ ] Empty/initial state shown before any translation is submitted.
- [ ] Network/server failure state shown if the request fails unexpectedly.

### Privacy and Safety

- [ ] No `console.log` of source text, translated text, transcripts, audio filenames, or raw API payloads.
- [ ] No backend contract changes made unless documented and approved.

---

## Validation Commands

```bash
cd src/frontend

# Install
npm install

# Type check
npx tsc --noEmit

# Production build
npm run build

# Dev server
npm run dev
```

Backend:

```bash
cd src/backend/MyTranslationApp.Api
dotnet run
```

---

## Manual Validation Checklist

Run these checks with both the backend and frontend running locally.

| # | Check | Expected |
|---|---|---|
| 1 | Open app in browser | App loads, language selectors are populated |
| 2 | Submit text translation with valid input | Mock result appears with correlationId |
| 3 | Submit audio translation with a small audio file | Mock transcript and translation appear |
| 4 | Submit text translation with empty text | Inline validation error, no request sent |
| 5 | Submit without selecting target language | Inline validation error, no request sent |
| 6 | Submit text translation with source = target | Inline validation error, no request sent |
| 7 | Submit audio with a file over 10 MB | Client-side size warning |
| 8 | Observe browser DevTools Network tab | No source text, transcripts, or audio in console logs |
| 9 | Inspect committed files for secrets | No API keys, tokens, or credentials |
| 10 | Kill backend, submit translation | Network error state shown |
