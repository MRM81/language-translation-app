# Sprint 004 — Frontend MVP Shell: Blueprint

**Project:** My Translation App
**Sprint:** 004
**Date:** 2026-05-28

---

## Files to Read First

1. `AGENTS.md`
2. `planning/STATE.md`
3. `planning/DECISIONS.md`
4. `planning/DOMAIN.md`
5. `planning/RISKS.md`
6. `planning/QUESTIONS.md`
7. `planning/FILE_INVENTORY.md`
8. `docs/ARCHITECTURE.md`
9. `docs/API.md`
10. `docs/VALIDATION.md`
11. `planning/sprints/004-frontend-mvp-shell/requirements.md` (this sprint)
12. `src/backend/MyTranslationApp.Api/Properties/launchSettings.json` (confirm port)
13. `src/backend/MyTranslationApp.Application/DTOs/*.cs` (confirm exact DTO shapes)

---

## Frontend Structure

```text
src/frontend/
├── README.md
├── package.json
├── index.html
├── vite.config.ts
├── tsconfig.json
├── tsconfig.node.json
├── .env.example
└── src/
    ├── main.tsx
    ├── App.tsx
    ├── api/
    │   └── translationApi.ts
    ├── components/
    │   ├── LanguageSelect.tsx
    │   ├── TextTranslationForm.tsx
    │   ├── AudioTranslationForm.tsx
    │   ├── ResultPanel.tsx
    │   └── ErrorPanel.tsx
    ├── types/
    │   └── api.ts
    └── styles/
        └── app.css
```

---

## Implementation Steps

### Step 1: Scaffold Vite React TypeScript App

Create `src/frontend/` with:

- `package.json` — React, ReactDOM, TypeScript, Vite, @vitejs/plugin-react
- `index.html` — Vite entry point
- `vite.config.ts` — proxy `/api` to `http://localhost:5074` (avoids CORS in dev)
- `tsconfig.json`, `tsconfig.node.json`
- `.env.example` — documents `VITE_API_BASE_URL=http://localhost:5074`

Vite proxy approach: configure the dev server to proxy `/api/*` to the backend. This avoids CORS entirely in development while keeping the frontend config clean.

### Step 2: Define TypeScript Types

Create `src/types/api.ts` with interfaces matching the backend DTOs exactly:

- `LanguageOption` — `code`, `name`
- `LanguageListResponse` — `languages`, `correlationId`
- `TextTranslationRequest` — `sourceText`, `sourceLanguage?`, `targetLanguage`
- `TextTranslationResponse` — `translatedText`, `sourceLanguage`, `targetLanguage`, `provider`, `correlationId`
- `AudioTranslationResponse` — `transcribedText`, `translatedText`, `sourceLanguage`, `targetLanguage`, `provider`, `correlationId`
- `ApiErrorDetail` — `field`, `message`
- `ApiErrorResponse` — `errorCode`, `message`, `details`, `correlationId`

### Step 3: Build API Client

Create `src/api/translationApi.ts`:

- `fetchLanguages()` → `LanguageListResponse`
- `translateText(req)` → `TextTranslationResponse`
- `translateAudio(file, targetLanguage, sourceLanguage?)` → `AudioTranslationResponse`
- Parse structured errors into `ApiErrorResponse` on non-OK responses
- No logging of request bodies, response text, or file names

### Step 4: Build Components

**`LanguageSelect.tsx`**
- Props: `label`, `value`, `onChange`, `languages`, `includeAuto?`
- Source language selector includes an "Auto-detect" option (value = empty string)
- Disabled while languages are loading

**`TextTranslationForm.tsx`**
- Text area input (max 5000 chars) with live character count
- Source language selector (with auto-detect)
- Target language selector (required)
- Submit button (disabled while loading or invalid)
- Client-side validation: text required, max 5000 chars, target language required, source ≠ target
- Calls `translateText`, shows loading state

**`AudioTranslationForm.tsx`**
- `<input type="file" accept="audio/*">` — no MediaRecorder
- Source language selector (with auto-detect)
- Target language selector (required)
- Submit button (disabled while loading or invalid)
- Client-side validation: file required, file ≤ 10 MB, target language required
- Advisory warning if file MIME type is not in the accepted list
- Calls `translateAudio`, shows loading state

**`ResultPanel.tsx`**
- Displays text or audio translation result
- Shows: translated text, transcript (audio only), source language, target language, provider, correlation ID
- Empty state when no result

**`ErrorPanel.tsx`**
- Displays API error response
- Shows: user-friendly message, `errorCode`, field-level `details[]`, `correlationId`
- Hidden when no error

### Step 5: Assemble App

`App.tsx`:
- Fetches languages on mount via `fetchLanguages()`
- Handles language load failure with a visible error state
- Renders language selectors (shared between forms)
- Renders `TextTranslationForm` and `AudioTranslationForm` side-by-side or stacked
- Renders `ResultPanel` and `ErrorPanel` below forms
- Clears result/error when a new submission begins

`main.tsx`:
- Mounts `<App />` into the DOM

### Step 6: Styles

`src/styles/app.css`:
- Clean, minimal styles
- Readable labels, clear submit buttons
- Loading indicator (disabled + visual cue on button)
- Mobile-friendly layout (single column on small screens)
- No external CSS framework required

---

## Backend API Client Contract

The frontend API client must use these exact field names when parsing responses:

| Endpoint | Key Response Fields |
|---|---|
| `GET /api/languages` | `languages[].code`, `languages[].name`, `correlationId` |
| `POST /api/translate/text` | `translatedText`, `sourceLanguage`, `targetLanguage`, `provider`, `correlationId` |
| `POST /api/translate/audio` | `transcribedText`, `translatedText`, `sourceLanguage`, `targetLanguage`, `provider`, `correlationId` |
| Error responses | `errorCode`, `message`, `details[].field`, `details[].message`, `correlationId` |

---

## CORS Strategy

The Vite dev server proxies `/api/*` to `http://localhost:5074`. This means the browser never makes a cross-origin request during development. CORS is also added to `Program.cs` as a belt-and-suspenders measure for any tooling that bypasses the Vite proxy. The CORS policy allows only the Vite dev origin (`http://localhost:5173`) and is development-only intent.

---

## Validation Plan

| Check | Method |
|---|---|
| `npm install` succeeds | Terminal output |
| `npx tsc --noEmit` passes | Terminal output |
| `npm run build` succeeds | Terminal output |
| Languages load in browser | Manual test |
| Text translation returns mock result | Manual test |
| Audio upload returns mock transcript + translation | Manual test |
| Empty text blocked | Manual test |
| Missing target language blocked | Manual test |
| File too large blocked client-side | Manual test |
| Backend error displays with errorCode + correlationId | Manual test |
| No console.log of user content | Code review |
| No secrets in any file | Code review |

---

## Documentation Updates Required

- `planning/STATE.md` — Sprint 003 complete, Sprint 004 active
- `planning/DECISIONS.md` — D-035 (Vite + React + TypeScript), D-036 (Vite proxy for local CORS), D-037 (audio file upload only in Sprint 004)
- `planning/RISKS.md` — R-019 (CORS gap), R-020 (DTO field name drift)
- `planning/QUESTIONS.md` — Q-016 through Q-019
- `planning/FILE_INVENTORY.md` — all Sprint 004 frontend files
- `docs/ARCHITECTURE.md` — frontend shell architecture section
- `docs/VALIDATION.md` — frontend validation and error display behaviour
