# Domain Notes

**Project:** My Translation App
**Client:** Acme Corp

---

## Business Problem

Communication problems are very common when two people speak different languages. This could be easily fixed by creating a platform that helps a person take in a sentence in either text or audio and the app immediately translates it into a chosen language for them.

---

## Users

**Primary:** Anyone trying to improve communication with people who speak different languages — travellers, expats, in-person meetings, casual conversations across language barriers.

**Characteristics:**
- Mobile users are likely common (travel context, in-person use).
- Users may not be technical.
- Users need a fast, simple, low-friction experience.
- No account or login should be required for the MVP.

---

## MVP Workflow

### Text Input Path

1. User selects source language and target language.
2. User types text into the input box.
3. User taps the Translate button.
4. Frontend validates input (non-empty, within length limit, valid language codes).
5. Frontend sends request to backend translation endpoint.
6. Backend validates request, calls translation provider, normalises response.
7. Frontend displays translated text.
8. User optionally taps Play to hear the translated text spoken aloud.
9. User can clear and start again.

### Audio Input Path

1. User selects source language and target language.
2. User presses and holds the microphone (push-to-talk) button.
3. Browser captures a short audio clip.
4. Frontend validates audio duration and file size.
5. Frontend sends audio to backend speech endpoint.
6. Backend validates request, calls speech-to-text provider, receives transcript.
7. Backend sends transcript to translation provider, receives translated text.
8. Backend returns transcript and translated text to frontend.
9. Frontend displays both the original transcript and the translation.
10. User optionally taps Play to hear the translated text spoken aloud.
11. User can clear and start again.

---

## Out Of Scope (MVP)

- Continuous real-time listening or interpretation.
- User accounts, registration, and login.
- Persistent translation history (beyond current session).
- Document or PDF translation.
- Offline translation models.
- WebSocket streaming translation.
- Native mobile applications.
- Browser extensions.
- Enterprise administration features.
- Billing or subscription management.
- Storing real user audio or translated text permanently.

---

## Pain Points

- Communication breakdown when two people speak different languages.
- Existing tools can be slow, complex, or require accounts.
- Audio translation in real-time apps is often over-engineered or inaccessible.

---

## UI / UX Notes

- Source language selector
- Target language selector
- Text input box
- Translate button
- Microphone push-to-talk button
- Transcript display for audio input
- Translated result display
- Optional play-audio button
- Clear / reset button
- Loading state while translating
- Error states for permission, provider, validation, and network failures
- Mobile-friendly layout preferred (translation is often needed while travelling or speaking in person)

---

## Terminology

| Term | Definition |
|---|---|
| Source Language | The language the user is translating from. |
| Target Language | The language the user is translating into. |
| Transcript | The text output from speech-to-text processing of recorded audio. |
| Push-to-Talk | User holds the record button to capture a short audio clip, releases to stop. |
| Provider | An external API service (e.g. Azure, Google, DeepL) used to perform translation or speech processing. |
| Provider Abstraction | A backend layer that normalises provider calls so providers can be swapped without frontend changes. |
| TTS | Text-to-Speech — converting translated text back into spoken audio. |
| STT | Speech-to-Text — converting recorded audio into a written transcript. |
| Session History | Translation results visible within the current browser session; not persisted across sessions. |
| Backend Proxy | The .NET backend that forwards requests to external provider APIs, keeping secrets server-side. |

---

## Systems and Tools

| System | Role | Status |
|---|---|---|
| React | Frontend browser UI | Recommended |
| .NET | Backend API proxy and service layer | Recommended |
| Azure Translator | Text translation provider | Recommended / TBD |
| Azure Speech | Speech-to-text and text-to-speech provider | Recommended / TBD |
| PostgreSQL | Database for persistence | Deferred |

---

## Notes

- Provider choice (Azure vs OpenAI vs Google vs DeepL) is an open question — see planning/QUESTIONS.md.
- Supported language list is not yet finalised — see planning/QUESTIONS.md.
- Text-to-speech playback is a should-have, not a blocker for MVP — see planning/QUESTIONS.md.
