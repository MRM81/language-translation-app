# Sprint 009B Requirements — Language Capability Metadata & Capability-Aware UI

## Scope Correction (Recorded)

The Architect Pack originally titled this sprint "Voice Playback & Language Capability Indicators."

During the dry run, the Builder discovered that voice playback (TTS Play button, `POST /api/translate/tts`, `synthesizeSpeech()` API client, full idle/loading/playing/error states) was already fully implemented in Sprint 007.

The sprint was corrected to:

**Language Capability Metadata + Capability-Aware UI**

Voice playback is not rebuilt. The existing TTS endpoint and ResultPanel Play button are preserved.

## Goal

Expose language capability metadata (supportsTextTranslation, supportsSpeechToText, supportsTextToSpeech) from the backend API and use it in the frontend to conditionally enable or disable the Play button.

## In Scope

- Add `SupportsTextTranslation`, `SupportsSpeechToText`, `SupportsTextToSpeech` boolean fields to `LanguageOptionDto`
- All 37 Sprint 009A languages default to `true` for all capabilities
- Update `LanguageOption` TypeScript interface to include the three capability fields
- `App.tsx` derives `targetLangSupportsTts` from the loaded languages list and passes it to `ResultPanel`
- `ResultPanel` accepts `targetLangSupportsTts` prop and disables the Play button with "Audio unavailable" label when `false`
- Integration tests via `GET /api/languages` verifying capability fields on all 37 languages
- Planning and docs updated

## Out Of Scope

- New TTS endpoint (`POST /api/speech/synthesize` rejected — canonical path is `POST /api/translate/tts`)
- Rebuilding voice playback (Sprint 007 already delivered it)
- Capability badges inside `<select>` `<option>` elements (native HTML limitation, deferred)
- Custom dropdown component
- Live Azure voice validation (credentials required — remains manual checklist)
- New providers, authentication, deployment

## Constraints

- Preserve existing `code` and `name` fields exactly
- `LanguageOptionDto` capability fields default to `true` — no `StaticLanguageCatalogService` changes required
- No secrets, credentials, or API keys
- No changes to `POST /api/translate/tts` endpoint or TTS workflow
