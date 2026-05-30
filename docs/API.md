# API

**Project:** My Translation App
**Sprint:** 003 — Backend API Skeleton (updated)
**Date:** 2026-05-28
**Status:** Sprint 003 — skeleton implemented with mock providers. Endpoints are live and tested. All 30 tests pass.

---

## Overview

The backend exposes a small HTTP API that the React frontend calls via a dedicated service layer. All external provider API calls are made server-side. The frontend never communicates directly with Azure, Google, OpenAI, DeepL, or any other translation/speech provider.

All endpoints accept and return JSON unless otherwise noted. Audio data is sent as multipart/form-data.

---

## Endpoints

### POST /api/translate/text

Translate typed text from a source language to a target language.

**Request:**

| Field | Type | Required | Notes |
|---|---|---|---|
| text | string | Yes | The text to translate. Must be non-empty. Maximum length TBD (suggest 2000 characters). |
| sourceLanguage | string | No | BCP-47 language code (e.g. "en", "es", "fr"). Optional if provider supports auto-detection. See planning/QUESTIONS.md Q-008. |
| targetLanguage | string | Yes | BCP-47 language code. Must differ from sourceLanguage if provided. |

**Response:**

| Field | Type | Notes |
|---|---|---|
| translatedText | string | The translated result. |
| sourceLanguage | string | Detected or echoed from request. |
| targetLanguage | string | Echoed from request. |
| provider | string | Optional — name of provider used (for diagnostics only, not for display). |

---

### POST /api/translate/audio

Accept a short recorded audio clip, transcribe it using speech-to-text, then translate the transcript.

**Request (multipart/form-data):**

| Field | Type | Required | Notes |
|---|---|---|---|
| audio | binary | Yes | Recorded audio blob. Maximum size and duration TBD (suggest 10 MB / 60 seconds). Accepted MIME types TBD — see planning/QUESTIONS.md Q-011. |
| sourceLanguage | string | No | BCP-47 language code for the spoken audio. Optional if provider supports auto-detection. |
| targetLanguage | string | Yes | BCP-47 language code for the translation output. |

**Response:**

| Field | Type | Notes |
|---|---|---|
| transcript | string | The speech-to-text result from the audio. Always returned alongside translation. |
| translatedText | string | The translated result. |
| sourceLanguage | string | Detected or echoed from request. |
| targetLanguage | string | Echoed from request. |

---

### GET /api/languages

Return the list of supported source and target language codes and display names.

**Implemented:** Yes (Sprint 003 — StaticLanguageCatalogService returns 10 common language codes).

**Response:**

| Field | Type | Notes |
|---|---|---|
| languages | array | List of language objects. |
| languages[].code | string | BCP-47 language code. |
| languages[].name | string | Human-readable display name. |

**Notes:** Language list is derived from the configured provider. The exact list depends on provider choice (see planning/QUESTIONS.md Q-001 and Q-002).

---

### POST /api/translate/tts

Convert translated text to spoken audio for browser playback. Implemented in Sprint 007.

**Request:**

```json
{
  "text": "Hola, ¿cómo estás?",
  "language": "es"
}
```

| Field | Type | Required | Notes |
|---|---|---|---|
| text | string | Yes | The translated text to synthesize. Must be non-empty. |
| language | string | Yes | BCP-47 language code (e.g. "en", "es"). Must be non-empty. |

**Response:**

- Content-Type: `audio/mpeg` (Azure mode) or `audio/wav` (Mock mode)
- Body: binary audio data suitable for browser playback.
- Header: `X-Correlation-ID` — request correlation ID (binary body cannot carry a JSON correlationId field).

**Error Handling:** Uses existing error response conventions. Returns `VALIDATION_ERROR` for empty text or empty language. Returns `PROVIDER_ERROR` (502) if Azure Speech fails.

**Notes:**
- Endpoint does not store generated audio.
- Playback is user-initiated (no automatic speech generation).
- Voice is selected from a server-side voice map; no voice selection is exposed to the client.

---

## DTO Conventions

Request and response types are explicit, named DTOs. Provider SDK response objects are never returned directly to the API layer or frontend. All provider-to-domain mapping occurs inside Infrastructure adapters.

### DTO Names

| DTO | C# Class | Direction | Purpose | Sprint |
|---|---|---|---|---|
| Text translation request | `TextTranslationRequestDto` | Inbound | Typed text translation request fields | 003 |
| Text translation response | `TextTranslationResponseDto` | Outbound | Translated text result + correlationId | 003 |
| Audio translation response | `AudioTranslationResponseDto` | Outbound | Transcript, translated text + correlationId | 003 |
| TTS synthesis request | `TtsSynthesisRequestDto` | Inbound | Text and language for speech synthesis | 007 |
| Language option | `LanguageOptionDto` | Outbound | Single language entry (code + display name) | 003 |
| Language list response | `LanguageListResponseDto` | Outbound | Language list + correlationId | 003 |
| API error response | `ApiErrorResponseDto` | Outbound | Structured error envelope returned on all errors | 003 |
| API error detail | `ApiErrorDetailDto` | Outbound | Field-level error detail (nested in error envelope) | 003 |

All success and error response DTOs include a `correlationId` field populated by `CorrelationIdMiddleware`.

### DTO Versioning

No URL-based API versioning is used in the MVP. The API is accessed via a single path for each endpoint.

DTOs are designed to support safe future extension without breaking existing clients:

- New optional fields may be added to response DTOs at any time. Existing clients that do not read the new field are unaffected.
- Required request fields must not be removed or renamed in a deployed DTO without a coordinated release.
- If a breaking change becomes unavoidable, introduce a new endpoint path or version prefix at that time rather than changing an existing one in place.
- Do not add a versioning scheme prematurely — defer until a breaking change actually requires it.

---

## Validation Responsibilities

See docs/VALIDATION.md for full rules.

| Layer | Responsibility |
|---|---|
| Frontend | Non-empty input, language code presence, audio duration/size (UX guard only — not a security boundary). |
| Backend | Schema validation, language code validity, content size limits, audio MIME type, audio duration limits. |
| Backend | Never expose provider error messages, SDK types, or internal details in any response. |

---

## Error Responses

All errors return a consistent JSON shape:

| Field | Type | Notes |
|---|---|---|
| error | string | Machine-readable error code (e.g. "INVALID_LANGUAGE", "TEXT_TOO_LONG", "PROVIDER_ERROR"). |
| message | string | Human-readable message safe to display. No internal details, stack traces, or provider messages. |
| correlationId | string | Request correlation ID injected by middleware. Safe to log and include in support requests. Contains no sensitive data. |

**Standard error codes:**

| Code | Meaning |
|---|---|
| INVALID_REQUEST | Request is malformed or missing required fields. |
| INVALID_LANGUAGE | Source or target language code is not supported. |
| TEXT_TOO_LONG | Input text exceeds maximum allowed length. |
| AUDIO_TOO_LARGE | Audio file exceeds maximum size limit. |
| AUDIO_TOO_LONG | Audio duration exceeds maximum allowed duration. |
| UNSUPPORTED_AUDIO_FORMAT | Audio MIME type is not in the accepted list. |
| TRANSCRIPTION_FAILED | Speech-to-text provider returned no result or failed. |
| TRANSLATION_FAILED | Translation provider returned no result or failed. |
| PROVIDER_ERROR | External provider returned an unexpected error. |
| PROVIDER_TIMEOUT | Provider did not respond within the configured timeout. |
| RATE_LIMITED | Request rate limit exceeded (future feature). |

---

## Authentication And Authorization

No authentication is required in the MVP (no user accounts). See planning/DECISIONS.md D-005.

If authentication is added in a future sprint, all endpoints should require a valid bearer token. Provider credentials are never returned to the client.

---

## Provider Boundary Rules

- Controllers call application services only. Controllers do not call provider adapters or Azure SDK classes directly.
- Application services call provider interfaces only. They do not import Azure SDK types.
- Provider SDK response objects are mapped to Domain models inside Infrastructure adapters before being returned to the Application layer.
- No provider SDK type, response shape, error message, region code, or API key appears in any response DTO.
- Provider-specific identifiers, voice names, and configuration keys are confined to `Infrastructure/Providers/Azure/`.
- Provider errors are caught in Infrastructure adapters and mapped to structured Domain errors before propagating to the Application layer.

---

## API Rules

- Backend must validate all requests before calling any external provider.
- Backend must not expose provider API keys, credentials, or internal error messages in any response.
- Provider-specific response formats must be normalised into Domain models before returning to the frontend.
- Errors must be structured, safe, and human-readable without revealing system internals.
- Audio and translated text must not be permanently stored.
- Raw audio, source text, and translated text must not be logged.
- Correlation IDs must be injected by middleware on every request and included in all error responses.
