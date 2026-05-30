# Sprint 003 — Blueprint

**Project:** My Translation App
**Sprint:** 003 — Backend API Skeleton
**Date:** 2026-05-28
**Status:** Active

---

## Implementation Steps

### Step 1 — Dry Run (Completed)

Builder read all required files, inspected the repo structure, identified .NET SDK 8.0.303, summarised intended changes, listed assumptions and risks, and waited for Architect approval before proceeding.

Approval granted with corrections:
- CancellationToken added to interfaces, services, and controllers
- Kestrel / form upload size limits added at server level
- Error codes centralised as constants
- MIME normalisation (strip parameters, starts-with for audio/webm)
- X-Correlation-ID reused from request if present
- GET /api/languages added to acceptance criteria
- Architecture dependency direction added to acceptance criteria

### Step 2 — Create Sprint 003 Documentation

Files:
- planning/sprints/003-backend-api-skeleton/requirements.md
- planning/sprints/003-backend-api-skeleton/blueprint.md (this file)
- planning/sprints/003-backend-api-skeleton/acceptance.md
- planning/sprints/003-backend-api-skeleton/handoff-prompt.md

### Step 3 — Create Backend Solution and Projects

Commands:
```
dotnet new sln -n MyTranslationApp -o src/backend
dotnet new webapi --use-controllers -n MyTranslationApp.Api -o src/backend/MyTranslationApp.Api
dotnet new classlib -n MyTranslationApp.Application -o src/backend/MyTranslationApp.Application
dotnet new classlib -n MyTranslationApp.Infrastructure -o src/backend/MyTranslationApp.Infrastructure
dotnet new xunit -n MyTranslationApp.Tests -o tests/backend/MyTranslationApp.Tests
```

Project references (dependency direction enforced):
- Api → Application
- Api → Infrastructure (composition root only)
- Infrastructure → Application
- Tests → Application
- Tests → Api (for WebApplicationFactory integration tests)

NuGet packages added to Application:
- Microsoft.Extensions.Options
- Microsoft.Extensions.Logging.Abstractions

NuGet packages added to Tests:
- Microsoft.AspNetCore.Mvc.Testing

### Step 4 — Application Layer

Files created under src/backend/MyTranslationApp.Application/:

Interfaces/:
- ISpeechToTextProvider.cs
- ITextTranslationProvider.cs
- ITextToSpeechProvider.cs (future placeholder only)
- ILanguageCatalogService.cs

Services/:
- TranslationService.cs

DTOs/:
- TextTranslationRequestDto.cs
- TextTranslationResponseDto.cs
- AudioTranslationResponseDto.cs
- ApiErrorResponseDto.cs
- ApiErrorDetailDto.cs
- LanguageOptionDto.cs
- LanguageListResponseDto.cs

Validation/:
- ErrorCodes.cs (centralised string constants)
- ValidationResult.cs
- TranslationValidationOptions.cs
- TranslationRequestValidator.cs

### Step 5 — Infrastructure Layer

Files created under src/backend/MyTranslationApp.Infrastructure/:

Providers/:
- MockSpeechToTextProvider.cs — returns "[mock transcript]"
- MockTextTranslationProvider.cs — returns "[mock-{targetLanguage}] {input}"
- MockTextToSpeechProvider.cs — placeholder stub, returns empty byte array
- StaticLanguageCatalogService.cs — returns hardcoded list of 10 common language codes

### Step 6 — API Layer

Files created/replaced under src/backend/MyTranslationApp.Api/:

Program.cs — composition root:
- Kestrel MaxRequestBodySize = 10 MB
- FormOptions MultipartBodyLengthLimit = 10 MB
- TranslationValidationOptions bound from appsettings.json "Translation" section
- Mock providers and services registered
- CorrelationIdMiddleware and ExceptionHandlingMiddleware registered
- Controllers mapped

appsettings.json — non-secret configuration only:
- Translation:MaxTextCharacters = 5000
- Translation:MaxAudioSeconds = 60
- Translation:MaxAudioBytes = 10485760
- Translation:AllowedAudioMimeTypes = [...]
- Translation:Provider = Mock

Controllers/:
- TranslationController.cs — POST /api/translate/text, POST /api/translate/audio
- LanguagesController.cs — GET /api/languages

Middleware/:
- CorrelationIdMiddleware.cs — reuses X-Correlation-ID from request or generates GUID; sets response header
- ExceptionHandlingMiddleware.cs — catches unhandled exceptions; logs type only; returns INTERNAL_ERROR envelope

### Step 7 — Tests

Files created under tests/backend/MyTranslationApp.Tests/:

Validation/:
- TranslationValidationTests.cs

Services/:
- TranslationServiceTests.cs

Controllers/ (integration):
- TranslationControllerTests.cs — uses WebApplicationFactory<Program>

### Step 8 — Verify Build and Tests

```
dotnet build src/backend/MyTranslationApp.sln
dotnet test src/backend/MyTranslationApp.sln
```

### Step 9 — Update Planning and Docs

Files updated:
- planning/STATE.md
- planning/DECISIONS.md (add D-027 through D-034)
- planning/QUESTIONS.md (resolve Q-009, Q-010, Q-011; add new open question)
- planning/RISKS.md (add/update Sprint 003 risks)
- planning/FILE_INVENTORY.md (add all new files)
- docs/API.md (confirm endpoints, add GET /api/languages, correlationId in responses)
- docs/VALIDATION.md (record actual rules implemented)
- docs/ARCHITECTURE.md (record .NET 8 selection and architecture dependency rule)

---

## Architecture Dependency Rules (Enforced)

```
Api          → Application  (call services, bind DTOs)
Api          → Infrastructure  (DI registration only in Program.cs)
Infrastructure → Application  (implement interfaces)
Tests        → Application
Tests        → Api

Application  ✗ Infrastructure  (zero references)
Infrastructure ✗ Api
Domain/Shared not needed for MVP skeleton
```

---

## Logging Rules Applied

Allowed:
- Correlation IDs
- Endpoint names
- Request start/end time, duration
- HTTP status codes
- Validation failure category (error code only, no field values)
- Provider name ("mock")
- Exception type (not message, not stack trace)

Forbidden:
- Source text
- Translated text
- Transcribed text
- Raw audio bytes
- API keys or secrets
- Full provider error messages or stack traces
