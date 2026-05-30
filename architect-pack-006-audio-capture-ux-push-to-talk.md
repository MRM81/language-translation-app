# Architect Pack 006 — Audio Capture UX / Push-to-Talk

## Sprint Goal

Enable end-to-end browser microphone recording and translation using the existing Azure-backed audio transcription and translation pipeline.

This sprint focuses on browser capture UX only.

No backend architectural changes should be required.

---

# planning/STATE.md

## Sprint Status

Sprint 005.2: Complete

Sprint 006: Active

### Sprint 006 Objective

Provide browser microphone capture using MediaRecorder and submit recorded audio to the existing Azure speech translation pipeline.

---

# planning/DECISIONS.md

Add:

### D-052
MediaRecorder is the primary browser audio capture mechanism.

### D-053
Push-to-talk is the MVP interaction model.

### D-054
Audio is uploaded after recording completes.

No streaming transcription in MVP.

### D-055
Chrome and Edge are primary validation browsers.

Firefox is supported where MediaRecorder formats are compatible.

### D-056
AudioCaptureService must detect supported MIME types at runtime using:

MediaRecorder.isTypeSupported()

and select the first supported format.

---

# planning/RISKS.md

### R-024
Browser MIME support differs between Chrome, Edge, Firefox and mobile browsers.

### R-025
Users may deny microphone permissions.

### R-026
Mobile browser recording behavior may differ from desktop.

### R-027
Chrome MediaRecorder output may not exactly match the MIME types validated during Sprint 005.2.

---

# planning/QUESTIONS.md

### Q-025
Resolved during Sprint 006 validation:

Does Chrome-produced WebM audio successfully transcribe through Fast Transcription API?

### Q-026

Should automatic speech language detection be enabled in a future sprint?

### Q-027

Should maximum recording duration remain 60 seconds?

---

# requirements.md

## In Scope

### Sprint Prerequisite

Before implementation of recording UI components, the Builder must complete Phase 0 Browser Audio Validation and resolve Q-025.

No UI implementation should begin until:

Browser-generated audio has been tested against the Azure Fast Transcription API.
Actual MIME output has been documented.
Compatibility status has been confirmed.

### Microphone Permissions

- Request microphone permission
- Handle allow
- Handle deny
- Handle unavailable device

### Push-To-Talk

- Press to start recording
- Press to stop recording
- Clear visual state changes

### Recording UX

- Recording indicator
- Recording timer
- Recording status text

### MIME Detection

Application must:

1. Detect supported MediaRecorder MIME types
2. Select first supported format
3. Gracefully fail if unsupported

Preferred order:

1. audio/webm;codecs=opus
2. audio/webm
3. audio/ogg;codecs=opus
4. audio/ogg

### Upload Flow

- Create audio blob
- Upload to /api/translate/audio
- Existing Azure pipeline processes request

### Results

Display:

- Transcript
- Translation
- Provider
- Correlation ID

### Error Handling

Display user-friendly messages for:

- Permission denied
- Recording failed
- Upload failed
- Azure transcription failure
- Unsupported browser

## Out Of Scope

- Streaming transcription
- Streaming translation
- Text-to-speech playback
- Conversation mode
- Audio persistence
- Background recording

---

# blueprint.md

## Phase 0 — Browser Audio Validation (Mandatory)

Before implementing the full Push-To-Talk UI:

1. Record a short browser audio clip using MediaRecorder.
2. Capture and document the actual MIME type produced by the browser.
3. Upload the recording to the existing Azure audio pipeline.
4. Verify successful transcription through the Fast Transcription API.
5. Verify successful translation through the Azure Translator pipeline.

Decision Gate:

If transcription succeeds:

* Resolve Q-025 as "Supported".
* Proceed with Sprint 006 implementation.

If transcription fails:

* Investigate MIME/content-type compatibility.
* Document findings in planning/QUESTIONS.md.
* Implement remediation before continuing with UI implementation.

This phase is a prerequisite for all remaining Sprint 006 work.

---

## Frontend Flow

Idle
↓
Request Permission
↓
Ready
↓
Recording
↓
Stop Recording
↓
Create Blob
↓
Upload Audio
↓
Azure Transcription
↓
Azure Translation
↓
Display Results


## Components

### PushToTalkButton

Responsibilities:

- Start recording
- Stop recording
- Display state

### RecordingIndicator

Displays:

- Recording active
- Recording stopped

### RecordingTimer

Displays elapsed recording time.

### AudioCaptureService

Responsibilities:

- MIME detection
- MediaRecorder management
- Blob generation

### AudioTranslationResult

Displays:

- Transcript
- Translation
- Provider
- Correlation ID

---

# acceptance.md

## Phase 0 Acceptance (Q-025 Validation)

AC-001

Chrome MediaRecorder output successfully transcribes through the Fast Transcription API.

AC-002

The actual MIME type produced by Chrome is documented.

AC-003

Q-025 is resolved and recorded in planning/QUESTIONS.md.

AC-004

If MIME incompatibility is discovered, a remediation approach is documented before UI implementation proceeds.

## Functional Acceptance

AC-005

User can grant microphone access.

AC-006

User can deny microphone access and receive a useful error.

AC-007

User can start recording.

AC-008

User can stop recording.

AC-009

Audio blob is generated successfully.

AC-010

Audio uploads successfully.

AC-011

Transcript is displayed.

AC-012

Translation is displayed.

AC-013

Provider equals "azure".

AC-014

Correlation ID displayed.

## MIME Acceptance

AC-015

Application detects supported MIME type at runtime.

AC-016

Selected MIME type is logged during testing.

AC-017

Chrome successfully records and uploads audio.

AC-018

Edge successfully records and uploads audio.

AC-019

Firefox successfully records and uploads audio if supported.

## Technical Acceptance

AC-020

Build succeeds.

AC-021

Tests pass.

---

# handoff-prompt.md

Read all Sprint 006 files before implementation.

Produce:

1. Dry run summary
2. Files to modify
3. MIME detection strategy
4. Browser compatibility strategy
5. Test plan
6. Risks

Do not implement until dry run is approved.
