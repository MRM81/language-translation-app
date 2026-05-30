# Sprint 005 Blueprint — Azure Provider Integration

**Project:** My Translation App
**Sprint:** 005
**Date:** 2026-05-28
**Status:** Complete

---

## Implementation Steps

### Step 1 — Application Layer additions

- Added `ProviderException` to `Application/Exceptions/ProviderException.cs` with an `ErrorCode` property.
- Added `string ProviderName { get; }` to `ITextTranslationProvider` and `ISpeechToTextProvider`.
- Updated `TranslationService` to use `_translationProvider.ProviderName` in response DTOs and log messages.
- Updated `ExceptionHandlingMiddleware` to catch `ProviderException` and return 502 with the provider error code.

### Step 2 — Mock provider updates

- Added `ProviderName => "mock"` to `MockTextTranslationProvider`.
- Added `ProviderName => "mock"` to `MockSpeechToTextProvider`.

### Step 3 — Azure configuration models

- Created `AzureTranslationOptions` in `Infrastructure/Configuration/`: Endpoint, Region, Key, IsConfigured.
- Created `AzureSpeechOptions` in `Infrastructure/Configuration/`: Region, Key, IsConfigured.

### Step 4 — Azure SDK packages

Added to `MyTranslationApp.Infrastructure.csproj`:
- `Azure.AI.Translation.Text` 1.0.0 (originally 2.0.0; downgraded in Sprint 005.1 — see D-047)
- `Microsoft.CognitiveServices.Speech` 1.50.0

No Azure packages added to Application or Api.

### Step 5 — Azure provider implementations

- `AzureTextTranslationProvider`: implements `ITextTranslationProvider` using `TextTranslationClient`. Maps `RequestFailedException` to `ProviderException(PROVIDER_ERROR)` and empty results to `ProviderException(TRANSLATION_FAILED)`.
- `AzureSpeechToTextProvider`: implements `ISpeechToTextProvider` using `SpeechRecognizer`. WAV audio uses a temp file + `AudioConfig.FromWavFileInput`. Compressed audio (MP3, WebM) uses `AudioInputStream.CreatePushStream` with the appropriate `AudioStreamContainerFormat`. Maps recognition results to `ProviderException` with appropriate error codes.

### Step 6 — Provider selection in Program.cs

- Reads `Translation:Provider` from configuration (default: `"Mock"`).
- `Mock`: registers mock providers — no credentials required.
- `Azure`: validates that `AzureTranslator:Key`, `AzureTranslator:Region`, `AzureSpeech:Key`, `AzureSpeech:Region` are all non-empty. Throws `InvalidOperationException` at startup if any are missing. Registers Azure providers only when all config is present.
- Unknown value: throws `InvalidOperationException` at startup.
- Provider name matching is case-insensitive.
- `ITextToSpeechProvider` and `ILanguageCatalogService` remain mock in all modes.

### Step 7 — Configuration and secrets

- `appsettings.json`: added `AzureTranslator` and `AzureSpeech` sections with placeholder values only.
- Added `<UserSecretsId>my-translation-app-api-001</UserSecretsId>` to `Api.csproj`.
- Real credentials documented in README using `dotnet user-secrets set`.

### Step 8 — Tests

- 15 new tests in `ProviderSelectionTests.cs`:
  - Mock provider starts without any Azure config.
  - Mock provider registers correct types.
  - Default config (no override) resolves to mock.
  - Azure provider: missing key/region throws at startup.
  - Azure provider: placeholder values pass startup (auth fails at first call, not startup).
  - Azure provider: valid fake credentials register Azure provider types.
  - Provider name is case-insensitive.
  - Unknown provider name throws at startup.

---

## Known Limitations

- **Audio duration enforcement** remains deferred (R-016). File size limit only.
- **WebM audio**: Azure Speech SDK does not natively support WebM containers. The provider maps `audio/webm` to `OGG_OPUS` container format as best-effort. This may fail for WebM audio from Chrome. MP3 or WAV files are recommended for manual live validation.
- **MP4 audio**: not supported by Azure Speech SDK. Returns `UNSUPPORTED_AUDIO_FORMAT` via `ProviderException`.
- **Language auto-detect in Speech**: if `sourceLanguage` is null or "auto", defaults to `en-US`. Full auto-detect language support deferred.
- **Startup validation**: only guards against empty/missing Key and Region. Does not validate that credentials are real or that the Azure resource exists. Invalid but non-empty credentials cause auth failures on the first API call.
