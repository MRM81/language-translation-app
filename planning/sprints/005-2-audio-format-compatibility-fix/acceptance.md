# Sprint 005.2 Acceptance — Audio Format Compatibility Fix

**Project:** My Translation App
**Sprint:** 005.2
**Date:** 2026-05-30
**Status:** Complete (automated tests pass; live validation pending credentials)

---

## Acceptance Criteria

| Criterion | Status |
|---|---|
| WAV transcription path preserved — SDK path unchanged | Done |
| MP3 upload routed to Fast Transcription REST API (no GStreamer) | Done — verified by `AzureSpeechProviderRoutingTests` |
| WebM/Opus upload routed to Fast Transcription REST API | Done — verified by `AzureSpeechProviderRoutingTests` |
| OGG/Opus upload routed to Fast Transcription REST API | Done — verified by `AzureSpeechProviderRoutingTests` |
| GStreamer not required for any supported format | Done — `RecognizeFromCompressedBytesAsync` removed |
| `audio/ogg` accepted by MIME validator | Done — added to `TranslationValidationOptions.cs` defaults and `appsettings.json` (see Post-completion Fix below) |
| `AzureSpeech:Endpoint` config field added and validated at startup | Done |
| Startup fails with clear message if `Endpoint` is missing in Azure mode | Done — `AzureProvider_MissingSpeechEndpoint_ThrowsOnStartup` passes |
| Existing 45 tests pass unchanged | Done — all 45 still pass |
| New provider routing tests added | Done — 16 new tests |
| Total tests passing | 61/61 |
| `dotnet build` succeeds — 0 errors, 0 warnings | Done |
| No real secrets committed | Confirmed — no credentials in any source file |
| Public API contract unchanged | Confirmed — no DTO changes |
| `planning/DECISIONS.md` records chosen approach | Done |
| `planning/STATE.md` updated | Done |
| `docs/ARCHITECTURE.md` updated | Done |
| `docs/VALIDATION.md` updated | Done |

---

## Test Counts

| Suite | Before Sprint 005.2 | After Sprint 005.2 |
|---|---|---|
| Validation tests | 16 | 16 |
| TranslationService tests | 5 | 5 |
| Controller integration tests | 9 | 9 |
| Provider selection tests | 15 | 16 (+1 missing-endpoint test) |
| Audio provider routing tests | 0 | 15 |
| **Total** | **45** | **61** |

---

## Live Azure Validation

Not run by Builder — no credentials available in this session. To validate with real Azure resources, follow the updated instructions in `src/backend/README.md`:

```bash
dotnet user-secrets set "AzureSpeech:Key" "YOUR_KEY" --project src/backend/MyTranslationApp.Api
dotnet user-secrets set "AzureSpeech:Region" "YOUR_REGION" --project src/backend/MyTranslationApp.Api
dotnet user-secrets set "AzureSpeech:Endpoint" "https://mark-translation-app-ai-001.cognitiveservices.azure.com" --project src/backend/MyTranslationApp.Api
```

Then:
1. Start backend with `Translation:Provider=Azure`
2. Upload a spoken WAV file — should transcribe and translate
3. Upload a spoken MP3 file — should transcribe and translate via Fast Transcription API
4. Upload a WebM/Opus file — should transcribe and translate via Fast Transcription API
5. Upload an OGG/Opus file — should transcribe and translate via Fast Transcription API

---

## Post-completion Fix — appsettings.json (found 2026-05-30)

Sprint 005.2 added `audio/ogg` to `TranslationValidationOptions.cs` code defaults but did not add it
to `appsettings.json`. In .NET, a JSON config array fully replaces code defaults at runtime. This meant
`audio/ogg` was absent from the live MIME validator even though `AllAcceptedMimeTypes_AreAllowed` tests
passed (tests bind from code defaults, not the JSON file).

Found during Sprint 006 Phase 0 preparation. Fixed: `audio/ogg` added to `AllowedAudioMimeTypes` in
`appsettings.json`. No code change, no test change required — tests were already correct.

---

## Known Limitations Carried Forward

- Audio duration enforcement remains deferred (R-016, Q-015).
- `audio/mp4` remains unsupported — `UNSUPPORTED_AUDIO_FORMAT` returned.
- Language auto-detect for speech defaults to `en-US` when no source language supplied (Q-008).
- Live validation with real audio files pending (no credentials in Builder environment).
