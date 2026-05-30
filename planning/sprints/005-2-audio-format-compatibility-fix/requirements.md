# Sprint 005.2 Requirements — Audio Format Compatibility Fix

**Project:** My Translation App
**Sprint:** 005.2
**Date:** 2026-05-30
**Status:** Complete

---

## Sprint Goal

Make compressed audio uploads (MP3, WebM, OGG) work without requiring local GStreamer, while preserving the working WAV path and existing API contract. Unblock Sprint 006 push-to-talk, which produces WebM/Opus from Chrome MediaRecorder.

---

## Requirements

| ID | Requirement | Priority | Status |
|---|---|---|---|
| R-001 | Keep WAV transcription path working | Must | Done |
| R-002 | Add functional compressed-audio transcription path for MP3, WebM, OGG | Must | Done |
| R-003 | Avoid local GStreamer requirement | Must | Done |
| R-004 | Preserve existing public API contract | Must | Done |
| R-005 | Keep provider abstraction intact | Must | Done |
| R-006 | Preserve secure logging | Must | Done |
| R-007 | Add/adjust tests for format routing and response parsing | Should | Done |
| R-008 | Update planning/docs | Must | Done |

---

## In Scope

- `AzureSpeechToTextProvider.cs`
- `AzureSpeechOptions.cs` — new `Endpoint` config field
- `TranslationValidationOptions.cs` — add `audio/ogg` to allowed types
- `MyTranslationApp.Infrastructure.csproj` — add `Microsoft.Extensions.Http`
- `Program.cs` — `AddHttpClient()`, updated startup error message
- Backend tests related to audio provider routing and error mapping
- `docs/ARCHITECTURE.md`
- `docs/VALIDATION.md`
- `planning/STATE.md`
- `planning/DECISIONS.md`
- `planning/RISKS.md`
- `planning/QUESTIONS.md`
- Sprint planning files

---

## Out Of Scope

- Browser microphone UI / push-to-talk controls
- Frontend redesign
- User accounts/authentication
- Database or persistence
- Text-to-speech playback
- Changing text translation provider behavior
- Creating new Azure resources
- Committing secrets or changing real user secrets
- Installing GStreamer as the solution
