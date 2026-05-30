# Sprint 003 — Acceptance Criteria

**Project:** My Translation App
**Sprint:** 003 — Backend API Skeleton
**Date:** 2026-05-28
**Status:** Active

---

## Done Criteria

### Planning and Documentation

- [ ] planning/sprints/003-backend-api-skeleton/requirements.md exists and is complete
- [ ] planning/sprints/003-backend-api-skeleton/blueprint.md exists and is complete
- [ ] planning/sprints/003-backend-api-skeleton/acceptance.md exists and is complete
- [ ] planning/sprints/003-backend-api-skeleton/handoff-prompt.md exists and is complete
- [ ] planning/STATE.md updated: Sprint 002 completed, Sprint 003 active or completed
- [ ] planning/DECISIONS.md updated: D-027 through D-033 added
- [ ] planning/QUESTIONS.md updated: Q-009, Q-010, Q-011 marked resolved
- [ ] planning/RISKS.md updated: Sprint 003 risks added or updated
- [ ] planning/FILE_INVENTORY.md updated: all Sprint 003 files listed
- [ ] docs/API.md updated: GET /api/languages added, correlationId in response shapes confirmed
- [ ] docs/VALIDATION.md updated: actual validation rules implemented recorded
- [ ] docs/ARCHITECTURE.md updated: .NET 8 version recorded

### Backend Structure

- [ ] src/backend/MyTranslationApp.sln exists
- [ ] src/backend/MyTranslationApp.Api/ exists and builds
- [ ] src/backend/MyTranslationApp.Application/ exists and builds
- [ ] src/backend/MyTranslationApp.Infrastructure/ exists and builds
- [ ] tests/backend/MyTranslationApp.Tests/ exists and builds
- [ ] dotnet build passes with zero errors
- [ ] dotnet test passes with zero failures

### Architecture Dependency Direction (Critical)

- [ ] MyTranslationApp.Application has zero references to MyTranslationApp.Infrastructure
- [ ] MyTranslationApp.Infrastructure references only MyTranslationApp.Application
- [ ] MyTranslationApp.Api references Application and Infrastructure (composition root only)
- [ ] No Azure SDK, Azure.AI, or provider-specific package is referenced in any project

### Provider Interfaces

- [ ] ISpeechToTextProvider exists in Application/Interfaces with CancellationToken parameter
- [ ] ITextTranslationProvider exists in Application/Interfaces with CancellationToken parameter
- [ ] ITextToSpeechProvider exists in Application/Interfaces with CancellationToken parameter (future placeholder)
- [ ] ILanguageCatalogService exists in Application/Interfaces with CancellationToken parameter
- [ ] No interface method signature references an Azure SDK, provider-specific type, or HTTP type

### Mock Providers

- [ ] MockTextTranslationProvider exists in Infrastructure and implements ITextTranslationProvider
- [ ] MockSpeechToTextProvider exists in Infrastructure and implements ISpeechToTextProvider
- [ ] MockTextToSpeechProvider exists in Infrastructure and implements ITextToSpeechProvider
- [ ] StaticLanguageCatalogService exists in Infrastructure and implements ILanguageCatalogService
- [ ] Mock outputs are clearly prefixed (e.g. "[mock-es]", "[mock transcript]")

### DTOs

- [ ] TextTranslationRequestDto exists with sourceText, sourceLanguage, targetLanguage fields
- [ ] TextTranslationResponseDto exists with translatedText, sourceLanguage, targetLanguage, provider, correlationId
- [ ] AudioTranslationResponseDto exists with transcribedText, translatedText, sourceLanguage, targetLanguage, provider, correlationId
- [ ] ApiErrorResponseDto exists with errorCode, message, details, correlationId
- [ ] ApiErrorDetailDto exists with field and message
- [ ] LanguageOptionDto exists with code and name
- [ ] LanguageListResponseDto exists with languages list and correlationId
- [ ] No DTO contains a provider-specific type, enum, or property

### Error Codes

- [ ] ErrorCodes.cs exists in Application/Validation with string constants for all error codes
- [ ] All error code usages in validation and middleware reference the constants (not raw strings)

### Validation

- [ ] TranslationRequestValidator validates: sourceText required, sourceText max 5000 chars, targetLanguage required
- [ ] TranslationRequestValidator validates: audio file required, audio size within limit, MIME type in allowed list
- [ ] MIME normalisation strips parameters before comparison (audio/webm;codecs=vp8 matches audio/webm in allowed list)
- [ ] Audio duration validation is documented as deferred: code comment, TODO, or note in VALIDATION.md
- [ ] Validation errors return correct error code constants
- [ ] Requests failing validation never reach provider methods

### Server-Level Size Limits

- [ ] Kestrel MaxRequestBodySize is configured (10 MB)
- [ ] FormOptions MultipartBodyLengthLimit is configured (10 MB)
- [ ] Limits are read from configuration, not hard-coded in middleware

### Endpoints

- [ ] POST /api/translate/text accepts JSON body and returns TextTranslationResponseDto
- [ ] POST /api/translate/audio accepts multipart/form-data and returns AudioTranslationResponseDto
- [ ] GET /api/languages returns LanguageListResponseDto
- [ ] No /api/health endpoint exists
- [ ] No authentication endpoints exist
- [ ] No TTS endpoint is wired (interface only)

### Correlation ID

- [ ] CorrelationIdMiddleware injects X-Correlation-ID header into every response
- [ ] If X-Correlation-ID header is present on the request, the same value is reused
- [ ] If X-Correlation-ID header is absent, a new GUID is generated
- [ ] correlationId field appears in all success response DTOs
- [ ] correlationId field appears in all error response DTOs

### Logging Safety

- [ ] No raw audio bytes appear in any log statement
- [ ] No sourceText content appears in any log statement
- [ ] No translatedText content appears in any log statement
- [ ] No transcribedText content appears in any log statement
- [ ] Correlation IDs, endpoint names, status codes, duration, and exception types may be logged
- [ ] ExceptionHandlingMiddleware logs exception type only, not message or stack trace

### Constraints Verified

- [ ] No Azure SDK package is referenced in any project
- [ ] No real credentials, API keys, secrets, or connection strings appear in any file
- [ ] No database schema, migration, or persistence code exists
- [ ] No authentication or authorization code exists
- [ ] No WebSocket or streaming endpoint exists
- [ ] No React frontend files exist under src/

### Test Coverage

- [ ] Validation test: valid text request passes validation
- [ ] Validation test: empty sourceText returns VALIDATION_ERROR
- [ ] Validation test: sourceText > 5000 chars returns TEXT_TOO_LONG
- [ ] Validation test: missing targetLanguage returns VALIDATION_ERROR
- [ ] Validation test: missing audio file returns VALIDATION_ERROR
- [ ] Validation test: unsupported MIME type returns UNSUPPORTED_AUDIO_FORMAT
- [ ] Validation test: audio/webm;codecs=opus is accepted (normalised to audio/webm base)
- [ ] Validation test: audio size over limit returns AUDIO_TOO_LARGE
- [ ] Service test: TranslateTextAsync returns mock translation
- [ ] Service test: TranslateAudioAsync returns mock transcript and mock translation
- [ ] Controller integration test: POST /api/translate/text returns correlationId in response
- [ ] Controller integration test: invalid request returns structured error with correlationId

---

## Known Limitations Accepted for Sprint 003

- Audio duration cannot be reliably measured from IFormFile without an audio decoding library. Duration validation is deferred to Sprint 005+. File size limit (10 MB) is enforced as the nearest proxy.
- Source language validation accepts any non-empty string or "auto". Full BCP-47 code validation against the language catalog is deferred.
- MIME type supplied by the client can be spoofed. Backend MIME validation is a best-effort guard, not a cryptographic guarantee.
