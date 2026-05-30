# Validation

**Project:** My Translation App
**Sprint:** 005 — Azure Provider Integration (updated)
**Date:** 2026-05-28
**Status:** Sprint 005 complete — provider failure handling implemented. See Sprint 005 section below.

---

## Overview

Validation occurs at two layers:

- **Frontend (UX guard):** Prevents clearly invalid submissions before a network request is made. Improves user experience but is not a security boundary. Frontend validation can be bypassed.
- **Backend (security boundary):** Enforces all rules authoritatively. Backend validation cannot be bypassed and must run before any external provider is called.

---

## Input Validation — Text Translation

| Rule | Layer | Detail |
|---|---|---|
| Text must not be empty or whitespace-only. | Frontend + Backend | Reject blank submissions. |
| Text must not exceed maximum length. | Frontend + Backend | Suggested maximum: 2000 characters. Exact value TBD (Q-009). Show character count in UI. |
| Source language selection. | Frontend + Backend | If manual selection is used, reject if sourceLanguage is missing. If auto-detect is supported, source language is optional — see planning/QUESTIONS.md Q-008. |
| Target language must be selected. | Frontend + Backend | Reject if targetLanguage is missing or empty. |
| Source and target language must differ. | Frontend + Backend | Reject if sourceLanguage equals targetLanguage when both are explicitly provided. |
| Language codes must be valid BCP-47 codes from the supported list. | Backend | Frontend may rely on the language selector populated from GET /api/languages, but backend must re-validate authoritatively. |

---

## Input Validation — Audio Translation

| Rule | Layer | Detail |
|---|---|---|
| Audio must be present. | Frontend + Backend | Reject if no audio blob is included in the request. |
| Audio duration must not exceed maximum. | Frontend + Backend | Suggested maximum: 60 seconds. TBD — see planning/QUESTIONS.md Q-010. Frontend should stop recording at limit. Backend must reject oversized audio. |
| Audio file size must not exceed maximum. | Backend | Suggested maximum: 10 MB. Exact value TBD. |
| Audio MIME type must be in the accepted list. | Backend | Accepted MIME types TBD — see planning/QUESTIONS.md Q-011. Backend returns UNSUPPORTED_AUDIO_FORMAT if MIME type is not accepted. Frontend should check MediaRecorder MIME type before sending where possible. |
| Source language. | Frontend + Backend | Optional if auto-detect is supported. Reject if missing and auto-detect is not configured — see planning/QUESTIONS.md Q-008. |
| Target language must be selected. | Frontend + Backend | Reject if missing. |
| Source and target language must differ. | Frontend + Backend | Reject if both are explicitly provided and equal. |
| Language codes must be valid BCP-47 codes. | Backend | Same rule as text translation. |

---

## Input Validation — Speech Synthesis (TTS)

| Rule | Layer | Detail |
|---|---|---|
| Text must not be empty. | Backend | Reject if text is missing or whitespace-only. |
| Text must not exceed maximum length. | Backend | Suggested maximum: 2000 characters. TBD. |
| Language code must be provided and valid. | Backend | Must be a BCP-47 code from the supported list. |

---

## Source Language Auto-Detect Policy

Source language is optional when auto-detection is supported by the configured provider.

- If `sourceLanguage` is omitted and the provider supports auto-detection, the backend passes a null or empty source language to the provider and returns the detected language in the response.
- If `sourceLanguage` is omitted and the provider does not support auto-detection, the backend returns INVALID_REQUEST.
- The frontend should not assume auto-detect is always available. It should display the detected source language when returned.
- Auto-detect policy depends on provider choice — see planning/QUESTIONS.md Q-001 and Q-008. Final policy to be confirmed before Sprint 003.

---

## Audio MIME Type Policy

Browser MediaRecorder output MIME type varies across browsers and operating systems. See also planning/RISKS.md R-011.

- Accepted MIME types are TBD pending provider support confirmation — see planning/QUESTIONS.md Q-011.
- Likely candidates include `audio/webm`, `audio/webm;codecs=opus`, `audio/ogg`, `audio/mp4`.
- Backend must validate the MIME type of the uploaded audio blob explicitly before processing.
- If the MIME type is not accepted, backend returns UNSUPPORTED_AUDIO_FORMAT with a safe message.
- Frontend should check `MediaRecorder.isTypeSupported()` for preferred types before recording and fall back gracefully if needed.
- Final accepted MIME type list must be documented before Sprint 003 implementation begins.

---

## Business Rule Validation

| Rule | Layer | Detail |
|---|---|---|
| Source and target language must differ for all translation endpoints. | Frontend + Backend | A user selecting "English" to "English" provides no translation value. |
| Unsupported language pairs must be rejected gracefully. | Backend | Return INVALID_LANGUAGE error with a safe message. Do not expose provider error details. |
| Audio translation must always return a transcript alongside the translated text. | Backend | User must be able to see what was transcribed, not only the translation result. |
| Session history (if displayed) must not be persisted beyond the current browser session. | Frontend | No server-side storage of translation results in MVP. |

---

## Security Validation

| Rule | Layer | Detail |
|---|---|---|
| Provider API keys must never appear in any response body, header, or error message. | Backend | Treat as a critical security rule. |
| Raw audio must not be logged by default. | Backend | Audio content is not written to any log output, file, or monitoring sink. |
| Source text must not be logged by default. | Backend | The text a user submits for translation is not written to logs. |
| Translated text must not be logged by default. | Backend | The translation result is not written to logs. |
| Correlation IDs and timing metrics are permitted in logs. | Backend | These are safe operational identifiers that contain no user content. |
| Requests that fail validation must not reach the external provider. | Backend | Validate before forwarding. Prevents abuse and unnecessary provider cost. |
| Error responses must be sanitised — no internal stack traces, provider messages, or system paths. | Backend | Return only the structured error format defined in docs/API.md. |
| Audio files must be size, duration, and MIME type checked before processing. | Backend | Prevents abuse, excessive API cost, and potential DoS through large or malformed file uploads. |

---

## Provider Timeout and Availability Handling

| Scenario | Expected Behaviour |
|---|---|
| Provider does not respond within configured timeout. | Backend returns PROVIDER_TIMEOUT. Frontend shows: "The translation service took too long to respond. Please try again." |
| Provider returns an error response. | Backend maps error to PROVIDER_ERROR or more specific code. Frontend shows: "The translation service is temporarily unavailable. Please try again shortly." |
| Provider returns an empty or malformed transcription result. | Backend returns TRANSCRIPTION_FAILED. Frontend shows: "We couldn't understand the audio. Please try again." |
| Provider returns an empty or malformed translation result. | Backend returns TRANSLATION_FAILED. Frontend shows: "Translation failed. Please try again." |

Provider timeout values are configuration settings, not hard-coded. Default values TBD in Sprint 003.

---

## Error Handling

| Scenario | Expected Behaviour |
|---|---|
| Text input is empty. | Frontend shows inline error: "Please enter text to translate." Request not sent. |
| Language not selected. | Frontend shows inline error: "Please select a language." Request not sent. |
| Source equals target language. | Frontend shows inline error: "Source and target languages must be different." |
| Input text exceeds character limit. | Frontend shows character count warning. Backend rejects with TEXT_TOO_LONG. |
| Microphone permission denied. | Frontend shows error state: "Microphone access was denied. Please allow microphone access in your browser settings." |
| Audio recording too long. | Frontend stops recording at limit. Backend rejects oversized audio with AUDIO_TOO_LONG. |
| Audio MIME type not supported. | Backend rejects with UNSUPPORTED_AUDIO_FORMAT. Frontend shows: "Your browser's audio format is not supported. Please try a different browser." |
| Provider returns a transcription error. | Backend returns TRANSCRIPTION_FAILED. Frontend shows: "We couldn't understand the audio. Please try again." |
| Provider returns a translation error. | Backend returns TRANSLATION_FAILED. Frontend shows: "Translation failed. Please try again." |
| Provider is unavailable. | Backend returns PROVIDER_ERROR. Frontend shows: "The translation service is temporarily unavailable. Please try again shortly." |
| Provider timeout. | Backend returns PROVIDER_TIMEOUT. Frontend shows: "The translation service took too long to respond. Please try again." |
| Network request fails. | Frontend shows: "Connection error. Please check your internet connection and try again." |

---

## Test Strategy (Sprint 003+)

Validation testing should cover the following categories:

**Unit tests (backend):**
- Each validation rule returns the correct error code for invalid input.
- Boundary values (e.g. text at exactly 2000 and 2001 characters) behave correctly.
- Missing required fields produce INVALID_REQUEST.
- Invalid language codes produce INVALID_LANGUAGE.

**Unit tests (frontend):**
- Validation helpers return the expected error messages for each invalid input.
- Character count display updates correctly as user types.
- Submit is blocked when validation fails.

**Integration tests (backend):**
- Requests that fail validation return structured error responses and do not call provider adapters.
- Requests that pass validation reach the mock provider adapter.
- Correlation IDs appear in all error responses.
- No raw audio, source text, or translated text appears in log output.

**Contract tests:**
- Response DTOs match the documented shape in docs/API.md.
- Error responses always include `error`, `message`, and `correlationId`.

**Manual / exploratory tests:**
- Microphone permission denied state on mobile and desktop browsers.
- Push-to-talk recording and upload on Chrome, Safari, Firefox.
- MediaRecorder MIME type compatibility across target browsers.
- Long recordings are cut off correctly at the duration limit.

---

## Test Cases

| ID | Case | Input | Expected Result |
|---|---|---|---|
| V-001 | Empty text input | Empty string | Frontend error — request not sent |
| V-002 | Text at exact character limit | 2000 chars | Accepted and translated |
| V-003 | Text exceeds character limit | 2001+ chars | Backend returns TEXT_TOO_LONG |
| V-004 | No source language selected (when required) | Missing field | Frontend + backend error |
| V-005 | No target language selected | Missing field | Frontend + backend error |
| V-006 | Source equals target | "en" to "en" | Frontend + backend error |
| V-007 | Invalid language code | "xx-invalid" | Backend returns INVALID_LANGUAGE |
| V-008 | No audio recorded | Empty audio | Frontend error — request not sent |
| V-009 | Audio within duration limit | 30-second clip | Accepted and processed |
| V-010 | Audio exceeds duration limit | 90-second clip | Backend returns AUDIO_TOO_LONG |
| V-011 | Audio file too large | Over size limit | Backend returns AUDIO_TOO_LARGE |
| V-012 | Microphone permission denied | Permission denied | Frontend shows permission error state |
| V-013 | Provider returns error | Simulated provider failure | Backend returns PROVIDER_ERROR — no internal detail in response |
| V-014 | No API keys in response | Any request | Confirm no credentials appear in any response body or header |
| V-015 | Unsupported audio MIME type | Non-accepted format | Backend returns UNSUPPORTED_AUDIO_FORMAT |
| V-016 | Provider timeout | Simulated timeout | Backend returns PROVIDER_TIMEOUT — no internal detail in response |
| V-017 | No raw content in logs | Any translation request | Confirm no audio, source text, or translated text appears in log output |
| V-018 | Correlation ID in error | Any invalid request | Error response includes correlationId field |

---

## Sprint 003 — Implemented Validation Rules

The following rules are implemented in `TranslationRequestValidator` and tested in `TranslationValidationTests`.

| Rule | Implemented | Error Code | Notes |
|---|---|---|---|
| sourceText required | Yes | VALIDATION_ERROR | Empty or whitespace-only input rejected |
| sourceText max 5,000 characters | Yes | TEXT_TOO_LONG | At-limit (5000 chars) accepted; over-limit (5001 chars) rejected |
| targetLanguage required | Yes | VALIDATION_ERROR | Empty or whitespace-only rejected |
| sourceLanguage optional | Yes | — | null or "auto" accepted; BCP-47 strings accepted |
| Audio file required | Yes | VALIDATION_ERROR | null or empty IFormFile rejected |
| Audio MIME type in allowed list | Yes | UNSUPPORTED_AUDIO_FORMAT | MIME normalised by stripping parameters before comparison |
| Audio file size ≤ 10 MB | Yes | AUDIO_TOO_LARGE | Enforced by TranslationRequestValidator |
| Audio duration ≤ 60 seconds | Deferred | — | See Known Limitations below |
| Correlation ID in all responses | Yes | — | CorrelationIdMiddleware; reuses X-Correlation-ID header or generates GUID |
| Server-level size limits | Yes (Kestrel) | — | KestrelServerOptions and FormOptions configured at 10 MB |

---

## Sprint 003 — Known Limitations

### Audio Duration Enforcement

Audio duration validation (60-second limit) is deferred. The backend cannot reliably measure audio duration from a raw `IFormFile` stream without an audio parsing library.

**Current enforcement:** File size limit (10 MB) is enforced as the nearest proxy.

**Configuration:** `Translation:MaxAudioSeconds = 60` is recorded in appsettings.json for future use.

**Code marker:** A TODO comment in `TranslationValidationOptions.cs` documents the deferral.

**Resolution:** Introduce an audio parsing dependency (e.g. NAudio or TagLib#) in Sprint 005+ to enable exact duration enforcement.

**Tracking:** Q-015, R-016.

### MIME Type Spoofing

MIME type supplied by the HTTP client can be spoofed. Backend MIME validation is a best-effort guard, not a cryptographic guarantee. Accepted in MVP scope.

### Kestrel Limits in Test Environment

Request body size limits configured via Kestrel and FormOptions do not apply to the in-memory WebApplicationFactory test server. Logical size validation via TranslationRequestValidator is tested and functions in all environments. Kestrel limits apply in production.

---

## Sprint 004 — Frontend Validation Behaviour

The following validation rules are implemented in the React frontend as UX guards. Backend remains the authoritative validation boundary.

### Text Translation Form

| Rule | Implementation | UX Behaviour |
|---|---|---|
| Text required | `sourceText.trim()` empty check | Inline error: "Please enter text to translate." Request not sent. |
| Text max 5,000 characters | `sourceText.length > 5000` | Live character count shown. Translate button disabled when over limit. Inline error shown on submit attempt. |
| Target language required | `targetLanguage` empty check | Inline error: "Please select a target language." Request not sent. |
| Source ≠ target language | `sourceLanguage === targetLanguage` when both set | Inline error: "Source and target languages must be different." Request not sent. |

Character count display: remaining characters shown below the textarea. Turns red and shows "N characters over limit" when exceeded.

### Audio Upload Form

| Rule | Implementation | UX Behaviour |
|---|---|---|
| File required | `file === null` check | Inline error: "Please select an audio file." Request not sent. |
| File max 10 MB | `file.size > 10485760` | Inline error: "File size must be 10 MB or less." Request not sent. |
| Target language required | `targetLanguage` empty check | Inline error: "Please select a target language." Request not sent. |
| Source ≠ target language | `sourceLanguage === targetLanguage` when both set | Inline error: "Source and target languages must be different." Request not sent. |
| MIME type advisory | `file.type` checked against accepted list (base type only) | Advisory warning shown below file input if MIME is not in the accepted list. Request is still allowed — backend is authoritative. |

Accepted MIME types for advisory check: `audio/webm`, `audio/mp4`, `audio/mpeg`, `audio/wav`. Backend normalises by stripping parameters before comparison (D-030).

### Language Loading

| Scenario | UX Behaviour |
|---|---|
| Languages loading | "Loading available languages…" banner. Forms hidden until load completes. |
| Language load failure | Error banner: "Could not load the language list. Please check that the backend is running and refresh the page." |

### Backend Error Display

The `ErrorPanel` component parses the structured backend error response and displays:

- `message` — human-readable description
- `details[]` — field-level errors: `field` and `message`
- `errorCode` — machine-readable code (shown for debugging)
- `correlationId` — shown for tracing

Network errors (fetch throws, no response body) are displayed as a string message in `ErrorPanel`.

### Privacy Rules (Frontend)

- No `console.log` of `sourceText`, `translatedText`, `transcribedText`, audio file names, or raw request/response bodies.
- Source language and target language codes are safe to log.
- Correlation IDs are safe to display in the UI.

### Loading State

- Submit buttons are disabled with text "Translating…" while a request is in flight.
- New submissions clear the previous result and error state before the request is sent.

---

## Sprint 005 — Provider Failure Handling

### ProviderException

`ProviderException` is a typed exception in the Application layer (`Application/Exceptions/`) that carries an `ErrorCode` string. Azure adapters throw it for any provider failure. `ExceptionHandlingMiddleware` catches it and returns HTTP 502 with the specific error code.

| Scenario | ErrorCode | HTTP Status | Message (safe) |
|---|---|---|---|
| Azure Translator returns empty result | `TRANSLATION_FAILED` | 502 | "Translation service returned no result. Please try again." |
| Azure Translator returns non-success HTTP status | `PROVIDER_ERROR` | 502 | "The translation service is temporarily unavailable. Please try again shortly." |
| Azure Translator request times out | `PROVIDER_TIMEOUT` | 502 | "The translation service took too long to respond. Please try again." |
| Azure Speech STT: no speech recognised | `TRANSCRIPTION_FAILED` | 502 | "Speech recognition could not understand the audio. Please speak clearly and try again." |
| Azure Speech STT: recognition cancelled with error | `PROVIDER_ERROR` | 502 | "The speech transcription service is temporarily unavailable. Please try again shortly." |
| Azure Speech STT: unsupported audio format for SDK | `UNSUPPORTED_AUDIO_FORMAT` | 502 | "Audio format '...' is not supported by the speech transcription provider." |
| Any unhandled exception | `INTERNAL_ERROR` | 500 | "An unexpected error occurred. Please try again." |

### Provider Config Validation

When `Translation:Provider = "Azure"`, startup validation runs before any request is accepted:

| Check | Failure Behaviour |
|---|---|
| `AzureTranslator:Key` is empty or missing | `InvalidOperationException` at startup with instructions |
| `AzureTranslator:Region` is empty or missing | `InvalidOperationException` at startup with instructions |
| `AzureSpeech:Key` is empty or missing | `InvalidOperationException` at startup with instructions |
| `AzureSpeech:Region` is empty or missing | `InvalidOperationException` at startup with instructions |
| `Translation:Provider` is an unknown value | `InvalidOperationException` at startup with instructions |

When `Translation:Provider = "Mock"`, no credential validation runs. The app starts immediately.

### Audio Format Support (Azure Speech) — Sprint 005.2

Sprint 005.2 replaced the SDK compressed-audio path (which required GStreamer on Windows) with the Azure Speech Fast Transcription REST API for all non-WAV formats. See D-049.

| MIME Type | Path | Support | Notes |
|---|---|---|---|
| `audio/wav` | Speech SDK (`AudioConfig.FromWavFileInput`) | Full | Temp file written; deleted in `finally`. No GStreamer dependency. |
| `audio/mpeg` | Fast Transcription REST API | Full | Accepted in validation; routed to REST. |
| `audio/webm` | Fast Transcription REST API | Full | Normalised from `audio/webm;codecs=opus` before routing. |
| `audio/ogg` | Fast Transcription REST API | Full | Added to `AllowedAudioMimeTypes` in Sprint 005.2. Firefox MediaRecorder output. |
| `audio/mp4` | — | Not supported | Passes MIME validation; rejected by provider with `UNSUPPORTED_AUDIO_FORMAT`. AAC/MP4 not accepted by Fast Transcription API for MVP. |

**Fast Transcription API limits:** 5 hours / 500 MB per request — well above MVP limits (60 seconds / 10 MB).

---

## Sprint 007 — TTS Validation

### TTS Request Validation

Implemented in `TranslationRequestValidator.ValidateTtsRequest`.

| Rule | Error Code | Notes |
|---|---|---|
| `text` must not be empty or whitespace-only | `VALIDATION_ERROR` | Returns 400 with field-level detail |
| `language` must not be empty or whitespace-only | `VALIDATION_ERROR` | Returns 400 with field-level detail |

Language code validity is not validated against the catalog at the validation layer — if Azure cannot synthesize speech for the given language, `ProviderException(PROVIDER_ERROR)` is returned.

### TTS Provider Failure Handling

| Scenario | ErrorCode | HTTP Status | Message (safe) |
|---|---|---|---|
| Azure TTS synthesis returns cancelled with error | `PROVIDER_ERROR` | 502 | "The speech synthesis service is temporarily unavailable. Please try again shortly." |
| Azure TTS synthesis returns cancelled without error | `PROVIDER_ERROR` | 502 | "Speech synthesis was cancelled. Please try again." |
| Azure TTS synthesis throws unexpected exception | `PROVIDER_ERROR` | 502 | "The speech synthesis service is temporarily unavailable. Please try again shortly." |

### Manual Validation Checklist (Sprint 007)

- [ ] Start backend with `Translation:Provider=Azure` and User Secrets.
- [ ] POST `/api/translate/tts` with `{"text":"Hello, how are you?","language":"es"}` → HTTP 200, `Content-Type: audio/mpeg`, non-empty body.
- [ ] Confirm `X-Correlation-ID` header is present in the response.
- [ ] Open frontend in Chrome: translate text, click Play → audio plays.
- [ ] Open frontend in Chrome: push-to-talk, translate, click Play → audio plays.
- [ ] Click Play while audio is already playing → previous audio stops, new audio starts.
- [ ] POST `/api/translate/tts` with empty text → HTTP 400, `VALIDATION_ERROR`.
- [ ] No secrets or user content appear in logs.
