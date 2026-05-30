# Architect Pack 007 — Text-to-Speech Playback

## Pack Metadata

| Field | Value |
|---|---|
| Project Name | My Translation App |
| Sprint Number | 007 |
| Sprint Name | Text-to-Speech Playback |
| Created Date | 2026-05-30 |
| Architect | ChatGPT |
| Builder Target | Claude Code |
| Status | Ready For Builder |

---

## Project Context

My Translation App is a browser-based translation app that supports text translation, audio upload translation, and browser push-to-talk translation using Azure-backed providers.

Completed foundation:

- Sprint 003: Backend API skeleton and provider abstractions
- Sprint 004: Frontend MVP shell
- Sprint 005: Azure provider integration
- Sprint 005.1: Azure Translator SDK compatibility fix
- Sprint 005.2: Audio format compatibility fix using Azure Fast Transcription API
- Sprint 006: Browser audio capture UX / push-to-talk

Sprint 007 adds spoken playback of translated text so users can hear the translated result aloud.

This closes the MVP communication loop:

Speak or type
→ Translate
→ Play translated speech

---

# planning/STATE.md

## Sprint Status Update

Sprint 006: Complete

Sprint 007: Active

### Sprint 007 Objective

Add text-to-speech playback for translated results using the existing Azure Speech capability and provider abstraction model.

### Current Status

The application currently returns translated text for:

- text input
- uploaded audio
- push-to-talk audio

The application does not yet let the user play the translated result aloud.

### Next Step

Builder must perform a dry run before implementation.

---

# planning/DECISIONS.md

Add:

## D-058 — Text-to-Speech Playback Completes MVP Communication Loop

Text-to-speech playback is now included as Sprint 007 because the user workflow requires the translated result to be heard aloud.

Rationale:

- Users may need to communicate verbally with someone who does not read the target language.
- Text-only output is useful but incomplete for real-world in-person communication.
- Azure Speech is already configured and can support TTS without a new Azure resource.

Status: Accepted

---

## D-059 — TTS Will Be Requested On Demand

The app will not generate speech audio automatically for every translation.

Users must click a Play button to request speech generation.

Rationale:

- Avoids unnecessary Azure usage and cost.
- Keeps initial translation response fast.
- Gives users control over when audio is generated.

Status: Accepted

---

## D-060 — TTS Endpoint Returns Audio Stream Or Audio Blob

The backend should expose a dedicated TTS endpoint that accepts text and language and returns playable audio.

Rationale:

- Keeps translation endpoints focused.
- Avoids bloating existing translation response DTOs.
- Allows frontend to request playback only when needed.

Status: Accepted

---

# planning/RISKS.md

Add:

## R-031 — Azure Voice Selection May Vary By Language

Some target languages may require specific Azure voice names.

Impact: Medium  
Likelihood: Medium  
Mitigation: Use a safe default voice mapping for supported MVP languages and return clear errors when unsupported.

---

## R-032 — TTS Can Increase Azure Usage Costs

Each playback request consumes Azure Speech resources.

Impact: Low  
Likelihood: Medium  
Mitigation: Playback is user-initiated only. Do not auto-generate speech on every translation.

---

## R-033 — Browser Audio Playback May Be Blocked Unless User Initiated

Some browsers block autoplay.

Impact: Low  
Likelihood: Medium  
Mitigation: Playback is triggered by a user click on the Play button.

---

## R-034 — Audio Object URLs May Leak Memory If Not Revoked

Generated audio URLs in the frontend need cleanup.

Impact: Low  
Likelihood: Medium  
Mitigation: Revoke object URLs after playback or when a new playback request replaces the previous audio.

---

# planning/QUESTIONS.md

Add:

## Q-028 — Which Languages Need Explicit Voice Mapping?

Needed From: Architect / Builder  
Blocking: No  
Notes: MVP can support the existing language catalog with default voice mapping. Unsupported languages should fail gracefully.

---

## Q-029 — Should TTS Playback Use MP3 Or WAV?

Needed From: Builder  
Blocking: Yes before implementation  
Notes: Prefer compressed browser-friendly audio if supported by Azure Speech SDK and current frontend. Builder should confirm during dry run.

---

## Q-030 — Should The App Cache Generated Speech Audio?

Needed From: Future Sprint  
Blocking: No  
Notes: Out of scope for Sprint 007. Playback can regenerate audio on each click.

---

# docs/API.md

Update with a new endpoint.

## POST /api/translate/tts

Generates speech audio for translated text.

### Request

```json
{
  "text": "Hola, ¿cómo estás?",
  "language": "es"
}
```

### Response

Returns audio content suitable for browser playback.

Recommended response:

- Content-Type: audio/mpeg or audio/wav
- Body: binary audio stream

### Error Handling

Use existing error response conventions:

- VALIDATION_ERROR
- PROVIDER_ERROR
- UNSUPPORTED_LANGUAGE if applicable

### Notes

- Endpoint should require non-empty text.
- Endpoint should require supported language.
- Endpoint should not store generated audio.
- Endpoint should return a correlation ID using existing middleware / response conventions where practical.

---

# docs/ARCHITECTURE.md

Update Azure provider section.

## Text-to-Speech Provider

Sprint 007 adds text-to-speech playback.

Architecture:

Frontend ResultPanel
→ Play button
→ POST /api/translate/tts
→ TranslationController or new TtsController
→ Application service
→ ITextToSpeechProvider
→ AzureTextToSpeechProvider
→ Azure Speech
→ Audio stream returned to browser

Provider model:

- MockTextToSpeechProvider remains available for mock mode.
- AzureTextToSpeechProvider is used when Translation:Provider = Azure.
- Existing AzureSpeech:Key and AzureSpeech:Region are reused.

---

# docs/VALIDATION.md

Add Sprint 007 validation section.

## Sprint 007 Validation

Validation must confirm:

- Play button appears for translated results.
- TTS request succeeds for text translation results.
- TTS request succeeds for audio translation results.
- Returned audio plays in Chrome.
- Returned audio plays in Edge.
- Unsupported or empty text is rejected.
- Provider errors are displayed clearly.
- Object URLs are cleaned up.
- No secrets are exposed.

---

# planning/sprints/007-text-to-speech-playback/requirements.md

# Sprint 007 Requirements — Text-to-Speech Playback

## Goal

Allow users to play translated text aloud using Azure Speech Text-to-Speech.

## Problem Being Solved

The app can currently translate typed text, uploaded audio, and push-to-talk audio, but users cannot hear the translated result.

For real-world communication, the translated phrase should be playable aloud so the user can speak through the app.

## In Scope

### Backend

- Implement Azure text-to-speech provider.
- Reuse existing Azure Speech configuration where possible.
- Add or complete TTS application service flow.
- Add dedicated API endpoint for TTS playback.
- Return browser-playable audio.
- Preserve mock provider behavior.
- Add validation for empty text and unsupported language.
- Add tests for provider selection, validation, endpoint behavior, and error mapping.

### Frontend

- Add Play button to ResultPanel for translated text.
- Support playback for both text translation and audio translation results.
- Show loading state while speech audio is being generated.
- Show playback error state if TTS fails.
- Clean up generated object URLs.
- Prevent duplicate overlapping playback where practical.

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
- Do not introduce unnecessary new infrastructure.
- Keep the MVP simple.

## Assumptions

- Existing Azure Speech resource supports TTS.
- Existing AzureSpeech:Key and AzureSpeech:Region are sufficient.
- MVP language set can use simple voice mapping or service defaults.
- Browser playback will be initiated by a user click, avoiding autoplay restrictions.

---

# planning/sprints/007-text-to-speech-playback/blueprint.md

# Sprint 007 Blueprint — Text-to-Speech Playback

## Required Builder Dry Run

Before implementation, Builder must inspect:

- Existing ITextToSpeechProvider interface
- Existing MockTextToSpeechProvider
- Existing Program.cs provider registration
- Existing application services
- Existing TranslationController and service structure
- Existing ResultPanel props and rendering
- Azure Speech SDK TTS API requirements
- Current language catalog

Builder must report:

1. Existing TTS-related code already present.
2. Exact backend files to modify or create.
3. Exact frontend files to modify or create.
4. Whether the current ITextToSpeechProvider interface is sufficient.
5. Recommended audio format: MP3 or WAV.
6. Voice mapping strategy for MVP languages.
7. Test plan.
8. Risks or blockers.

Do not implement until dry run is approved.

---

## Backend Approach

### Step 1 — Inspect Existing Provider Abstraction

Review:

- ITextToSpeechProvider
- MockTextToSpeechProvider
- Program.cs DI provider switching
- Existing application services

Confirm whether the abstraction already supports:

- input text
- language code
- cancellation token
- audio result type

If insufficient, propose minimal interface adjustment during dry run.

---

### Step 2 — Implement AzureTextToSpeechProvider

Use Microsoft.CognitiveServices.Speech SDK already present in the project.

Expected behavior:

Input:

- text
- target language

Output:

- audio bytes
- MIME type / content type

Provider should:

- reject empty text
- map language code to voice where needed
- call Azure Speech TTS
- return audio bytes
- map provider errors to ProviderException
- avoid logging secrets or raw sensitive payloads

---

### Step 3 — Add API Endpoint

Preferred endpoint:

```text
POST /api/translate/tts
```

Request:

```json
{
  "text": "Hola, ¿cómo estás?",
  "language": "es"
}
```

Response:

- binary audio stream
- Content-Type: selected audio type
- correlation ID available via existing conventions where practical

Builder may use a new controller if cleaner, but should avoid unnecessary architectural churn.

---

### Step 4 — Add Frontend API Helper

Add a frontend API helper such as:

```ts
synthesizeSpeech(text: string, language: string): Promise<Blob>
```

It should:

- POST to the backend TTS endpoint
- receive a Blob
- surface API errors consistently with existing translation API helpers

---

### Step 5 — Add Play Button To ResultPanel

ResultPanel should show a Play button when translated text is present.

Button states:

- Play
- Loading
- Playing
- Error

The Play button should work for:

- text translation result
- audio translation result
- push-to-talk result

---

### Step 6 — Browser Audio Playback

Frontend should:

- create object URL from returned audio Blob
- play using HTMLAudioElement or equivalent
- clean up object URL
- prevent duplicate playback if user clicks repeatedly
- show useful errors if playback fails

---

## Voice Mapping Strategy

Builder must propose a simple MVP voice mapping during dry run.

Possible supported language examples:

- en → en-US-JennyNeural or default English voice
- es → es-ES-ElviraNeural or compatible Spanish voice
- fr → fr-FR-DeniseNeural or compatible French voice
- de → de-DE-KatjaNeural or compatible German voice
- it → it-IT-ElsaNeural
- ja → ja-JP-NanamiNeural
- ko → ko-KR-SunHiNeural
- zh-Hans → zh-CN-XiaoxiaoNeural
- pt → pt-BR-FranciscaNeural
- ar → ar-SA-ZariyahNeural

If exact voice names are uncertain, Builder must verify against Azure Speech voice list before implementation.

---

## Files Likely To Change

Backend likely:

- src/backend/MyTranslationApp.Application/Interfaces/ITextToSpeechProvider.cs
- src/backend/MyTranslationApp.Infrastructure/Providers/Mock/MockTextToSpeechProvider.cs
- src/backend/MyTranslationApp.Infrastructure/Providers/Azure/AzureTextToSpeechProvider.cs
- src/backend/MyTranslationApp.Api/Program.cs
- src/backend/MyTranslationApp.Api/Controllers/TranslationController.cs or new TtsController
- src/backend/MyTranslationApp.Application/Validation/*
- tests/backend/MyTranslationApp.Tests/*

Frontend likely:

- src/frontend/src/api/translationApi.ts
- src/frontend/src/types/api.ts
- src/frontend/src/components/ResultPanel.tsx
- src/frontend/src/app.css

Planning/docs:

- planning/STATE.md
- planning/DECISIONS.md
- planning/RISKS.md
- planning/QUESTIONS.md
- docs/API.md
- docs/ARCHITECTURE.md
- docs/VALIDATION.md

---

# planning/sprints/007-text-to-speech-playback/acceptance.md

# Sprint 007 Acceptance — Text-to-Speech Playback

## Backend Acceptance

- [ ] AzureTextToSpeechProvider implemented.
- [ ] MockTextToSpeechProvider behavior preserved.
- [ ] Provider switching works in Mock and Azure modes.
- [ ] TTS API endpoint exists.
- [ ] TTS endpoint rejects empty text.
- [ ] TTS endpoint rejects unsupported language or handles it gracefully.
- [ ] TTS endpoint returns browser-playable audio.
- [ ] TTS endpoint maps provider failures to existing error conventions.
- [ ] No secrets are logged or committed.

## Frontend Acceptance

- [ ] Play button appears when translated text exists.
- [ ] Play button works for text translation results.
- [ ] Play button works for audio translation results.
- [ ] Button shows loading state during audio generation.
- [ ] Audio plays in Chrome.
- [ ] Audio plays in Edge.
- [ ] Playback errors are shown clearly.
- [ ] Object URLs are cleaned up.
- [ ] Existing translation flows still work.

## Validation Acceptance

- [ ] Backend build succeeds.
- [ ] Backend tests pass.
- [ ] Frontend TypeScript check passes.
- [ ] Frontend build succeeds.
- [ ] Manual Azure TTS validation succeeds.
- [ ] Manual text translation + playback succeeds.
- [ ] Manual push-to-talk translation + playback succeeds.
- [ ] planning/STATE.md updated.
- [ ] planning/DECISIONS.md updated if decisions are made.
- [ ] planning/RISKS.md updated.
- [ ] planning/QUESTIONS.md updated.
- [ ] docs/API.md updated.
- [ ] docs/ARCHITECTURE.md updated.
- [ ] docs/VALIDATION.md updated.

---

# planning/sprints/007-text-to-speech-playback/handoff-prompt.md

# Builder Handoff Prompt — Sprint 007 Text-to-Speech Playback

You are the Builder for My Translation App.

You are working on Sprint 007: Text-to-Speech Playback.

Follow the Architect / Builder methodology.

## Read First

1. AGENTS.md
2. planning/STATE.md
3. planning/DECISIONS.md
4. planning/RISKS.md
5. planning/QUESTIONS.md
6. docs/API.md
7. docs/ARCHITECTURE.md
8. docs/VALIDATION.md
9. planning/sprints/007-text-to-speech-playback/requirements.md
10. planning/sprints/007-text-to-speech-playback/blueprint.md
11. planning/sprints/007-text-to-speech-playback/acceptance.md
12. Existing TTS provider/interface files
13. Existing ResultPanel frontend files

## Required Dry Run

Do not implement immediately.

First produce a dry run report answering:

1. What TTS code already exists?
2. Is ITextToSpeechProvider sufficient?
3. What backend files will change?
4. What frontend files will change?
5. Which audio format should be returned?
6. What Azure Speech voice mapping will be used?
7. What tests will be added or updated?
8. What manual validation will be run?
9. What risks or ambiguities remain?

## Implementation Rules

- Do not invent product scope beyond the pack.
- Do not add voice selection UI.
- Do not auto-play translated speech.
- Do not store generated audio.
- Do not commit secrets.
- Reuse existing Azure Speech config.
- Preserve existing text/audio/push-to-talk translation behavior.
- Update planning and docs after implementation.

## Completion Report Required

After implementation, report:

- Files created
- Files modified
- Commands run
- Build results
- Test results
- Manual Azure validation results
- Acceptance criteria status
- Remaining risks
- Recommended Sprint 008
