# Architect Pack 009A — Language Catalog Expansion

## Pack Metadata

| Field | Value |
|---|---|
| Project Name | My Translation App |
| Sprint Number | 009A |
| Sprint Name | Language Catalog Expansion |
| Architect | ChatGPT |
| Builder Target | Claude Code |
| Status | Ready For Builder Dry Run |

---

## Sprint Goal

Expand the supported language catalog from the current MVP set to a broader portfolio-quality set of European and global languages while preserving all existing translation, audio transcription, and text-to-speech workflows.

This sprint is a focused enhancement sprint.

It must not change deployment architecture, authentication, persistence, or core provider design.

---

## Project Context

The application currently supports a small language catalog suitable for MVP validation.

The app now has:

- text translation
- audio upload translation
- push-to-talk translation
- text-to-speech playback
- Azure provider integration
- modern portfolio UI

The next product improvement is to support more languages, especially European languages and Mandarin Chinese.

---

# planning/STATE.md

## Sprint Status

Sprint 008: Complete

Sprint 009A: Active

### Sprint 009A Objective

Expand the language catalog and update all affected validation, UI, Azure translation, speech-to-text, and text-to-speech language handling.

### Success Definition

Users can select from a broader language list and successfully use the existing workflows where Azure supports the required translation, STT, and TTS capability.

---

# planning/DECISIONS.md

Add:

## D-065 — Language Expansion Is A Focused Enhancement Sprint

Language expansion will be handled as Sprint 009A instead of being bundled into deployment prep.

Rationale:

- Language support affects validation, UI, translation, speech, and TTS mapping.
- It should be tested independently before deployment.

Status: Accepted

---

## D-066 — Translation Support And Speech Support Are Not Identical

The app must distinguish between:

- languages supported for text translation
- languages supported for speech-to-text
- languages supported for text-to-speech

Rationale:

Azure Translator may support more languages than Azure Speech STT/TTS.

Status: Accepted

---

## D-067 — German Covers Austria For MVP

"Austrian" is not treated as a separate translation language in Sprint 009A.

Austria is represented by German.

If future voice localization is needed, Austrian German voice selection can be considered later.

Status: Accepted

---

## D-068 — Mandarin Chinese Should Be Explicitly Represented

Mandarin Chinese should be represented clearly in the UI.

Preferred MVP labels:

- Chinese Simplified / Mandarin
- Chinese Traditional, if supported and useful

Status: Accepted

---

# planning/RISKS.md

Add:

## R-038 — Azure Translator And Azure Speech Language Support May Differ

Impact: Medium  
Likelihood: High  
Mitigation: Builder must verify supported languages for translation, STT, and TTS before finalizing the catalog.

---

## R-039 — TTS Voice Mapping Can Become Fragile

Impact: Medium  
Likelihood: Medium  
Mitigation: Use explicit voice map only where verified. Gracefully handle unsupported TTS languages.

---

## R-040 — Long Language Lists Can Hurt Mobile UX

Impact: Medium  
Likelihood: Medium  
Mitigation: Keep language selectors usable on mobile. Consider alphabetical ordering and clear labels.

---

## R-041 — Unsupported Audio Languages May Confuse Users

Impact: Medium  
Likelihood: Medium  
Mitigation: If a language supports text translation but not STT or TTS, document and display behavior clearly.

---

# planning/QUESTIONS.md

Add:

## Q-034 — Should The App Show Capability Badges Per Language?

Should language options indicate capabilities such as Text, Speech Input, and Speech Output?

Status: Open  
Blocking: No  
Recommendation: Defer badges unless needed. Keep Sprint 009A simple.

---

## Q-035 — Should Chinese Traditional Be Included In MVP Expansion?

Status: Open  
Blocking: No  
Recommendation: Include if Azure support is straightforward.

---

## Q-036 — Should Languages Be Grouped By Region?

Status: Open  
Blocking: No  
Recommendation: Alphabetical list for MVP. Region grouping can be deferred.

---

# docs/API.md

Update language catalog documentation.

## GET /api/languages

The language catalog must document each language with:

- code
- display name
- translation support
- speech-to-text support, if represented
- text-to-speech support, if represented

If the current DTO only supports code and name, Builder should report whether extending the DTO is necessary or whether capability handling can remain internal for Sprint 009A.

---

# docs/ARCHITECTURE.md

Update language catalog section.

## Language Catalog

Sprint 009A expands the language catalog.

The catalog affects:

- frontend language selectors
- request validation
- Azure Translator source/target codes
- Azure Speech STT locale mapping
- Azure Speech TTS voice mapping

Translation, STT, and TTS support must not be assumed to be identical.

---

# docs/VALIDATION.md

Add Sprint 009A validation section.

## Sprint 009A Validation

Validate:

- Expanded languages appear in frontend selectors.
- Existing workflows still work for original languages.
- Czech and Slovak appear and translate correctly.
- German appears and can be used for Austria-related use.
- Mandarin Chinese appears with clear naming.
- TTS mapping succeeds or fails gracefully for expanded languages.
- STT mapping succeeds or fails gracefully for expanded languages.
- Mobile selector usability remains acceptable.

---

# planning/sprints/009A-language-catalog-expansion/requirements.md

# Sprint 009A Requirements — Language Catalog Expansion

## Goal

Expand available languages, especially European languages and Mandarin Chinese, while preserving all completed MVP workflows.

## In Scope

### Language Catalog

Add or verify support for:

#### Existing Core Languages

- English
- Spanish
- French
- German
- Italian
- Portuguese
- Arabic
- Japanese
- Chinese / Mandarin
- Russian

#### European Expansion

- Dutch
- Polish
- Czech
- Slovak
- Romanian
- Hungarian
- Greek
- Swedish
- Danish
- Norwegian
- Finnish
- Ukrainian
- Turkish
- Croatian
- Serbian
- Slovenian
- Bulgarian
- Lithuanian
- Latvian
- Estonian

#### Global Expansion

- Korean
- Vietnamese
- Thai
- Indonesian
- Malay
- Hindi

### Required Clarification

Builder must verify exact Azure language codes before implementation.

Examples likely to require confirmation:

- Czech: likely `cs`
- Slovak: likely `sk`
- Dutch: likely `nl`
- Greek: likely `el`
- Chinese Simplified: likely `zh-Hans` or provider-specific code
- Chinese Traditional: likely `zh-Hant` if included
- Norwegian: may require `nb` or `no`
- Serbian: may require script-specific code

### Validation

- Update backend language catalog.
- Update frontend language selectors if they consume API catalog only.
- Update validation rules.
- Update STT locale map where speech input is supported.
- Update TTS voice map where playback is supported.
- Preserve original language behavior.
- Add tests for new catalog entries.

## Out Of Scope

- Auto language detection improvements.
- Region-specific dialect selection UI.
- Voice selection UI.
- Capability badges unless already trivial.
- Translation history.
- Deployment.
- Authentication.
- Database persistence.

## Constraints

- Do not hardcode unsupported Azure language codes without verification.
- Do not break existing provider behavior.
- Do not add languages to STT/TTS mappings unless Azure support is verified or graceful fallback exists.
- Keep selectors mobile-friendly.

---

# planning/sprints/009A-language-catalog-expansion/blueprint.md

# Sprint 009A Blueprint — Language Catalog Expansion

## Required Builder Dry Run

Before implementation, Builder must inspect:

- Existing language catalog service/file
- Translation validation rules
- AzureTextTranslationProvider language handling
- AzureSpeechToTextProvider language map
- AzureTextToSpeechProvider voice map
- Frontend LanguageSelect behavior
- Existing tests covering language catalog

Builder must report:

1. Current language catalog source of truth.
2. Current supported language codes.
3. Exact files to modify.
4. Azure Translator support verification method.
5. Azure Speech STT support verification method.
6. Azure Speech TTS voice mapping strategy.
7. Whether DTO changes are required.
8. Test plan.
9. Risks or unsupported languages.

Do not implement until dry run is approved.

---

## Phase 1 — Verify Azure Language Support

Builder should verify:

- Translator supported target/source codes
- Speech-to-text locales for relevant languages
- Text-to-speech voices for relevant languages

Use official Microsoft documentation or SDK/service response where available.

Do not guess codes.

---

## Phase 2 — Expand Text Translation Catalog

Update the central language catalog to include verified text translation languages.

Language display names should be user-friendly.

Examples:

- Czech
- Slovak
- German
- Chinese Simplified (Mandarin)
- Chinese Traditional, if included
- Portuguese
- Dutch
- Polish

---

## Phase 3 — Update Speech-To-Text Mapping

For languages with verified STT support, update the speech locale map.

If a language supports text translation but not speech input, do not claim speech support.

---

## Phase 4 — Update Text-To-Speech Mapping

For languages with verified TTS support, update the Azure voice map.

If a mapped neural voice fails verification, use Azure default voice fallback where practical or document unsupported playback.

---

## Phase 5 — Update Tests

Tests should cover:

- Language catalog includes new European languages.
- Czech and Slovak are present.
- Mandarin Chinese label is clear.
- Existing languages still present.
- STT mapping handles expanded languages or fails gracefully.
- TTS voice mapping handles expanded languages or falls back safely.
- Validation accepts new language codes.
- Validation rejects unsupported codes.

---

## Phase 6 — Manual Validation

Manually test:

- English → Czech
- English → Slovak
- English → German
- English → Mandarin Chinese
- Czech → English, if supported
- Slovak → English, if supported
- TTS playback for at least German, Czech or Slovak, and Mandarin if supported
- Push-to-talk for at least one expanded STT-supported language

---

# planning/sprints/009A-language-catalog-expansion/acceptance.md

# Sprint 009A Acceptance — Language Catalog Expansion

## Catalog Acceptance

- [ ] Expanded language catalog implemented.
- [ ] Czech is available.
- [ ] Slovak is available.
- [ ] German is available and documented as Austria-compatible for MVP.
- [ ] Mandarin Chinese is available with clear label.
- [ ] Existing original languages still available.
- [ ] Language list remains usable on mobile.

## Translation Acceptance

- [ ] Text translation works for Czech.
- [ ] Text translation works for Slovak.
- [ ] Text translation works for German.
- [ ] Text translation works for Mandarin Chinese.
- [ ] Existing translation workflows still work.

## Speech-To-Text Acceptance

- [ ] STT mappings updated where Azure support is verified.
- [ ] Unsupported STT languages fail gracefully.
- [ ] Push-to-talk still works for existing supported languages.

## Text-To-Speech Acceptance

- [ ] TTS voice mappings updated where Azure support is verified.
- [ ] Unsupported TTS languages fail gracefully.
- [ ] TTS playback still works for existing supported languages.

## Test Acceptance

- [ ] Backend build succeeds.
- [ ] Backend tests pass.
- [ ] Frontend TypeScript passes.
- [ ] Frontend build succeeds.
- [ ] Language catalog tests added or updated.
- [ ] Validation tests added or updated.

## Documentation Acceptance

- [ ] docs/API.md updated.
- [ ] docs/ARCHITECTURE.md updated.
- [ ] docs/VALIDATION.md updated.
- [ ] planning/STATE.md updated.
- [ ] planning/DECISIONS.md updated.
- [ ] planning/RISKS.md updated.
- [ ] planning/QUESTIONS.md updated.

---

# planning/sprints/009A-language-catalog-expansion/handoff-prompt.md

# Builder Handoff Prompt — Sprint 009A Language Catalog Expansion

You are the Builder for My Translation App.

You are working on Sprint 009A: Language Catalog Expansion.

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
9. planning/sprints/009A-language-catalog-expansion/requirements.md
10. planning/sprints/009A-language-catalog-expansion/blueprint.md
11. planning/sprints/009A-language-catalog-expansion/acceptance.md
12. Existing language catalog files
13. Existing STT and TTS mapping files
14. Existing validation tests

## Required Dry Run

Do not implement immediately.

First produce a dry run report answering:

1. Where is the language catalog source of truth?
2. What language codes are currently supported?
3. Which requested languages are supported by Azure Translator?
4. Which requested languages are supported by Azure Speech STT?
5. Which requested languages are supported by Azure Speech TTS?
6. What files will change?
7. What tests will be added or updated?
8. Which language codes require special handling?
9. Which languages, if any, should be excluded or marked text-only?

## Implementation Rules

- Do not guess Azure language codes.
- Do not invent unsupported speech or TTS support.
- Preserve existing language behavior.
- Keep selectors mobile-friendly.
- Do not add new major product features.
- Update planning and docs after implementation.

## Completion Report Required

After implementation, report:

- Files created
- Files modified
- Commands run
- Build results
- Test results
- Manual validation results
- Expanded language list
- Any text-only languages
- Any unsupported languages deferred
- Acceptance criteria status
- Recommended Sprint 009B or Sprint 010
