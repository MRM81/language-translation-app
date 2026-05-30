# Sprint 009B Blueprint — Language Capability Metadata & Capability-Aware UI

## Files Modified

| File | Change |
|---|---|
| `LanguageOptionDto.cs` | Added `SupportsTextTranslation`, `SupportsSpeechToText`, `SupportsTextToSpeech` (bool, default `true`) |
| `src/frontend/src/types/api.ts` | Added 3 capability fields to `LanguageOption` interface |
| `src/frontend/src/App.tsx` | Derives `targetLangSupportsTts` from `languages` + `result.data.targetLanguage`; passes to `ResultPanel` |
| `src/frontend/src/components/ResultPanel.tsx` | Accepts `targetLangSupportsTts?: boolean` prop; disables Play button with "Audio unavailable" when `false` |

## Files Created

| File | Notes |
|---|---|
| `tests/.../Controllers/LanguageCapabilityTests.cs` | 14 integration tests via `GET /api/languages` |

## Key Design Decisions

### DTO Defaults, Not Service Changes

`StaticLanguageCatalogService` is unchanged. The three new bool fields default to `true` in `LanguageOptionDto`. All 37 current languages serialize with `supportsTextTranslation: true`, `supportsSpeechToText: true`, `supportsTextToSpeech: true` automatically.

When a language that lacks one capability is added in a future sprint, the service entry for that language would explicitly set the relevant flag to `false`.

### No New Endpoint

`POST /api/translate/tts` (D-060) is the canonical TTS path. The Architect Pack's suggested `POST /api/speech/synthesize` was rejected. No controller changes.

### `targetLangSupportsTts` Threading

`App.tsx` lookup:
```tsx
targetLangSupportsTts={
  result
    ? (languages.find(l => l.code === result.data.targetLanguage)?.supportsTextToSpeech ?? true)
    : true
}
```

The `?? true` fallback ensures the Play button defaults to enabled if the language is not found in the list (e.g., during a race condition at load time). This is the safe default.

### Capability Badges in `<select>` Deferred

Native HTML `<option>` elements do not support nested HTML. Appending capability text to every option name would clutter the mobile dropdown (Q-037 resolved: deferred to a future custom dropdown sprint).

### ResultPanel Play Button States

| Condition | Button state | Label |
|---|---|---|
| No result | — (button not rendered) | — |
| Result, TTS supported, idle | enabled | `▶ Play` |
| Result, TTS supported, loading | disabled | `Loading…` |
| Result, TTS supported, playing | disabled | `◼ Playing…` |
| Result, TTS supported, error | enabled | `▶ Play` + error message |
| Result, TTS not supported | disabled | `Audio unavailable` |
