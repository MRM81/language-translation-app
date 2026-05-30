# Lessons Learned — My Translation App

---

## Sprint 001–002 — Discovery & Implementation Architecture

**Always invest in architecture before a line of implementation.**

Spending two full sprints on architecture — before writing any application code — paid off immediately in Sprint 003. The provider abstraction interfaces were correct on the first implementation pass because the contracts were designed before the adapters. The layering constraint (Application has zero Infrastructure references) was verified at compile time in every subsequent sprint with no violations.

**Decisions belong in writing.** The `planning/DECISIONS.md` log turned out to be one of the most valuable artifacts in the project. Decisions recorded with rationale in Sprint 002 (e.g. D-022 on API versioning, D-007 on credentials) prevented circular discussions in later sprints. Every time a question came up about why something was built a particular way, the answer was in the log.

---

## Sprint 003 — Backend API Skeleton

**Mock providers let you move fast without secrets.**

The full backend ran, validated requests, returned structured responses, and passed 30 tests — without a single Azure credential. The mock provider pattern meant the frontend could be built in Sprint 004 against a fully functional backend, completely independently of Azure resource provisioning.

**Centralise error codes early.** The `ErrorCodes` constants class introduced in Sprint 003 prevented string duplication across controllers, middleware, and tests. Adding a new error code later (e.g. for TTS in Sprint 007) was one line.

---

## Sprint 004 — Frontend MVP Shell

**TypeScript interface discipline pays dividends.** Defining `types/api.ts` interfaces that mirror backend DTOs field-for-field caught the `errorCode` vs `code` discrepancy (D-038) during implementation rather than at runtime. The Architect Pack documentation had used `code`; the actual backend serialised to `errorCode`. Strong typing surfaced this before a single API call was made.

**`errorCode` not `code`.** A small naming inconsistency between the Architect Pack documentation and the actual backend DTO serialisation. Recorded as D-038. Lesson: always validate your documentation against the actual wire format.

---

## Sprint 005 — Azure Provider Integration

**Azure SDK versions are not interchangeable.** `Azure.AI.Translation.Text` 2.0.0 targets API version `2026-06-06` which is not available on standard Azure AI Services multi-service resources. Pinning to 1.0.0 (API v3.0) was the fix. The convenience method signature happened to be identical in both SDK versions, so no provider code changed. Lesson: pin SDK versions at integration time and document the reason (D-047).

**Test factory isolation matters.** `WebApplicationFactory` loads User Secrets in the Development environment. If User Secrets had `Translation:Provider=Azure`, tests that expected mock output would fail on any machine with credentials configured. Explicitly pinning `Translation:Provider=Mock` in test factories (D-048) makes tests environment-independent.

---

## Sprint 005.2 — Audio Format Compatibility Fix

**Know your infrastructure constraints before choosing an API path.** The Azure Speech SDK's compressed-audio push-stream path had a GStreamer dependency that was invisible until the first real test on a Windows dev machine. The Azure Fast Transcription REST API (a newer endpoint) handled all compressed formats server-side with no native dependencies. Lesson: test the full path — including infrastructure — on the actual target OS before committing to an SDK path.

**JSON config arrays fully replace code defaults.** `appsettings.json` JSON arrays completely replace (not merge with) code-level defaults at runtime in .NET. A `AllowedAudioMimeTypes` list correct in C# was silently overridden by the JSON array that didn't include `audio/ogg`. This caused `audio/ogg` to be rejected at the live validation layer even though tests (which used code defaults) passed. Lesson: when config and code both define a list, prefer config as the single source of truth, and test with config loaded.

---

## Sprint 006 — Push-to-Talk

**Phase 0 live validation before full implementation.** Running a minimal live path test (Phase 0) before building all the push-to-talk UI components confirmed that Chrome's `audio/webm;codecs=opus` was accepted by the Azure Fast Transcription API end-to-end. This resolved a high-impact risk (R-030) that would have been expensive to discover after the full UI was built.

**`MediaRecorder` MIME type selection at runtime.** Browser MIME type support differs across browsers and OS versions. Runtime detection via `MediaRecorder.isTypeSupported()` with a preference list handles this without hardcoding. The preference order `audio/webm;codecs=opus → audio/webm → audio/ogg;codecs=opus → audio/ogg` covers all target browsers.

---

## Sprint 007 — Text-to-Speech

**Binary API responses need a different correlation ID strategy.** The standard pattern of returning `correlationId` in the JSON body doesn't work when the response body is raw binary audio. Using the `X-Correlation-ID` response header (already established as an inbound convention by `CorrelationIdMiddleware`) was the correct solution — consistent and standard.

**Harden fetch error paths against non-JSON bodies.** The `synthesizeSpeech` function calling `res.json()` unconditionally in the error path caused raw JS engine error strings to leak to the UI when the error body was not valid JSON (proxy errors, empty bodies). Always check `Content-Type` before calling `.json()`. Always discriminate on a known field (e.g. `errorCode`) before writing an API error message to the UI.

---

## Sprint 008 — UX Modernization

**CSS custom properties are the right foundation for a design system.** Introducing `--color-bg`, `--accent`, `--radius-lg`, etc. at `:root` makes the entire visual system legible and future-proof. A dark-mode toggle in a future sprint requires only a `[data-theme="light"]` block, not a full stylesheet rewrite.

**Mobile-first means the record button must be fingertip-sized.** A `0.6rem 1.25rem` text button is fine on desktop but fails as a primary touch target for a voice interaction feature. The circular 72×72px record button is the correct mobile affordance for this use case.

**Visual hierarchy makes the output readable without scanning.** The previous result panel buried the translation in a key-value table. Displaying the translated text as hero-sized bold text (1.375rem, weight 700) means the result is legible at a glance — which is the point of a translation tool.
