# Sprint 007 Requirements — Text-to-Speech Playback

## Goal

Allow users to play translated text aloud using Azure Speech Text-to-Speech.

## Problem Being Solved

The app can currently translate typed text, uploaded audio, and push-to-talk audio, but users cannot hear the translated result.

For real-world communication, the translated phrase should be playable aloud so the user can speak through the app.

## In Scope

### Backend

- Implement Azure text-to-speech provider.
- Reuse existing Azure Speech configuration (AzureSpeech:Key, AzureSpeech:Region).
- Add TTS application service.
- Add POST /api/translate/tts endpoint.
- Return browser-playable audio (MP3 in Azure mode, silent WAV in Mock mode).
- Preserve mock provider behavior.
- Add validation for empty text and empty language.
- Add tests for provider selection, endpoint behavior, and error mapping.

### Frontend

- Add Play button to ResultPanel for translated text.
- Support playback for both text translation and audio translation results.
- Show loading state while speech audio is being generated.
- Show playback error state if TTS fails.
- Clean up generated object URLs.
- Prevent duplicate overlapping playback.

## Out Of Scope

- Automatic playback after translation.
- Voice selection UI.
- User accounts or saved audio.
- Speech audio caching.
- Streaming TTS.
- Conversation mode.
- Advanced voice styles, tone, or gender controls.
- Custom pronunciation dictionaries.

## Constraints

- No secrets in source control.
- Use existing Azure Speech resource and user secrets.
- Follow existing provider abstraction pattern.
- Do not break existing text/audio translation flows.
- Keep the MVP simple.

## Assumptions

- Existing Azure Speech resource supports TTS.
- Existing AzureSpeech:Key and AzureSpeech:Region are sufficient.
- MVP language set can use a voice map with a language-based fallback.
- Browser playback is initiated by a user click, avoiding autoplay restrictions.
