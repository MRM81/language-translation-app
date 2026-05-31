# Architect Pack: Sprint 014 — Push-To-Talk Conversation UX

## Pack Metadata

| Field | Value |
|---|---|
| Project Name | My Translation App |
| Sprint Number | 014 |
| Sprint Name | Push-To-Talk Conversation UX |
| Status | Ready For Builder |
| Depends On | Sprint 013 |
| Created | 2026-05-31 |

---

# 1. Project Context

Sprint 013 successfully introduced Conversation Mode.

Current conversation workflow:

- Text conversation
- Audio file upload conversation
- Auto-play translated responses
- Two participant language model
- Conversation history
- AWS production deployment

The biggest remaining UX friction is audio input.

Users must currently upload audio files.

This sprint introduces Push-To-Talk (PTT) recording directly in the browser.

---

# 2. Sprint Goal

Replace the audio file upload workflow in Conversation Mode with a fast Push-To-Talk experience while preserving file upload as a fallback.

---

# 3. Problem Being Solved

Current workflow:

Speak
→ Record externally
→ Save file
→ Upload file
→ Translate
→ Play response

Target workflow:

Hold button
→ Speak
→ Release
→ Translate
→ Auto-play response

This dramatically reduces interaction friction.

---

# 4. Requirements

| ID | Requirement | Priority |
|---|---|---|
| R-001 | Add Push-To-Talk button | Must |
| R-002 | Browser microphone recording | Must |
| R-003 | Auto-stop recording on release | Must |
| R-004 | Integrate with existing Conversation Mode | Must |
| R-005 | Reuse existing audio translation API | Must |
| R-006 | Preserve audio file upload fallback | Must |
| R-007 | Mobile support | Must |
| R-008 | Accessibility support | Must |
| R-009 | Loading and recording states | Must |
| R-010 | Preserve existing Translation Mode | Must |

---

# 5. In Scope

## Frontend

- MediaRecorder integration
- Push-To-Talk button
- Recording indicator
- Recording timer
- Mobile microphone workflow
- Conversation Mode integration

## Backend

No backend changes expected.

Reuse:

- POST /api/translate/audio
- POST /api/translate/tts

---

# 6. Out Of Scope

Do not implement:

- Continuous recording
- Streaming audio
- WebSockets
- Live transcription
- Voice activity detection
- Speaker diarization
- Conversation persistence
- Multi-party conversations

---

# 7. Architecture Principle

Push-To-Talk is a UI enhancement only.

Browser Microphone
→ MediaRecorder
→ Audio Blob
→ Existing /api/translate/audio
→ Existing TTS pipeline

No duplicate STT logic.

---

# 8. User Workflow

## Speaker A

Hold microphone button
→ Speak
→ Release
→ Upload recording
→ STT
→ Translation
→ Auto-play

Active speaker automatically switches to Speaker B.

---

# 9. UI Design

Desktop:

[ Speaker A ]
[ 🎤 Hold To Talk ]

Recording:

[ 🔴 Recording... 00:07 ]

After release:

[ Translating... ]

Mobile:

Large thumb-friendly microphone button.

Minimum target size:

48px × 48px

---

# 10. Component Changes

Expected new files:

src/frontend/src/components/
    PushToTalkButton.tsx

Expected modified files:

ConversationMode.tsx
ConversationInput.tsx
app.css

Existing audio upload workflow remains available as fallback.

---

# 11. MediaRecorder Strategy

Preferred MIME types:

audio/webm
audio/mp4

Fallback detection required.

Builder must verify browser compatibility.

---

# 12. States

Idle
Recording
Processing
Error

Visual feedback required for each state.

---

# 13. Accessibility

- Keyboard accessible
- Screen reader labels
- Recording state announced
- Error state announced

---

# 14. Validation Plan

Builder must validate:

- Chrome desktop
- Edge desktop
- Mobile browser
- Permission denied path
- Recording path
- Translation path
- Auto-play path

---

# 15. Acceptance Criteria

- Push-To-Talk button exists
- Browser recording works
- Recording indicator works
- Translation succeeds
- Auto-play succeeds
- Speaker switching succeeds
- File upload fallback remains
- Mobile verified
- Build passes
- Existing tests pass

---

# 16. Decisions To Add

D-094
D-095
D-096
D-097

---

# 17. Questions To Track

Q-052 Should voice activity detection replace hold-to-talk?

Q-053 Should microphone mode become default over text mode?

Q-054 Should recording be stored temporarily for replay?

---

# 18. Builder Dry Run Instructions

1. Review existing AudioCaptureService.
2. Review Conversation Mode implementation.
3. Review MediaRecorder support.
4. Determine browser compatibility.
5. Produce dry run report.
6. Wait for approval.

Do not implement immediately.

---

# 19. Builder Handoff Prompt

You are the Builder for My Translation App.

Sprint 014 — Push-To-Talk Conversation UX.

Read Sprint 013 outputs first.

Perform a dry run before implementation.

Reuse existing APIs and translation workflow.

Do not introduce new backend endpoints.

Focus on browser recording, UX speed, mobile usability, and accessibility.

Provide a dry run report before implementation.

---

# 20. Completion Report Requirements

Provide:

- Files created
- Files modified
- Build results
- Test results
- Browser compatibility findings
- Mobile validation
- Decisions added
- Risks added
- Questions added

Recommended next sprint:

Sprint 015 — Conversation Persistence & Export
