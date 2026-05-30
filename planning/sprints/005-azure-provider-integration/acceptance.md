# Sprint 005 Acceptance — Azure Provider Integration

**Project:** My Translation App
**Sprint:** 005
**Date:** 2026-05-28
**Status:** Complete

---

## Acceptance Criteria

| Criterion | Status |
|---|---|
| Sprint 005 planning files exist under `planning/sprints/005-azure-provider-integration/` | Done |
| Azure text translation provider is implemented behind `ITextTranslationProvider` | Done |
| Azure speech-to-text provider is implemented behind `ISpeechToTextProvider` | Done |
| Mock providers remain available | Done |
| Provider selection supports at least `Mock` and `Azure` | Done |
| Invalid provider configuration fails safely at startup | Done |
| Missing Azure configuration fails safely at startup when provider is Azure | Done |
| Public API endpoint paths remain unchanged | Done |
| Public request/response DTO shapes remain unchanged | Done |
| Existing mock-mode tests pass | Done — 30/30 original tests pass |
| New provider-selection/config validation tests added | Done — 15 new tests (45 total) |
| `dotnet build` succeeds | Done — 0 errors |
| `dotnet test` succeeds | Done — 45/45 pass |
| No real secrets, keys, tokens, or credentials are committed | Done |
| `appsettings.json` contains placeholders or non-secret defaults only | Done |
| Backend README documents Azure local setup using User Secrets or environment variables | Done |
| README documents how to switch between Mock and Azure provider modes | Done |
| README documents manual live validation steps | Done |
| No raw text, transcripts, audio filenames, audio content, or request bodies are logged | Done |
| `planning/STATE.md` is updated | Done |
| `planning/DECISIONS.md` is updated | Done |
| `planning/RISKS.md` is updated | Done |
| `planning/QUESTIONS.md` is updated | Done |
| `planning/FILE_INVENTORY.md` is updated | Done |
| `docs/ARCHITECTURE.md` is updated | Done |
| `docs/VALIDATION.md` is updated | Done |

---

## Test Counts

| Suite | Before Sprint 005 | After Sprint 005 |
|---|---|---|
| Validation tests | 16 | 16 |
| TranslationService tests | 5 | 5 |
| Controller integration tests | 9 | 9 |
| Provider selection tests | 0 | 15 |
| **Total** | **30** | **45** |

---

## Live Azure Validation

Not run by Builder — no credentials available. To validate with real Azure resources, follow the instructions in `src/backend/README.md`.

---

## Known Limitations Carried Forward

- Audio duration enforcement remains deferred (R-016, Q-015).
- WebM audio format is not fully supported by Azure Speech SDK (see blueprint.md).
- MP4 audio format is not supported by Azure Speech SDK (see blueprint.md).
- Language auto-detect for speech transcription defaults to `en-US` when no source language is supplied.
