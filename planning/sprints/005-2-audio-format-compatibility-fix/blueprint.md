# Sprint 005.2 Blueprint — Audio Format Compatibility Fix

**Project:** My Translation App
**Sprint:** 005.2
**Date:** 2026-05-30
**Status:** Complete

---

## Problem

The Sprint 005 `AzureSpeechToTextProvider` used `AudioInputStream.CreatePushStream` with compressed `AudioStreamContainerFormat` values for MP3 and WebM. On Windows, this path requires GStreamer (a native media framework) to decode the compressed audio. Without GStreamer, this threw `ApplicationException` before recognition began, producing `PROVIDER_ERROR` for all compressed uploads.

WAV was unaffected because it used `AudioConfig.FromWavFileInput`, which the SDK handles natively.

This would have blocked Sprint 006 push-to-talk: Chrome MediaRecorder commonly outputs `audio/webm;codecs=opus`.

---

## Documentation Verification (Pre-Implementation)

Before implementation, the Azure Speech REST API documentation was fetched from Microsoft Learn. Two deviations from the initial dry run assumptions were found:

1. **Short-audio REST endpoint** (`/speech/recognition/conversation/cognitiveservices/v1`) only accepts WAV and OGG/Opus. MP3 and WebM are not supported.
2. **Endpoint URL format** uses the resource name (`{resourceName}.cognitiveservices.azure.com`), not the region.

The **Fast Transcription API** (`/speechtotext/transcriptions:transcribe?api-version=2025-10-15`) supports all required formats (WAV, MP3, WebM, OGG, FLAC, and more) without GStreamer.

---

## Chosen Approach

- **WAV**: keep on the existing Speech SDK path (`AudioConfig.FromWavFileInput`). No change.
- **MP3, WebM, OGG**: route to the Azure Speech Fast Transcription REST API.
- **MP4**: continues to be rejected with `UNSUPPORTED_AUDIO_FORMAT`.

---

## Implementation Steps

### Step 1 — AzureSpeechOptions: add Endpoint

Added `Endpoint` property to `AzureSpeechOptions`:
```
AzureSpeech:Endpoint = https://your-resource.cognitiveservices.azure.com
```

Updated `IsConfigured` to require `Key`, `Region`, AND `Endpoint`. Region is still used by the WAV SDK path. Endpoint is used by the REST path.

### Step 2 — AzureSpeechToTextProvider: add REST path

- Added `IHttpClientFactory` constructor injection (no Scoped/Singleton leaks).
- Added `RecognizeFromFastTranscriptionAsync` — builds multipart/form-data request, POSTs to Fast Transcription endpoint, parses `combinedPhrases[0].text`.
- Updated `TranscribeAsync` branching: `audio/wav` → SDK path; anything else → `ValidateSupportedCompressedFormat` + REST path.
- Removed the broken `RecognizeFromCompressedBytesAsync` and `MapToContainerFormat` (GStreamer-dependent).

### Step 3 — TranslationValidationOptions: add audio/ogg

Added `"audio/ogg"` to `AllowedAudioMimeTypes`. Previously the provider had a code path for OGG but it was silently rejected at validation. Now Firefox MediaRecorder output (`audio/ogg`) passes validation.

### Step 4 — Infrastructure project: add Microsoft.Extensions.Http

Added `Microsoft.Extensions.Http 8.0.0` package to `MyTranslationApp.Infrastructure.csproj` to provide `IHttpClientFactory`.

### Step 5 — Program.cs: register HttpClient

Added `builder.Services.AddHttpClient()` before application services. Updated the Azure Speech startup error message to mention `AzureSpeech:Endpoint`.

### Step 6 — Tests

- Updated `ProviderSelectionTests`: added `AzureSpeech:Endpoint` to all 6 Azure-mode tests that set credentials (required now that `IsConfigured` checks all three fields).
- Updated `TranslationValidationTests`: added `audio/ogg` to the `AllAcceptedMimeTypes_AreAllowed` parameterized list.
- Added `AzureSpeechProviderRoutingTests` (16 new tests):
  - Routing: WAV → SDK (no HTTP call), MP3/WebM/OGG → REST (HTTP call).
  - Normalisation: `audio/webm;codecs=opus` normalised to `audio/webm`.
  - Request shape: subscription key header, URL contains endpoint + API version, definition JSON contains locale.
  - Response parsing: success returns `combinedPhrases[0].text`, empty array → `TRANSCRIPTION_FAILED`, blank text → `TRANSCRIPTION_FAILED`.
  - Error mapping: HTTP 401 → `PROVIDER_ERROR`, HTTP 400 → `PROVIDER_ERROR`, network failure → `PROVIDER_ERROR`.
  - Unsupported format: `audio/mp4` → `UNSUPPORTED_AUDIO_FORMAT`, no HTTP call made.
  - Startup: `AzureProvider_MissingSpeechEndpoint_ThrowsOnStartup`.

---

## Fast Transcription API Details

| Field | Value |
|---|---|
| Endpoint | `{AzureSpeech:Endpoint}/speechtotext/transcriptions:transcribe?api-version=2025-10-15` |
| Method | POST |
| Content-Type | multipart/form-data |
| Auth header | `Ocp-Apim-Subscription-Key: {AzureSpeech:Key}` |
| Audio part name | `audio` |
| Audio Content-Type | normalised MIME type (e.g. `audio/mpeg`, `audio/webm`, `audio/ogg`) |
| Definition part | `{"locales":["en-US"]}` (BCP-47 from `ResolveLanguage`) |
| Success response | `combinedPhrases[0].text` |
| No-speech response | empty `combinedPhrases` array or blank `text` → `TRANSCRIPTION_FAILED` |
| HTTP error response | any non-2xx → `PROVIDER_ERROR` |
| Max audio duration | 5 hours (well within MVP 60-second limit) |
| Max file size | 500 MB (well within MVP 10 MB limit) |

---

## Known Limitations Carried Forward

- Audio duration enforcement remains deferred (R-016, Q-015).
- Language auto-detect for speech defaults to `en-US` when no source language supplied.
- `audio/mp4` remains unsupported — `UNSUPPORTED_AUDIO_FORMAT` returned.
- Live validation with real audio files was not possible in this session (no credentials in environment). Manual validation instructions are in `src/backend/README.md`.
