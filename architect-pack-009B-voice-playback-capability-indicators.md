# Architect Pack: Sprint 009B - Voice Playback & Language Capability Indicators

## Pack Metadata

| Field | Value |
|---|---|
| Project Name | My Translation App |
| Client / Owner | Mark McLachlan |
| Project Slug | my-translation-app |
| Sprint Number | 009B |
| Sprint Name | Voice Playback & Language Capability Indicators |
| Created Date | 2026-05-31 |
| Architect | ChatGPT Architect Layer |
| Builder Target | Claude Code / Codex / Cursor / Other |
| Status | Ready For Builder |

---

## 1. Project Context

My Translation App is a web-based text and audio translation platform. The app currently supports text translation, speech-to-text, text-to-speech provider infrastructure, a modern responsive frontend, and an expanded 37-language catalog from Sprint 009A.

Sprint 009A completed successfully with:

- 37 supported languages
- backend catalog as the source of truth
- STT and TTS provider maps expanded
- 115 / 115 automated tests passing
- clean build with 0 warnings and 0 errors
- planning and documentation updated

Sprint 009B builds directly on Sprint 009A by exposing language capability metadata and allowing users to play translated text as audio from the UI.

---

## 2. Sprint Goal

Add browser-based playback for translated text and expose language capability indicators so users can understand which languages support text translation, speech-to-text, and text-to-speech.

The user-facing outcome is that after translation, users can click a play button to hear the translated result spoken aloud where TTS is supported.

---

## 3. Problem Being Solved

The app currently supports backend TTS infrastructure, but the UI does not clearly expose voice playback as part of the translation workflow. Users can translate text or audio, but cannot yet easily hear the translated result.

The expanded 37-language catalog also creates a need to show capability information clearly. Future provider changes may mean not every language supports every capability, so the API and UI should avoid assuming all language features are always available.

---

## 4. Requirements

| ID | Requirement | Priority | Notes |
|---|---|---|---|
| R-001 | Add language capability metadata to language API responses | Must | Include text translation, STT, and TTS support flags |
| R-002 | Preserve existing `code` and `name` language response fields | Must | Avoid breaking frontend consumers |
| R-003 | Add translated-text playback from frontend | Must | User can hear the translated output |
| R-004 | Reuse existing backend TTS provider/service where available | Must | Do not introduce a new provider |
| R-005 | Add or expose a backend endpoint for TTS playback if one does not already exist | Must | Use existing architecture and DTO patterns |
| R-006 | Disable or hide playback when translated text is empty | Must | Prevent invalid requests |
| R-007 | Disable playback when selected target language does not support TTS | Must | Use capability metadata |
| R-008 | Show language capability indicators in the UI | Should | At minimum show TTS availability clearly |
| R-009 | Add loading and error states for playback | Must | No silent failures |
| R-010 | Preserve responsive/mobile UI from Sprint 008 | Must | No layout regression |
| R-011 | Add backend and frontend tests where practical | Must | Protect API and UI behaviour |
| R-012 | Update docs and planning files | Must | Keep folder as source of truth |

---

## 5. In Scope

The Builder may work on:

- Language DTO / API response expansion
- Language capability model or metadata source
- Backend TTS endpoint if missing
- Frontend playback button in the translation result area
- Frontend capability indicators or badges
- Loading, disabled, and error states for playback
- Tests for language capability metadata
- Tests for TTS endpoint behavior where feasible
- Frontend tests for playback UI where feasible
- Documentation and planning updates

---

## 6. Out Of Scope

The Builder must not work on:

- New translation providers
- Replacing Azure provider implementation
- Real-time streaming audio
- Conversation mode
- Auto-detect language improvements
- User accounts or persistence
- Deployment configuration
- Major UI redesign beyond playback/capability indicators
- Changing the 37-language catalog unless required by a discovered bug
- Storing Azure keys or credentials in the repo

---

## 7. Assumptions

| ID | Assumption | Confidence | Action If Wrong |
|---|---|---|---|
| A-001 | Backend already has a TTS provider or service from earlier sprints | High | If missing, create a minimal endpoint/service wrapper around existing provider abstraction only |
| A-002 | Frontend translation result already includes target language code or selected target language state | High | If not, pass selected target language into playback request from the existing form state |
| A-003 | The 37-language catalog remains the source of truth | High | Do not duplicate catalog in frontend |
| A-004 | For Sprint 009B, all 37 languages can initially be marked as supporting text translation, STT, and TTS based on Sprint 009A mappings | Medium | If implementation discovers unsupported entries, document and mark false instead of forcing support |
| A-005 | Live Azure validation requires credentials and may not be runnable in local tests | High | Keep live validation manual and document checklist |

---

## 8. Constraints

- Preserve existing API compatibility where possible.
- Do not remove or rename existing language `code` and `name` fields.
- Do not hardcode language lists in frontend components.
- Do not add secrets, keys, or credentials.
- Use existing backend architecture and provider abstractions.
- Keep changes small and traceable.
- Keep UI responsive and accessible.
- Add graceful handling for provider failures.
- Do not block the whole app if TTS playback fails.

---

## 9. Dependencies

| Dependency | Type | Status | Notes |
|---|---|---|---|
| Sprint 009A language catalog | Project state | Available | 37 languages completed |
| Azure TTS provider | Provider | Available / verify | Builder must inspect current implementation |
| Frontend result panel | UI component | Available / verify | Likely target for Play button |
| Existing API DTOs | Backend contract | Available | Extend carefully |
| Azure credentials | External service | Not guaranteed | Required only for live validation |

---

## 10. Files To Read First

The Builder must read these before doing work:

1. `AGENTS.md`
2. `planning/STATE.md`
3. `planning/DECISIONS.md`
4. `planning/DOMAIN.md`
5. `planning/RISKS.md`
6. `planning/QUESTIONS.md`
7. `docs/API.md`
8. `docs/ARCHITECTURE.md`
9. `docs/VALIDATION.md`
10. `planning/sprints/009A-language-catalog-expansion/requirements.md`
11. `planning/sprints/009A-language-catalog-expansion/blueprint.md`
12. `planning/sprints/009A-language-catalog-expansion/acceptance.md`
13. `src/backend/MyTranslationApp.Infrastructure/Providers/StaticLanguageCatalogService.cs`
14. `src/backend/MyTranslationApp.Infrastructure/Providers/AzureTextToSpeechProvider.cs`
15. Frontend result/display components, especially `ResultPanel.tsx` or equivalent
16. Current API controllers related to languages, translation, and speech/TTS

---

## 11. Files To Create Or Modify

Exact paths may vary depending on the current repository structure. Builder must inspect first.

| Path | Action | Purpose |
|---|---|---|
| `planning/STATE.md` | Modify | Mark Sprint 009B active/completed as appropriate |
| `planning/DECISIONS.md` | Modify if needed | Record durable decisions |
| `planning/RISKS.md` | Modify if needed | Add capability/playback risks |
| `planning/QUESTIONS.md` | Modify if needed | Resolve or add questions |
| `planning/FILE_INVENTORY.md` | Modify if needed | Record new sprint files/tests |
| `docs/API.md` | Modify | Document updated language response and playback endpoint |
| `docs/ARCHITECTURE.md` | Modify | Document capability metadata and playback flow |
| `docs/VALIDATION.md` | Modify | Add validation checklist for playback and capabilities |
| `planning/sprints/009B-voice-playback-capability-indicators/requirements.md` | Create | Sprint requirements |
| `planning/sprints/009B-voice-playback-capability-indicators/blueprint.md` | Create | Sprint implementation blueprint |
| `planning/sprints/009B-voice-playback-capability-indicators/acceptance.md` | Create | Sprint acceptance checklist |
| `planning/sprints/009B-voice-playback-capability-indicators/handoff-prompt.md` | Create | Builder handoff prompt |
| Backend language DTO/model files | Modify | Add capability flags |
| Backend language service/catalog files | Modify | Provide capability metadata |
| Backend TTS controller/endpoint files | Create/Modify | Expose playback audio if missing |
| Backend tests | Create/Modify | Validate capability metadata and TTS endpoint |
| Frontend API client files | Modify | Consume capability fields and playback endpoint |
| Frontend result panel/component files | Modify | Add Play Audio button |
| Frontend language selector files | Modify | Add capability indicators if appropriate |
| Frontend tests | Create/Modify | Validate playback UI states where feasible |

---

## 12. Blueprint

### Step 1: Inspect Current TTS And Language Architecture

- Locate the language API endpoint and DTO.
- Locate `StaticLanguageCatalogService` and current `LanguageOptionDto` or equivalent.
- Locate current TTS provider/service interfaces.
- Locate any existing controller endpoint for speech synthesis.
- Locate frontend API client and result display components.

Builder must not assume endpoint names before inspection.

### Step 2: Extend Language Capability Metadata

Extend the language response shape while preserving existing fields.

Preferred response shape:

```json
{
  "code": "cs",
  "name": "Czech",
  "supportsTextTranslation": true,
  "supportsSpeechToText": true,
  "supportsTextToSpeech": true
}
```

Rules:

- `code` and `name` must remain unchanged.
- Capability flags should be booleans.
- Initial values may be true for all 37 languages if supported by Sprint 009A maps.
- If capability data already exists elsewhere, reuse it rather than duplicating logic.

### Step 3: Add Or Confirm TTS Playback Endpoint

If an endpoint already exists, reuse and document it.

If missing, add a minimal endpoint following existing API patterns.

Preferred endpoint shape:

```text
POST /api/speech/synthesize
```

Preferred request:

```json
{
  "text": "Bonjour",
  "languageCode": "fr"
}
```

Preferred response options:

Option A, preferred if existing architecture supports it:

- `audio/mpeg` or equivalent binary audio stream

Option B, acceptable if easier with current architecture:

```json
{
  "audioContentBase64": "...",
  "contentType": "audio/mpeg",
  "correlationId": "..."
}
```

The Builder should choose the option that best fits existing code and tests, then document the decision.

### Step 4: Add Frontend Playback UX

Add a Play Audio button to the translation result area.

Required states:

- hidden or disabled when no translated text exists
- disabled when target language lacks TTS support
- idle state
- loading/synthesizing state
- playing state if practical
- error state if synthesis fails

Accessibility requirements:

- button must have clear text or `aria-label`
- loading state should be announced or visually clear
- error message should be readable and not only color-based
- keyboard users must be able to operate playback

### Step 5: Add Capability Indicators

Add capability indicators to language selection UI where practical.

Minimum acceptable implementation:

- TTS support is shown for target language or result area

Preferred implementation:

- Language options indicate support for text translation, STT, and TTS
- Badges or small labels are responsive and do not clutter mobile UI

Examples:

```text
Czech · Text · Mic · Voice
German (Austria) · Text · Mic · Voice
```

Use accessible text, not icons alone.

### Step 6: Add Tests

Backend tests should verify:

- language response includes capability fields
- all 37 languages include capability metadata
- required languages have TTS capability true where mapped
- TTS request validation rejects empty text
- TTS request validation rejects empty language code
- endpoint returns an expected mocked response if provider is mockable

Frontend tests should verify where existing test tooling supports it:

- Play button appears when translation exists
- Play button is disabled when no translation exists
- Play button shows loading state during playback request
- error state appears on failed playback request
- capability indicator renders without breaking selector

### Step 7: Update Documentation And Planning

Update docs and sprint files to reflect:

- language capability metadata
- playback endpoint contract
- frontend playback behavior
- manual Azure validation checklist
- risks and open questions

---

## 13. Data Flow / Logic Flow

### Capability Metadata Flow

```text
StaticLanguageCatalogService
    -> Language DTO with capability flags
    -> GET /api/languages
    -> Frontend language selectors
    -> Capability indicators and playback enablement
```

### Playback Flow

```text
User translates text
    -> Translation result displayed
    -> User clicks Play Audio
    -> Frontend sends translated text + target language code
    -> Backend validates request
    -> Backend calls existing TTS provider
    -> Backend returns audio
    -> Frontend plays audio in browser
    -> UI shows success/error state
```

---

## 14. UI / UX Notes

The playback feature should be simple and demo-friendly.

Required UI behaviour:

- Add a clear `Play Audio` or `Listen` button near the translated result.
- Do not make playback the primary CTA; translation remains primary.
- Disable playback until a translated result exists.
- Show a short message when playback is unavailable.
- Preserve Sprint 008 responsive/mobile layout.
- Avoid crowding the language dropdown on small screens.

Suggested labels:

- `Play Audio`
- `Playing...`
- `Audio unavailable for this language`
- `Could not play audio. Please try again.`

---

## 15. API / Integration Notes

### Updated `GET /api/languages`

Existing fields must remain:

```json
{
  "code": "en",
  "name": "English"
}
```

Add fields:

```json
{
  "code": "en",
  "name": "English",
  "supportsTextTranslation": true,
  "supportsSpeechToText": true,
  "supportsTextToSpeech": true
}
```

### TTS Synthesis Endpoint

Use existing project naming conventions. If none exists, preferred endpoint:

```text
POST /api/speech/synthesize
```

Validation:

- text required
- languageCode required
- text length should respect existing validation conventions
- unsupported language should return a clear validation error
- provider failure should return a safe error response with correlation ID if existing pattern supports it

Security:

- no credentials in code
- no raw provider exceptions exposed to client
- request body size should remain reasonable

---

## 16. Validation Plan

| Check | Method | Expected Result |
|---|---|---|
| Backend build | `dotnet build --configuration Release` | Build succeeds with 0 errors |
| Backend tests | `dotnet test --configuration Release` | All tests pass |
| Language API compatibility | API/controller tests | Existing `code` and `name` still present |
| Capability metadata | New tests | All 37 languages have capability fields |
| TTS validation | New tests | Empty text/language rejected |
| TTS playback | Manual or mocked test | Translated text can be synthesized |
| Frontend build | Existing frontend build command | Build succeeds |
| Frontend tests | Existing frontend test command if present | Tests pass |
| Responsive UI | Manual check | Playback controls usable on mobile width |
| Accessibility | Manual check | Button has accessible label and visible states |
| Live Azure validation | Manual with credentials | Selected languages synthesize audio successfully |

---

## 17. Acceptance Criteria

Sprint is complete when:

- [ ] `GET /api/languages` preserves `code` and `name` fields.
- [ ] `GET /api/languages` includes capability flags for text translation, STT, and TTS.
- [ ] All 37 Sprint 009A languages include capability metadata.
- [ ] A playback path exists for translated text.
- [ ] User can click a Play Audio / Listen button after translation.
- [ ] Playback sends translated text and target language code to backend.
- [ ] Playback is disabled or unavailable when translated text is empty.
- [ ] Playback is disabled or unavailable when TTS is unsupported.
- [ ] Playback loading and error states are visible.
- [ ] Capability indicators are shown in the UI where practical.
- [ ] Backend validation handles invalid TTS requests safely.
- [ ] Existing translation workflow is not broken.
- [ ] Existing language selector behavior is not broken.
- [ ] Responsive/mobile layout remains usable.
- [ ] Backend tests pass.
- [ ] Frontend build/tests pass where available.
- [ ] `docs/API.md` documents updated language response and playback endpoint.
- [ ] `docs/ARCHITECTURE.md` documents capability and playback flow.
- [ ] `docs/VALIDATION.md` documents automated and manual validation.
- [ ] `planning/STATE.md` is updated.
- [ ] New decisions are added to `planning/DECISIONS.md` if needed.
- [ ] New risks are added to `planning/RISKS.md` if needed.
- [ ] Open questions are added or resolved in `planning/QUESTIONS.md`.
- [ ] No secrets, credentials, private tokens, or unsafe sensitive data are added.

---

## 18. Risks

| Risk | Impact | Likelihood | Mitigation | Owner |
|---|---|---|---|---|
| Azure voice availability differs by region/SKU | Medium | Medium | Add capability flags and document live validation | Builder |
| Browser audio playback fails due to format handling | Medium | Medium | Use standard audio content type and object URL/base64 handling | Builder |
| API response change breaks frontend assumptions | Medium | Low | Preserve `code` and `name`; add fields only | Builder |
| Capability badges clutter mobile UI | Low | Medium | Use compact badges or result-area messaging | Builder |
| Provider errors leak details | High | Low | Use existing safe error handling and correlation IDs | Builder |
| Large text synthesis creates slow UX | Medium | Medium | Add loading state and respect validation limits | Builder |

---

## 19. Open Questions

| Question | Needed From | Blocking? | Notes |
|---|---|---|---|
| Q-037: Should capability badges appear inside every dropdown option or only near selected language/result area? | Architect / Builder | No | Builder may choose least disruptive UI |
| Q-038: Should playback return binary audio or base64 JSON? | Builder | No | Choose based on existing architecture |
| Q-039: Should live Azure validation be run during this sprint if credentials are available? | Project Owner | No | Manual checklist acceptable if credentials unavailable |
| Q-040: Should all 37 languages be marked fully capable until live validation proves otherwise? | Architect | No | Recommended yes for 009B, with documented caveat |

---

## 20. State Updates Required

At the end of the sprint, update:

- `planning/STATE.md`
- `planning/DECISIONS.md` if decisions were made
- `planning/RISKS.md` if risks changed
- `planning/QUESTIONS.md` if questions were opened or answered
- `planning/FILE_INVENTORY.md` if files were added
- active sprint acceptance status

Suggested decisions if needed:

- D-074: Language API responses include capability metadata.
- D-075: Translated text playback uses the existing backend TTS provider path.
- D-076: Playback errors are surfaced safely in the UI without exposing provider internals.
- D-077: Live Azure voice validation remains manual unless credentials are available in the environment.

---

## 21. Builder Dry Run Instructions

Before implementation, the Builder must produce a dry run summary.

The dry run must include:

- files read
- sprint understanding
- current TTS endpoint/provider status
- current frontend result component structure
- planned file changes
- proposed API contract for playback
- proposed UI placement for Play Audio
- commands to run
- assumptions
- risks or ambiguities
- whether implementation is safe to start

Do not implement until the dry run has been reviewed if approval is required.

---

## 22. Builder Handoff Prompt

Copy this prompt into the Builder chat.

```markdown
You are the Builder for My Translation App.

You are working on Sprint 009B: Voice Playback & Language Capability Indicators.

Follow the Architect / Builder methodology.

Read these files first:

1. AGENTS.md
2. planning/STATE.md
3. planning/DECISIONS.md
4. planning/DOMAIN.md
5. planning/RISKS.md
6. planning/QUESTIONS.md
7. docs/API.md
8. docs/ARCHITECTURE.md
9. docs/VALIDATION.md
10. planning/sprints/009A-language-catalog-expansion/requirements.md
11. planning/sprints/009A-language-catalog-expansion/blueprint.md
12. planning/sprints/009A-language-catalog-expansion/acceptance.md
13. src/backend/MyTranslationApp.Infrastructure/Providers/StaticLanguageCatalogService.cs
14. src/backend/MyTranslationApp.Infrastructure/Providers/AzureTextToSpeechProvider.cs
15. Current API controllers related to languages, translation, and speech/TTS
16. Current frontend result/display components, especially ResultPanel.tsx or equivalent

Your task:

Add voice playback for translated text and expose language capability indicators.

Core requirements:

- Preserve existing language API fields: code and name.
- Add capability fields to language responses:
  - supportsTextTranslation
  - supportsSpeechToText
  - supportsTextToSpeech
- Ensure all 37 Sprint 009A languages include capability metadata.
- Add or reuse a backend TTS playback endpoint.
- Add a frontend Play Audio / Listen button near the translated result.
- Disable playback when translated text is empty.
- Disable playback when selected target language does not support TTS.
- Show loading and error states for playback.
- Add capability indicators in the UI where practical.
- Preserve existing translation workflow and responsive UI.
- Do not introduce new providers.
- Do not store secrets or credentials.

Before implementation, perform a dry run only.

Your dry run must include:

1. Files read
2. Current TTS endpoint/provider status
3. Current frontend result component structure
4. Proposed backend API contract for playback
5. Proposed language capability DTO shape
6. Proposed UI placement for Play Audio and capability indicators
7. Files expected to change
8. Tests expected to add/update
9. Commands expected to run
10. Risks, assumptions, or ambiguities
11. Whether implementation is safe to start

Stop after the dry run and wait for approval.
```

---

## 23. Completion Report Template

When finished, the Builder should report:

```markdown
# Sprint 009B Completion Report

## Summary

## Files Created

## Files Modified

## Commands Run

## Tests / Validation

## Acceptance Criteria Status

## Decisions Added

## Risks Added Or Updated

## Questions Added Or Resolved

## Known Limitations

## Recommended Next Sprint
```
