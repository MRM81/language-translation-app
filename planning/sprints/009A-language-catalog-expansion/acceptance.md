# Sprint 009A Acceptance — Language Catalog Expansion

## Catalog Acceptance

- [x] Expanded language catalog implemented (37 languages).
- [x] Czech (`cs`) is available.
- [x] Slovak (`sk`) is available.
- [x] German (`de`) is available and documented as Austria-compatible for MVP.
- [x] Mandarin Chinese (`zh`) is available with clear label "Chinese (Simplified)".
- [x] Chinese Traditional (`zh-Hant`) is available with label "Chinese (Traditional)".
- [x] Existing original 10 languages still available.
- [x] Language list is alphabetically ordered — usable on mobile.

## Translation Acceptance

- [ ] Text translation works for Czech — live validation pending (Azure credentials required).
- [ ] Text translation works for Slovak — live validation pending.
- [ ] Text translation works for German — confirmed working from Sprint 005.
- [ ] Text translation works for Mandarin Chinese — confirmed working from Sprint 005.
- [x] Existing translation workflows still work (71 passing tests preserved).

## Speech-To-Text Acceptance

- [x] STT mappings updated for all 37 languages.
- [x] Unsupported STT languages fail gracefully (pass-through fallback in ResolveLanguage).
- [x] Push-to-talk still works for existing supported languages (115/115 tests pass).

## Text-To-Speech Acceptance

- [x] TTS voice mappings updated for all 37 languages.
- [x] Unsupported TTS languages fail gracefully (SpeechSynthesisLanguage fallback).
- [x] TTS playback still works for existing supported languages (115/115 tests pass).

## Test Acceptance

- [x] Backend build succeeds — 0 warnings, 0 errors.
- [x] Backend tests pass — 115/115 (71 existing + 44 new catalog tests).
- [ ] Frontend TypeScript check — not re-run (no frontend files changed).
- [ ] Frontend build — not re-run (no frontend files changed).
- [x] Language catalog tests added: `LanguageCatalogTests.cs` (44 tests).
- [x] Validation tests unchanged and passing.

## Documentation Acceptance

- [x] docs/API.md updated.
- [x] docs/ARCHITECTURE.md updated.
- [x] docs/VALIDATION.md updated.
- [x] planning/STATE.md updated.
- [x] planning/DECISIONS.md updated (D-070 to D-073).
- [x] planning/RISKS.md updated (R-038 to R-041).
- [x] planning/QUESTIONS.md updated (Q-034 to Q-036).

## Live Validation (Requires Azure Credentials)

The following are pending live validation once Azure credentials are available:

- [ ] English → Czech text translation
- [ ] English → Slovak text translation
- [ ] English → German text translation
- [ ] English → Mandarin Chinese text translation
- [ ] Czech → English text translation
- [ ] Slovak → English text translation
- [ ] TTS playback for German (`de-DE-KatjaNeural`)
- [ ] TTS playback for Czech (`cs-CZ-VlastaNeural`)
- [ ] TTS playback for Mandarin Chinese (`zh-CN-XiaoxiaoNeural`)
- [ ] Push-to-talk STT for at least one expanded language
