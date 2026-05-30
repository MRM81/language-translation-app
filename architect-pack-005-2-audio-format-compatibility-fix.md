# Architect Pack: Sprint 005.2 - Audio Format Compatibility Fix

## Pack Metadata

| Field | Value |
|---|---|
| Project Name | My Translation App |
| Sprint Number | 005.2 |
| Sprint Name | Audio Format Compatibility Fix |
| Status | Ready For Builder |
| Builder Target | Claude Code |

---

## 1. Project Context

Sprint 005 and 005.1 successfully enabled real Azure text translation. Live text calls now return `provider: "azure"` and real translations. Audio translation is implemented, but compressed audio uploads such as MP3/WebM/OGG fail on Windows because the current Azure Speech SDK compressed-audio path requires local GStreamer. WAV audio reaches Azure Speech successfully.

Sprint 006 is planned for browser microphone / push-to-talk. Chrome MediaRecorder commonly outputs WebM/Opus, so Sprint 006 will be blocked unless compressed audio support is fixed first.

---

## 2. Sprint Goal

Make audio translation work reliably for compressed browser/upload formats without requiring local GStreamer installation. Preserve the working WAV path and add a GStreamer-free path for MP3/WebM/OGG using Azure Speech REST API or another low-risk Azure-supported approach.

---

## 3. Problem Being Solved

The current `AzureSpeechToTextProvider` uses Azure Speech SDK compressed audio streams for MP3/WebM/OGG. On Windows, this throws `ApplicationException` before recognition starts because the native compressed codec dependency is missing. This creates `PROVIDER_ERROR` for common uploaded files and would block Sprint 006 push-to-talk.

---

## 4. Requirements

| ID | Requirement | Priority | Notes |
|---|---|---|---|
| R-001 | Keep WAV transcription path working | Must | Existing WAV SDK path should remain unless there is a strong reason to replace it. |
| R-002 | Add functional compressed-audio transcription path | Must | MP3 and WebM/Opus are the primary targets. OGG/Opus should be supported if straightforward. |
| R-003 | Avoid local GStreamer requirement | Must | The app should run on a normal dev machine without installing native codec libraries. |
| R-004 | Preserve existing API contract | Must | Do not change frontend request/response DTOs unless absolutely required. |
| R-005 | Keep provider abstraction intact | Must | Changes should stay inside infrastructure/provider layer where possible. |
| R-006 | Preserve secure logging | Must | Do not log secrets, raw keys, full request bodies, or excessive provider internals. |
| R-007 | Add/adjust tests for format routing | Should | Unit or integration tests should verify compressed formats do not use the failing SDK path. |
| R-008 | Update planning/docs | Must | Record decision and state updates. |

---

## 5. In Scope

The Builder may work on:

- `AzureSpeechToTextProvider.cs`
- `AzureSpeechOptions.cs` if endpoint/config additions are needed
- DI/config registration only if needed for `HttpClient`
- Backend tests related to audio provider routing/error mapping
- `docs/ARCHITECTURE.md`
- `docs/API.md` or `docs/VALIDATION.md` if audio behavior is documented there
- `planning/STATE.md`
- `planning/DECISIONS.md`
- `planning/RISKS.md`
- `planning/QUESTIONS.md`
- `planning/sprints/005-2-audio-format-compatibility-fix/requirements.md`
- `planning/sprints/005-2-audio-format-compatibility-fix/blueprint.md`
- `planning/sprints/005-2-audio-format-compatibility-fix/acceptance.md`
- `planning/sprints/005-2-audio-format-compatibility-fix/handoff-prompt.md`

---

## 6. Out Of Scope

The Builder must not work on:

- Browser microphone UI / push-to-talk controls
- Frontend redesign
- User accounts/authentication
- Database or persistence
- Text-to-speech playback
- Changing text translation provider behavior
- Creating new Azure resources
- Committing secrets or changing real user secrets
- Installing GStreamer as the solution

---

## 7. Assumptions

| ID | Assumption | Confidence | Action If Wrong |
|---|---|---|---|
| A-001 | Existing Azure Speech key and region are valid | High | If REST auth fails, diagnose config before changing architecture. |
| A-002 | WAV path works with real speech | High | Verify with a real spoken WAV sample. |
| A-003 | Compressed formats fail due to local codec dependency, not Azure auth | High | Confirm with current diagnostic and targeted test. |
| A-004 | Azure Speech REST short-audio endpoint supports the needed compressed formats | Medium | Builder must verify against current Microsoft docs before implementation. |
| A-005 | Short-audio recognition is sufficient for MVP upload/push-to-talk snippets | Medium | Record duration limits in docs and risks. |

---

## 8. Constraints

- Do not commit real Azure keys or local secrets.
- Do not weaken error handling or logging security.
- Do not introduce paid third-party conversion services.
- Do not add large native dependencies unless explicitly approved.
- Keep implementation practical for MVP.
- Preserve current backend API shape.
- Preserve SOLID/provider abstraction boundaries.

---

## 9. Blueprint

### Step 1: Read and confirm current implementation

Read the active project files and inspect:

- Current audio upload endpoint flow
- `AzureSpeechToTextProvider` WAV path
- Current compressed-audio path
- MIME type normalization and validation
- Existing tests around audio translation

### Step 2: Verify Azure Speech REST compatibility

Use current Microsoft documentation or SDK docs to confirm:

- REST endpoint URL pattern for the configured region
- Required headers
- Accepted content types
- Duration/file-size limits
- Response shape
- Error response shape

Expected endpoint shape is likely regional, for example:

```text
https://{region}.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1?language={language}
```

Builder must verify before implementation.

### Step 3: Implement compressed-audio REST path

Recommended design:

- Keep WAV using existing SDK `AudioConfig.FromWavFileInput` path.
- For MP3/WebM/OGG, route to a new REST transcription helper.
- Use configured `AzureSpeech:Key` and `AzureSpeech:Region`.
- Add optional `AzureSpeech:Endpoint` only if necessary; prefer deriving from region unless docs require otherwise.
- Send audio bytes with the original/normalized content type.
- Parse successful transcript response.
- Map no-recognition/empty transcript to existing `TRANSCRIPTION_FAILED` behavior.
- Map provider/network/auth errors to existing provider error pattern.

### Step 4: Tests

Add or adjust tests to confirm:

- WAV still uses the WAV path.
- MP3/WebM are no longer sent through the local compressed SDK path.
- Unsupported formats remain rejected.
- Provider errors are mapped safely.
- Test factories remain provider-isolated and do not accidentally call Azure.

### Step 5: Manual validation

With User Secrets already configured for Azure:

- Test text translation still works.
- Upload a real spoken WAV file.
- Upload a real spoken MP3 file.
- Upload a WebM/Opus file if available.
- Confirm response includes transcript, translation, provider `azure`, and correlation ID.

### Step 6: Update docs/state

Update planning and docs with:

- Decision to avoid GStreamer and use REST for compressed audio.
- Known REST short-audio limits.
- Validation results.
- Sprint 006 readiness status.

---

## 10. API / Data Model Notes

No public API contract change is intended.

Existing endpoint remains:

```text
POST /api/translate/audio
```

Expected behavior remains:

- Accept multipart/form-data audio file
- Accept source language / target language fields as already implemented
- Return transcript, translated text, provider, source/target language, correlation ID
- Return existing error DTO shape on failure

---

## 11. Validation Rules

- Reject unsupported file types before provider call.
- Enforce existing max file size.
- Empty/no speech recognition should produce the existing transcription failure error, not a generic crash.
- Azure auth/network failures should produce provider-level errors without leaking secrets.
- Correlation ID must be present on success and failure.
- Tests must not depend on real Azure unless explicitly marked/manual.

---

## 12. Acceptance Criteria

Sprint is complete when:

- [ ] WAV transcription still works or is proven unchanged.
- [ ] MP3 upload with real speech successfully transcribes and translates using Azure.
- [ ] WebM/Opus upload is supported or a documented blocker is recorded with evidence.
- [ ] GStreamer is not required for successful MP3/WebM transcription.
- [ ] Existing text translation still returns `provider: "azure"` with real translations.
- [ ] Existing backend tests pass.
- [ ] New or updated tests cover compressed audio routing/error behavior where practical.
- [ ] No secrets are committed.
- [ ] `planning/DECISIONS.md` records the chosen audio compatibility approach.
- [ ] `planning/STATE.md` is updated.
- [ ] Relevant docs are updated.
- [ ] Sprint 006 readiness is explicitly stated.

---

## 13. Risks

| Risk | Impact | Likelihood | Mitigation | Owner |
|---|---|---|---|---|
| Azure Speech REST may not support every browser format equally | Medium | Medium | Validate MP3 first, WebM second; record limits. | Builder |
| REST short-audio endpoint may have duration limits | Medium | Medium | Document limit and keep push-to-talk snippets short for MVP. | Architect/Builder |
| MIME type sent by browser may include codec suffix | Medium | High | Normalize and preserve compatible content type headers. | Builder |
| Tests may accidentally use real Azure due to User Secrets | Medium | Low | Continue explicit Mock pinning in test factories. | Builder |
| Error responses may vary by Azure service | Low | Medium | Map safely to existing app error model. | Builder |

---

## 14. Open Questions

| Question | Needed From | Blocking? | Notes |
|---|---|---|---|
| Does Azure Speech REST accept the exact WebM/Opus output from Chrome MediaRecorder? | Builder verification | Yes for Sprint 006 readiness | Must verify before push-to-talk sprint. |
| What is the maximum practical recording duration for MVP? | Architect/User | No | Default assumption: short phrases only. |
| Should MP4 audio remain unsupported until later? | Architect/User | No | Safari may output MP4; may be deferred if REST/API support is unclear. |

---

## 15. State Updates Required

At sprint completion, update:

- `planning/STATE.md`
- `planning/DECISIONS.md`
- `planning/RISKS.md` if risk status changes
- `planning/QUESTIONS.md` if questions are answered or added
- `docs/ARCHITECTURE.md`
- `docs/API.md` or `docs/VALIDATION.md` if relevant
- active sprint acceptance status

---

## 16. Builder Dry Run Instructions

Before implementation, the Builder must produce a dry run summary.

The dry run must include:

- Files read
- Current audio provider flow
- Confirmed Microsoft/Azure REST endpoint details
- Planned file changes
- Planned tests
- Assumptions
- Risks or ambiguities
- Whether MP3/WebM support is safe to implement without GStreamer

Do not implement until dry run is reviewed.

---

## 17. Builder Handoff Prompt

```markdown
You are the Builder for My Translation App.

You are working on Sprint 005.2: Audio Format Compatibility Fix.

Follow the Architect / Builder methodology. Do not write implementation code immediately.

Read these files first:

1. AGENTS.md
2. planning/STATE.md
3. planning/DECISIONS.md
4. planning/RISKS.md
5. planning/QUESTIONS.md
6. docs/ARCHITECTURE.md
7. docs/API.md
8. docs/VALIDATION.md
9. planning/sprints/005-azure-provider-integration/requirements.md
10. planning/sprints/005-azure-provider-integration/blueprint.md
11. planning/sprints/005-azure-provider-integration/acceptance.md
12. AzureSpeechToTextProvider.cs
13. AzureSpeechOptions.cs
14. TranslationController.cs
15. Audio translation tests or provider selection tests

Sprint goal:
Make compressed audio uploads such as MP3/WebM/OGG work without requiring local GStreamer, while preserving the working WAV path and existing API contract.

Current known issue:
MP3/WebM/OGG fail on Windows because the Azure Speech SDK compressed-audio path requires local GStreamer. WAV reaches Azure Speech successfully.

Your first task is a dry run only.

Dry run must report:

1. Files read.
2. Current audio provider flow.
3. Exact failure point for compressed audio.
4. Confirmed Azure Speech REST endpoint, headers, accepted formats, and limits from current docs.
5. Planned implementation approach.
6. Files expected to change.
7. Tests expected to add/update.
8. Validation commands.
9. Risks or blockers.
10. Whether implementation is safe to start.

Do not implement until approved.
Do not commit secrets.
Do not install GStreamer as the solution.
Do not change the public API contract unless explicitly justified.
Use project files as the source of truth.
```

---

## 18. Completion Report Template

When finished, Builder should report:

```markdown
# Sprint 005.2 Completion Report

## Summary

## Files Modified

## Commands Run

## Tests / Validation

## Audio Format Validation Results

## Acceptance Criteria Status

## Decisions Added

## Risks Added Or Updated

## Questions Added Or Resolved

## Sprint 006 Readiness

## Known Limitations
```
