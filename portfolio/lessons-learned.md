# Lessons Learned — My Translation App

---

## Sprint 001–002 — Discovery & Implementation Architecture

**Always invest in architecture before a line of implementation.**

Spending two full sprints on architecture — before writing any application code — paid off immediately in Sprint 003. The provider abstraction interfaces were correct on the first implementation pass because the contracts were designed before the adapters. The layering constraint (Application has zero Infrastructure references) was verified at compile time in every subsequent sprint with no violations.

**Decisions belong in writing.** The `planning/DECISIONS.md` log turned out to be one of the most valuable artifacts in the project. Decisions recorded with rationale in Sprint 002 prevented circular discussions in later sprints. Every time a question came up about why something was built a particular way, the answer was in the log.

---

## Sprint 003 — Backend API Skeleton

**Mock providers let you move fast without secrets.**

The full backend ran, validated requests, returned structured responses, and passed 30 tests — without a single Azure credential. The mock provider pattern meant the frontend could be built in Sprint 004 against a fully functional backend, completely independently of Azure resource provisioning.

**Centralise error codes early.** The `ErrorCodes` constants class introduced in Sprint 003 prevented string duplication across controllers, middleware, and tests. Adding a new error code later was one line.

---

## Sprint 004 — Frontend MVP Shell

**TypeScript interface discipline pays dividends.** Defining `types/api.ts` interfaces that mirror backend DTOs field-for-field caught the `errorCode` vs `code` discrepancy (D-038) during implementation rather than at runtime.

**`errorCode` not `code`.** Always validate your documentation against the actual wire format.

---

## Sprint 005 — Azure Provider Integration

**Azure SDK versions are not interchangeable.** `Azure.AI.Translation.Text` 2.0.0 targets API version `2026-06-06` which is not available on standard Azure AI Services resources. Pinning to 1.0.0 (API v3.0) was the fix. Lesson: pin SDK versions at integration time and document the reason.

**Test factory isolation matters.** `WebApplicationFactory` loads User Secrets in the Development environment. Explicitly pinning `Translation:Provider=Mock` in test factories (D-048) makes tests environment-independent.

---

## Sprint 005.2 — Audio Format Compatibility Fix

**Know your infrastructure constraints before choosing an API path.** The Azure Speech SDK's compressed-audio push-stream path had a GStreamer dependency that was invisible until the first real test on a Windows dev machine. The Azure Fast Transcription REST API handled all compressed formats server-side with no native dependencies. Lesson: test the full path — including infrastructure — on the actual target OS before committing to an SDK path.

**JSON config arrays fully replace code defaults.** `appsettings.json` JSON arrays completely replace (not merge with) code-level defaults at runtime in .NET. A `AllowedAudioMimeTypes` list correct in C# was silently overridden by the JSON array that didn't include `audio/ogg`. Lesson: when config and code both define a list, prefer config as the single source of truth, and test with config loaded.

---

## Sprint 006 — Push-to-Talk

**Phase 0 live validation before full implementation.** Running a minimal live path test before building all the push-to-talk UI confirmed that Chrome's `audio/webm;codecs=opus` was accepted by the Azure Fast Transcription API end-to-end. This resolved a high-impact risk before the full UI was built.

**`MediaRecorder` MIME type selection at runtime.** Browser MIME type support differs across browsers and OS versions. Runtime detection via `MediaRecorder.isTypeSupported()` with a preference list handles this without hardcoding.

---

## Sprint 007 — Text-to-Speech

**Binary API responses need a different correlation ID strategy.** Using the `X-Correlation-ID` response header (already established as an inbound convention) was the correct solution for returning a correlation ID alongside binary audio.

**Harden fetch error paths against non-JSON bodies.** Always check `Content-Type` before calling `.json()` in error paths. Always discriminate on a known field before writing an API error message to the UI.

---

## Sprint 008 — UX Modernization

**CSS custom properties are the right foundation for a design system.** Introducing `--color-bg`, `--accent`, `--radius-lg`, etc. at `:root` makes the entire visual system legible and future-proof.

**Mobile-first means the record button must be fingertip-sized.** A small text button is fine on desktop but fails as a primary touch target for a voice interaction feature.

**Visual hierarchy makes the output readable without scanning.** Displaying the translated text as hero-sized bold text means the result is legible at a glance — which is the point of a translation tool.

---

## Sprints 009A/B — Language Catalog Expansion & Capability Metadata

**Capability flags prevent silent feature degradation.** Adding `SupportsTextToSpeech`, `SupportsSpeechToText`, and `SupportsTextTranslation` flags to the language catalog DTO meant the frontend could disable the Play button gracefully for languages without TTS support. Without flags, the button would appear active and fail at runtime.

**Static catalogs age out.** A static 10-language catalog was fine for prototyping but required an explicit expansion sprint. When language support is a first-class feature, the catalog design should anticipate growth from day one.

---

## Sprint 010 — Deployment Readiness Hardening

**Health endpoints are not optional for deployed services.** `GET /health` returning a simple JSON body is the minimum viable observability surface. Without it, load balancers, deployment pipelines, and operations runbooks have no way to confirm the service is alive.

**Config-driven CORS before you need it.** Hardcoded CORS origins in `Program.cs` would have required a code change for every environment. `AllowedCorsOrigins` as a config array meant the production CORS value was an environment variable, not a deployment.

---

## Sprint 011 — AWS Deployment Preparation

**One environment variable unblocks a production architecture.** The entire separate-origin AWS deployment was blocked by a single hardcoded `API_BASE = ''` assumption in `translationApi.ts`. `VITE_API_BASE_URL` as an optional override with a sane default (`''` for same-origin) meant local dev required no change while production could point to any URL.

**Write the deployment docs before the deployment.** Drafting `docs/AWS_DEPLOYMENT.md` before touching the AWS console forced clarity on the exact sequence of steps, config values, and rollback procedures. The actual deployment (Sprint 012) had no surprises because the path was already written.

---

## Sprint 012 — AWS Production Deployment

**Same-origin CloudFront proxy solves multiple problems at once.** By proxying `/api/*` through CloudFront to EB, the pattern simultaneously solved: mixed-content errors (browser to HTTP EB), CORS complexity, and EB URL exposure. One architectural decision eliminated three separate concerns.

**IAM role creation is a one-time setup cost, not ongoing overhead.** The `aws-elasticbeanstalk-ec2-role` IAM role is created once. Every subsequent EB environment can reuse it. Document it the first time so it's never discovered by accident again.

---

## Sprint 013 — Conversation Mode

**Frontend orchestration layers add powerful features without backend cost.** Conversation Mode reuses three existing API endpoints to deliver a full bilingual conversation experience. No new backend controllers, no new DTOs, no new Azure integrations. The entire feature is React state management and component composition.

**Language swap semantics matter.** Swapping languages in Conversation Mode affects future turns only — existing history is immutable. This was a design decision that had to be explicit (D-092), because the alternative (retroactively rewriting history) would have been confusing and misleading. Design decisions about immutability belong in writing.

---

## Sprint 014 — Push-To-Talk in Conversation Mode

**Component reuse is only possible when components are designed without tight coupling.** `AudioCaptureService`, `PushToTalkButton`, `RecordingIndicator`, and `RecordingTimer` were all reused in Conversation Mode without modification. This was possible because they were designed around a MediaRecorder lifecycle API, not around the Translation Mode UI structure.

**Return type discipline matters in async pipelines.** Declaring `onAudioSubmit` as `void` instead of `Promise<void>` would have prevented `ConversationInput` from awaiting the translate + TTS pipeline, causing the recording state to reset before playback finished. One return type change, significant UX difference.

---

## Sprint 015 — Conversation Persistence

**Design the migration path before writing the schema.** The single-to-multi conversation migration (Sprint 016) was non-destructive because Sprint 015 stored a version field. Without a version, there would have been no safe way to detect legacy format at read time.

**Silent failure for localStorage errors is correct.** Storage quota exhaustion and private browsing mode both cause `localStorage` to throw. Wrapping all storage operations in silent `try/catch` means persistence failures degrade gracefully — the conversation is still usable in-session, just not persisted.

---

## Sprint 016 — Multi-Conversation Management

**`window.confirm()` is underrated for destructive operations in MVP.** A custom confirmation modal adds a component, a CSS class, keyboard trap management, and accessibility attributes. `window.confirm()` is accessible, cross-browser, zero-dependency, and immediately understood by users. Complexity should be introduced when there's a real reason, not preemptively.

**The delete-and-recreate invariant.** When the last conversation is deleted, a new "Conversation 1" is auto-created. This invariant means the UI never has to handle an empty conversation list — a state that would require special-casing in every component that reads conversations.

---

## Sprint 017 — Conversation Search & Demo Polish

**Auto-title from first message solves the blank-name problem.** Users routinely create conversations without naming them. Auto-titling from the first 40 characters of the first message gives conversations a meaningful name without requiring user action, while respecting any manual rename the user makes later.

**Full-text search at localStorage scale is instant.** Loading all conversation messages from localStorage for each search query has negligible cost at the data volumes a v1 app will ever see in a browser. Over-engineering a search index would have added complexity with zero user-perceptible benefit.

---

## Sprint 018 — UI/UX Redesign

**The landing page transforms the first impression.** Opening directly into the translator is efficient for development but not the right experience for a portfolio demo or a first-time user. A landing page with branding, description, and a clear CTA converts a tool into a product.

**Simple state machines beat routing libraries for portfolio apps.** Three state variables (`screen`, `workspaceMode`, `translationInputMode`) replace a routing library for a single-page app with a small number of views. No additional dependency, no bundle cost, full control.

**The Architect / Builder methodology worked.** Every sprint had an Architect Pack — a written specification produced before any code was written. The Builder (Claude Code) read the pack, ran a dry run, and waited for explicit approval before implementing. This discipline caught scope creep, prevented regressions, and kept the decision log accurate. The project shipped v1.0 on time across 19 sprints with 133 passing tests and zero major regressions.
