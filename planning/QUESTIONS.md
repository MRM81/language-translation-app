# Questions

Track open questions that require client, Architect, or stakeholder input.

**Project:** My Translation App

---

| ID | Date | Question | Needed From | Blocking? | Status | Answer |
|---|---|---|---|---|---|---|
| Q-001 | 2026-05-28 | Which translation/speech provider should be used first: Azure (recommended), OpenAI, Google, DeepL, or another? | Project Owner | No — Azure assumed for planning. Final choice needed before implementation. | Open | TBD |
| Q-002 | 2026-05-28 | What language pairs must be supported at launch? | Project Owner | No — provider language list used as default. | Open | TBD |
| Q-003 | 2026-05-28 | Should text-to-speech (translated audio playback) be included in MVP or deferred to a follow-on sprint? | Project Owner | No | Open | TBD |
| Q-004 | 2026-05-28 | What is the maximum audio recording duration allowed in MVP? | Architect / Project Owner | No — suggested default: 30–60 seconds. | Open | TBD |
| Q-005 | 2026-05-28 | Should session translation history (current session only) be visible to the user in the UI? | Project Owner | No — recommended yes, session-only. | Open | TBD |
| Q-006 | 2026-05-28 | Where will the app be hosted (Azure, AWS, self-hosted, Vercel, etc.)? | Project Owner / Builder | No | Open | TBD |
| Q-007 | 2026-05-28 | Is this app intended as a portfolio/demo project or a production business product? | Project Owner | No — affects polish, rate limiting, privacy posture, and deployment approach. | Open | TBD |
| Q-008 | 2026-05-28 | Should source language be manually selected, auto-detected by the provider, or should the app support both? | Product Owner | No — backend architecture supports optional source language. Final policy affects validation rules. | Open | TBD |
| Q-009 | 2026-05-28 | What is the exact maximum character length for typed text input in MVP? | Architect / Product Owner | No — suggested 2000 characters in Sprint 001 docs. Needs confirmation before Sprint 003 validation implementation. | Resolved | 5,000 characters. Decision D-028 applied in Sprint 003. |
| Q-010 | 2026-05-28 | What is the exact maximum push-to-talk recording duration in MVP? | Architect / Product Owner | No — suggested 60 seconds in Sprint 001 docs. Needs confirmation before Sprint 003 audio validation implementation. | Resolved | 60 seconds. Decision D-029 applied in Sprint 003. Duration enforcement deferred (see R-016). |
| Q-011 | 2026-05-28 | Which browser audio MIME types should the backend accept from MediaRecorder output? | Builder / Technical Reviewer | No — depends on target browsers and Azure Speech SDK input format support. Required before Sprint 003 audio validation. See R-011. | Resolved | audio/webm, audio/webm;codecs=opus, audio/mp4, audio/mpeg, audio/wav. MIME matching normalises by stripping parameters. Decision D-030 applied in Sprint 003. |
| Q-012 | 2026-05-28 | Should text-to-speech (TTS) playback be included in Sprint 003 or deferred to a later sprint? | Product Owner | No | Resolved | TTS deferred. ITextToSpeechProvider interface defined as placeholder only. Decision D-033. |
| Q-013 | 2026-05-28 | Should translation history (session-only) ever be persisted to a database in a future sprint? | Product Owner | No — would trigger database schema, privacy design, and compliance review. | Open | TBD |
| Q-014 | 2026-05-28 | What is the deployment target and hosting environment for the app? | Product Owner | No — affects environment configuration, secrets management approach, and Sprint 008+ planning. | Open | TBD |
| Q-015 | 2026-05-28 | Should audio duration validation (60-second limit) be enforced in Sprint 004 or Sprint 005? | Builder / Architect | No — currently enforced via file size only. Exact enforcement requires an audio parsing library. | Open | TBD |
| Q-016 | 2026-05-28 | Should browser microphone recording (MediaRecorder / push-to-talk button) be added in Sprint 005 or a dedicated sprint? | Architect / Project Owner | No — Sprint 004 uses file upload only. Push-to-talk is deferred per D-037. | Open | TBD |
| Q-017 | 2026-05-28 | What final frontend styling direction is preferred — plain CSS, a utility framework (Tailwind), or a component library (MUI, Shadcn)? | Project Owner | No — Sprint 004 uses clean plain CSS. Final styling decision can be made before Sprint 008 polish. | Open | TBD |
| Q-018 | 2026-05-28 | Should the Vite dev server proxy be replaced with a CORS allow-list when the app is deployed, or will the frontend and backend be served from the same origin? | Builder / Architect | No — depends on hosting decision Q-014. Must be resolved before deployment sprint. | Open | TBD |
| Q-019 | 2026-05-28 | Should session translation history (last N results) be displayed in the UI in a future sprint? | Project Owner | No — Sprint 004 shows one result at a time. History display requires scoping a session store or context expansion. | Open | TBD |
| Q-020 | 2026-05-28 | Which Azure region will be used for local validation? | User | No — can proceed with placeholders and docs. | Open | TBD |
| Q-021 | 2026-05-28 | Will Translator and Speech use one multi-service Azure resource or separate resources? | User | No — config supports separate sections (AzureTranslator and AzureSpeech). | Open | TBD |
| Q-022 | 2026-05-28 | Which audio formats should be officially supported for Azure Speech live validation? | Architect/User | No — WAV and MP3 are fully supported. WebM is best-effort. MP4 is not supported by SDK. | Resolved | WAV (SDK path), MP3/WebM/OGG (Fast Transcription REST API). MP4 remains unsupported. Sprint 005.2. |
| Q-025 | 2026-05-30 | Does the Azure Speech Fast Transcription API accept `audio/webm` (without codec suffix) from Chrome MediaRecorder output? | Builder verification / live test | Yes — Sprint 006 push-to-talk readiness depends on this. | Resolved | CONFIRMED — Phase 0 live test (2026-05-30). Chrome 148 on Windows 10 negotiated `audio/webm;codecs=opus`, blob Content-Type `audio/webm`. HTTP 200, transcript and translation returned via Azure. Full response: correlationId 24824bf5-c1d2-40bc-90b7-4d6099bde6e8. Sprint 006 implementation unblocked. |
| Q-026 | 2026-05-30 | Should automatic speech language detection be enabled in a future sprint? | Architect / Project Owner | No | Open | TBD |
| Q-027 | 2026-05-30 | Should maximum recording duration remain 60 seconds for the push-to-talk MVP? | Architect / Project Owner | No — 60 s is the current code default. | Open | TBD |
| Q-023 | 2026-05-28 | Should provider selection default to Mock or fail if missing? | Architect | No | Resolved | Default is Mock (D-041). App starts without Azure config unless Provider=Azure is explicitly set. |
| Q-024 | 2026-05-28 | Should Azure live tests be automated later? | Architect/User | No — defer until CI/secrets strategy exists. | Open | TBD |
| Q-028 | 2026-05-30 | Which languages need explicit voice mapping beyond the 10 in the static catalog? | Architect / Builder | No — MVP covers the catalog languages. Unmapped codes use Azure default via fallback. | Open | TBD |
| Q-029 | 2026-05-30 | Should TTS playback use MP3 or WAV? | Builder | Yes — resolved during dry run. | Resolved | MP3 (audio/mpeg). Azure mode uses Audio24Khz96KBitRateMonoMp3. Mock mode returns silent WAV as a test fixture. D-060 / D-062. |
| Q-030 | 2026-05-30 | Should the app cache generated speech audio? | Future Sprint | No — out of scope for Sprint 007. Playback regenerates audio on each click. | Open | TBD |
| Q-031 | 2026-05-30 | Which design inspiration sources best represent the desired visual direction? | User | No — Sprint 008 builder review identified dark navy + indigo accent as the dominant pattern across inspiration images 1, 7, 8. | Open | TBD |
| Q-032 | 2026-05-30 | Should the application support dark mode toggle (user-selectable) in a future sprint? | Architect | No — Sprint 008 adopts dark as the primary theme. A toggle is a future enhancement. | Open | TBD |
| Q-033 | 2026-05-30 | Should branding (logo, icon, application name styling) be introduced? | User | No — "My Translation App" retained as placeholder in Sprint 008. Builder must not hardcode a product name without owner direction. | Open | TBD |
| Q-034 | 2026-05-31 | Should the app show capability badges per language (Text, Speech Input, Speech Output)? | Architect / Project Owner | No | Open | Deferred. Sprint 009A keeps selectors simple. Sprint 009B should address capability indicators. |
| Q-035 | 2026-05-31 | Should Chinese Traditional (`zh-Hant`) be included in the MVP expansion? | Project Owner | No | Resolved | Included — Azure support is straightforward and D-068 recommends explicit Mandarin representation. |
| Q-036 | 2026-05-31 | Should languages be grouped by region in the selector? | Project Owner | No | Open | Alphabetical list for MVP. Region grouping can be deferred. |
