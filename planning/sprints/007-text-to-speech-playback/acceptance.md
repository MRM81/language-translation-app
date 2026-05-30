# Sprint 007 Acceptance — Text-to-Speech Playback

## Backend Acceptance

- [x] AzureTextToSpeechProvider implemented.
- [x] MockTextToSpeechProvider updated — returns silent WAV fixture (44 bytes, audio/wav).
- [x] Provider switching works: Mock → MockTextToSpeechProvider, Azure → AzureTextToSpeechProvider.
- [x] TTS API endpoint exists: POST /api/translate/tts.
- [x] TTS endpoint rejects empty text (400 VALIDATION_ERROR).
- [x] TTS endpoint rejects empty language (400 VALIDATION_ERROR).
- [x] TTS endpoint returns browser-playable audio (200 + Content-Type audio/*).
- [x] TTS endpoint maps provider failures to existing error conventions (ProviderException → 502).
- [x] Correlation ID returned in X-Correlation-ID response header.
- [x] No secrets are logged or committed.

## Frontend Acceptance

- [x] Play button appears when translated text exists.
- [x] Play button works for text translation results.
- [x] Play button works for audio translation results.
- [x] Button shows loading state during audio generation.
- [x] Button shows playing state while audio is active.
- [x] Playback errors are shown clearly.
- [x] Object URLs are cleaned up (revokeObjectURL on ended and on replace).
- [x] Existing translation flows still work.

## Validation Acceptance

- [x] Backend build succeeds.
- [x] Backend tests pass (71/71).
- [x] Frontend TypeScript check passes.
- [x] Frontend build succeeds.
- [x] Manual Azure TTS validation — confirmed.
- [x] Manual text translation + playback in Chrome — confirmed.
- [x] Manual push-to-talk translation + playback in Chrome — confirmed.
- [x] planning/STATE.md updated.
- [x] planning/DECISIONS.md updated.
- [x] planning/RISKS.md updated.
- [x] planning/QUESTIONS.md updated.
- [x] docs/API.md updated.
- [x] docs/ARCHITECTURE.md updated.
- [x] docs/VALIDATION.md updated.

## Post-Completion Defect Fix

Defect found during validation: clicking Play showed "Failed to execute 'json' on 'Response': Unexpected end of JSON input".

Root cause: `synthesizeSpeech` called `res.json()` unconditionally in the error path. When the error response body was non-JSON (proxy error, empty body), `res.json()` threw a browser SyntaxError. The `ResultPanel` catch block passed `err.message` to the UI without checking whether the thrown value was a structured `ApiErrorResponse`.

Fix: `synthesizeSpeech` now checks `Content-Type` before calling `res.json()` in the error path. `ResultPanel` catch block now discriminates on `apiErr?.errorCode` so only structured API errors write their message to the UI; raw JS errors fall through to a safe fallback string.
