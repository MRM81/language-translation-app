# Sprint 007 Blueprint — Text-to-Speech Playback

## Dry Run Results

Dry run completed and approved by Architect on 2026-05-30.

Approved with two adjustments:
1. Fallback voice strategy for unmapped languages (use Azure default for locale rather than throwing).
2. MockTextToSpeechProvider returns a minimal valid silent WAV fixture instead of empty bytes.

## Architecture

```
Frontend ResultPanel
→ Play button (user click)
→ synthesizeSpeech(text, language) → POST /api/translate/tts
→ TranslationController [HttpPost("tts")]
→ TextToSpeechService
→ ITextToSpeechProvider
→ AzureTextToSpeechProvider (Azure mode) / MockTextToSpeechProvider (Mock mode)
→ Azure Speech SDK SpeechSynthesizer
→ audio bytes returned as File(audioData, contentType)
→ Frontend creates object URL → HTMLAudioElement → plays
```

## Voice Mapping Strategy

Primary map (10 supported languages):
- en → en-US-JennyNeural
- es → es-ES-ElviraNeural
- fr → fr-FR-DeniseNeural
- de → de-DE-KatjaNeural
- zh → zh-CN-XiaoxiaoNeural
- ja → ja-JP-NanamiNeural
- ar → ar-SA-ZariyahNeural
- pt → pt-BR-FranciscaNeural
- it → it-IT-ElsaNeural
- ru → ru-RU-SvetlanaNeural

Fallback: if language not in map, resolve short code to locale via LocaleMap, then use
`SpeechConfig.SpeechSynthesisLanguage` and let Azure select its default voice. If Azure
cannot resolve the language, the SDK returns a cancellation result which maps to ProviderException.

## Audio Format

Azure mode: MP3 via `SpeechSynthesisOutputFormat.Audio24Khz96KBitRateMonoMp3` → Content-Type: audio/mpeg
Mock mode: Minimal silent WAV (44 bytes, 0 samples) → Content-Type: audio/wav

## Correlation ID

Returned in `X-Correlation-ID` response header (binary response body cannot carry JSON field).

## Files Created

- src/backend/MyTranslationApp.Application/DTOs/TtsSynthesisRequestDto.cs
- src/backend/MyTranslationApp.Application/Services/TextToSpeechService.cs
- src/backend/MyTranslationApp.Infrastructure/Providers/Azure/AzureTextToSpeechProvider.cs
- tests/backend/MyTranslationApp.Tests/Controllers/TtsControllerTests.cs
- planning/sprints/007-text-to-speech-playback/requirements.md (this file)
- planning/sprints/007-text-to-speech-playback/blueprint.md (this file)
- planning/sprints/007-text-to-speech-playback/acceptance.md

## Files Modified

- src/backend/MyTranslationApp.Infrastructure/Providers/MockTextToSpeechProvider.cs
- src/backend/MyTranslationApp.Application/Validation/TranslationRequestValidator.cs
- src/backend/MyTranslationApp.Api/Controllers/TranslationController.cs
- src/backend/MyTranslationApp.Api/Program.cs
- tests/backend/MyTranslationApp.Tests/Providers/ProviderSelectionTests.cs
- src/frontend/src/api/translationApi.ts
- src/frontend/src/components/ResultPanel.tsx
- src/frontend/src/styles/app.css
- planning/STATE.md
- planning/DECISIONS.md
- planning/RISKS.md
- planning/QUESTIONS.md
- docs/API.md
- docs/ARCHITECTURE.md
- docs/VALIDATION.md
