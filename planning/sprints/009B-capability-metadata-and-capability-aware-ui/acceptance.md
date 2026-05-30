# Sprint 009B Acceptance — Language Capability Metadata & Capability-Aware UI

## API Acceptance

- [x] `GET /api/languages` preserves `code` and `name` fields (no breaking change).
- [x] `GET /api/languages` includes `supportsTextTranslation`, `supportsSpeechToText`, `supportsTextToSpeech` on every language entry.
- [x] All 37 Sprint 009A languages have all three capability flags set to `true`.

## Frontend Acceptance

- [x] `LanguageOption` TypeScript interface includes the three capability fields.
- [x] `App.tsx` derives `targetLangSupportsTts` from the loaded language list for the current result.
- [x] `ResultPanel` accepts `targetLangSupportsTts` prop.
- [x] Play button is disabled with label "Audio unavailable" when `targetLangSupportsTts` is `false`.
- [x] Play button operates normally (idle/loading/playing/error) when `targetLangSupportsTts` is `true` or not provided.
- [x] Existing Play button UX (Sprint 007) is fully preserved.

## Test Acceptance

- [x] Backend build — 0 warnings, 0 errors.
- [x] Backend tests — 129/129 pass (115 prior + 14 new capability integration tests).
- [x] Frontend TypeScript — clean (`npx tsc --noEmit`).
- [x] Frontend build — clean (`npm run build`).

## Documentation Acceptance

- [x] `docs/API.md` updated.
- [x] `docs/ARCHITECTURE.md` updated.
- [x] `docs/VALIDATION.md` updated.
- [x] `planning/STATE.md` updated.
- [x] `planning/DECISIONS.md` updated (D-074 to D-077).
- [x] `planning/RISKS.md` updated (R-042 to R-044).
- [x] `planning/QUESTIONS.md` updated (Q-037 to Q-040 resolved).

## Live Azure Validation (Pending Credentials)

- [ ] `GET /api/languages` returns 37 entries with capability flags in browser
- [ ] Translate English → Czech → Play button is enabled and produces audio
- [ ] Future: add a language with `supportsTextToSpeech: false` and confirm Play button shows "Audio unavailable"
