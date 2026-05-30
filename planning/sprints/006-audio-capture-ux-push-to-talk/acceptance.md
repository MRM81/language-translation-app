# Sprint 006 Acceptance — Audio Capture UX / Push-to-Talk

**Project:** My Translation App
**Sprint:** 006
**Date:** 2026-05-30
**Status:** Complete

---

## Phase 0 Acceptance (Q-025 Validation)

| Criterion | Status |
|---|---|
| AC-001 Chrome MediaRecorder output transcribes through Fast Transcription API | Done — Phase 0 live test confirmed HTTP 200 |
| AC-002 Actual MIME type produced by Chrome documented | Done — `audio/webm;codecs=opus` negotiated, `audio/webm` as blob Content-Type |
| AC-003 Q-025 resolved and recorded in planning/QUESTIONS.md | Done |
| AC-004 If MIME incompatibility found, remediation documented before UI work | N/A — pipeline confirmed working |

---

## Functional Acceptance

| Criterion | Status |
|---|---|
| AC-005 User can grant microphone access | Done — `AudioCaptureService.start()` calls `getUserMedia`; permission grant proceeds to recording |
| AC-006 User can deny microphone access and receive a useful error | Done — `NotAllowedError` caught, user-friendly message shown, UI resets to Idle |
| AC-007 User can start recording | Done — PushToTalkButton in Idle state calls `handleStartRecording` |
| AC-008 User can stop recording | Done — PushToTalkButton in Recording state calls `handleStopRecording` |
| AC-009 Audio blob generated successfully | Done — `AudioCaptureService.stop()` resolves with `{ blob, mimeType }` |
| AC-010 Audio uploads successfully | Done — blob wrapped as `File`, passed to existing `translateAudio()` API call |
| AC-011 Transcript displayed | Done — `ResultPanel` renders `transcribedText` for `kind: 'audio'` results (unchanged) |
| AC-012 Translation displayed | Done — `ResultPanel` renders `translatedText` (unchanged) |
| AC-013 Provider equals "azure" | Done — backend returns `provider: "azure"`, displayed in ResultPanel |
| AC-014 Correlation ID displayed | Done — `ResultPanel` renders `correlationId` (unchanged) |

---

## MIME Acceptance

| Criterion | Status |
|---|---|
| AC-015 Application detects supported MIME type at runtime | Done — `detectSupportedMimeType()` calls `MediaRecorder.isTypeSupported()` at recording start |
| AC-016 Selected MIME type logged during testing | Done — `phase0-test.html` logs MIME type; production code does not log user-session data |
| AC-017 Chrome successfully records and uploads audio | Done — Phase 0 confirmed; live UI validation pending |
| AC-018 Edge successfully records and uploads audio | Pending — live UI validation against Edge required |
| AC-019 Firefox successfully records and uploads audio if supported | Pending — Firefox uses `audio/ogg;codecs=opus`; backend accepts it; live UI validation required |

---

## Technical Acceptance

| Criterion | Status |
|---|---|
| AC-020 Build succeeds | Done — `npm run build` clean, 875ms, 0 errors |
| AC-021 Tests pass | Done — 61/61 backend tests pass; no frontend test framework in this sprint |

---

## Additional Fix (post-Sprint-005.2 remediation applied this sprint)

| Item | Status |
|---|---|
| `appsettings.json` missing `audio/ogg` in `AllowedAudioMimeTypes` | Fixed — added before Sprint 006 implementation began |
| `AzureSpeechProviderRoutingTests.AzureProvider_MissingSpeechEndpoint_ThrowsOnStartup` failing after Phase 0 credentials set | Fixed — added `builder.UseSetting("AzureSpeech:Endpoint", "")` to override User Secrets in test isolation |

---

## Known Limitations Carried Forward

- AC-018 (Edge) and AC-019 (Firefox) require live browser validation.
- No frontend unit test framework — `AudioCaptureService` MIME detection logic is untested in isolation.
- Q-027 open: 60-second recording limit is currently a hardcoded `MAX_RECORD_SECONDS` constant in `AudioTranslationForm.tsx`.
- Mobile/Safari (R-029) out of MVP scope — shows unsupported-browser error if no MIME type detected.
