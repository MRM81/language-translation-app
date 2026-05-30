# Architect Pack: Sprint 005 - Azure Provider Integration

## Pack Metadata

| Field | Value |
|---|---|
| Project Name | My Translation App |
| Client / Owner | Mark McLachlan |
| Project Slug | my-translation-app |
| Sprint Number | 005 |
| Sprint Name | Azure Provider Integration |
| Created Date | 2026-05-28 |
| Architect | ChatGPT Architect Layer |
| Builder Target | Claude Code / Codex / Cursor / Other |
| Status | Ready For Builder |

---

## 1. Project Context

My Translation App is a web browser based language translation application. The MVP allows users to enter text or upload an audio file, choose a target language, and receive a translated result.

Completed sprint foundation:

- Sprint 001: Discovery and architecture baseline.
- Sprint 002: Durable architecture and project decisions.
- Sprint 003: .NET 8 backend API skeleton with mock providers.
- Sprint 004: React + TypeScript frontend MVP shell connected to mock backend endpoints.

Sprint 005 introduces real Azure-backed provider adapters behind the existing provider interfaces. The goal is to replace mock translation and speech-to-text behaviour through configuration while preserving the existing API contract and frontend workflow.

The handoff remains the project folder, not chat history.

---

## 2. Sprint Goal

Implement Azure Translator and Azure Speech-to-Text provider adapters in the backend Infrastructure layer, wire provider selection through safe configuration, and document local setup using .NET User Secrets or environment variables without committing secrets.

---

## 3. Problem Being Solved

The app currently proves the full workflow using mock providers only. This sprint moves the backend from deterministic mock output to real provider integration while keeping the API, frontend, DTOs, validation, and architecture boundaries stable.

---

## 4. Requirements

| ID | Requirement | Priority | Notes |
|---|---|---|---|
| R-005-001 | Add Azure text translation provider | Must | Implement behind `ITextTranslationProvider`. |
| R-005-002 | Add Azure speech-to-text provider | Must | Implement behind `ISpeechToTextProvider`. |
| R-005-003 | Preserve all existing public API contracts | Must | No frontend contract breakage. |
| R-005-004 | Preserve provider abstraction boundaries | Must | Application must not reference Azure SDKs or Infrastructure types. |
| R-005-005 | Add configuration-based provider selection | Must | Support at least `Mock` and `Azure`. |
| R-005-006 | Do not commit Azure keys/secrets | Must | Use placeholders, User Secrets, or environment variables only. |
| R-005-007 | Add safe error handling for provider failures | Must | Do not expose secrets or raw provider internals. |
| R-005-008 | Add tests covering provider selection and config validation | Should | Prefer tests that do not require real Azure calls. |
| R-005-009 | Update backend documentation | Must | Include setup instructions and required config keys. |
| R-005-010 | Keep Sprint 004 frontend unchanged unless contract bug is discovered | Must | This is backend integration only. |

---

## 5. In Scope

The Builder may work on:

- Backend Infrastructure provider implementations.
- Backend configuration options and validation.
- Dependency injection registration in `Program.cs` or equivalent composition root.
- Backend README/setup documentation.
- Existing backend tests or new backend tests.
- Planning and docs updates required for Sprint 005.

Expected backend provider work:

- `AzureTextTranslationProvider`
- `AzureSpeechToTextProvider`
- Azure provider options/configuration models
- Provider selection logic for `Mock` vs `Azure`
- Safe exceptions/error mapping
- Documentation for local secrets setup

---

## 6. Out Of Scope

The Builder must not work on:

- React frontend feature changes.
- UI redesign.
- Browser microphone recording or push-to-talk.
- Text-to-speech endpoint implementation.
- Authentication or authorization.
- Database schema, migrations, or persistence.
- WebSockets, realtime streaming, or live transcription.
- Deployment, CI/CD, hosting, or production infrastructure.
- Creating real Azure resources on behalf of the user.
- Committing real Azure keys, regions, endpoints, tokens, or connection strings.
- Broad refactors unrelated to provider integration.

---

## 7. Assumptions

| ID | Assumption | Confidence | Action If Wrong |
|---|---|---|---|
| A-005-001 | Azure Translator and Azure Speech are the intended first real providers. | High | Record alternative in `QUESTIONS.md` and stop before replacing provider plan. |
| A-005-002 | The existing Sprint 003 interfaces are sufficient or need only small non-breaking changes. | Medium | If interface changes are required, document and update tests. |
| A-005-003 | Local development should use .NET User Secrets or environment variables. | High | Do not commit secrets under any circumstance. |
| A-005-004 | Existing frontend can validate real provider output without changes. | High | Only report frontend issues; do not expand scope. |
| A-005-005 | Real Azure calls may not be runnable by Builder if credentials are unavailable. | High | Provide config validation and mocked/unit tests; document manual validation separately. |

---

## 8. Constraints

- Must target .NET 8.
- Must preserve current endpoints:
  - `GET /api/languages`
  - `POST /api/translate/text`
  - `POST /api/translate/audio`
- Must preserve DTO response shape, including `correlationId`.
- Must not log raw source text, translated text, transcripts, audio filenames, audio content, or full request bodies.
- Must not expose provider secrets in error responses.
- Must keep Application layer provider-neutral.
- Must keep Infrastructure as the Azure-specific implementation layer.
- Must keep mock providers available for local/dev fallback.
- Must not introduce production deployment assumptions.

---

## 9. Dependencies

| Dependency | Type | Status | Notes |
|---|---|---|---|
| Azure Translator service | External API | Required for live validation | Builder must not create resource or commit key. |
| Azure Speech service | External API | Required for live validation | Builder must not create resource or commit key. |
| Existing provider interfaces | Code | Available | Created in Sprint 003. |
| Existing frontend shell | Code | Available | Created in Sprint 004. |
| .NET User Secrets | Local config | Expected | Preferred for local secret storage. |
| Azure SDK packages | Package | To be added if required | Infrastructure project only. |

---

## 10. Files To Read First

The Builder must read these before doing work:

1. `AGENTS.md`
2. `planning/STATE.md`
3. `planning/DECISIONS.md`
4. `planning/DOMAIN.md`
5. `planning/RISKS.md`
6. `planning/QUESTIONS.md`
7. `docs/ARCHITECTURE.md`
8. `docs/API.md`
9. `docs/VALIDATION.md`
10. `src/backend/MyTranslationApp.Api/Program.cs`
11. `src/backend/MyTranslationApp.Application/Interfaces/ISpeechToTextProvider.cs`
12. `src/backend/MyTranslationApp.Application/Interfaces/ITextTranslationProvider.cs`
13. `src/backend/MyTranslationApp.Application/Services/TranslationService.cs`
14. `src/backend/MyTranslationApp.Infrastructure/Providers/MockTextTranslationProvider.cs`
15. `src/backend/MyTranslationApp.Infrastructure/Providers/MockSpeechToTextProvider.cs`
16. `planning/sprints/005-azure-provider-integration/requirements.md`
17. `planning/sprints/005-azure-provider-integration/blueprint.md`
18. `planning/sprints/005-azure-provider-integration/acceptance.md`
19. `planning/sprints/005-azure-provider-integration/handoff-prompt.md`

---

## 11. Files To Create Or Modify

| Path | Action | Purpose |
|---|---|---|
| `planning/sprints/005-azure-provider-integration/requirements.md` | Create | Sprint requirements. |
| `planning/sprints/005-azure-provider-integration/blueprint.md` | Create | Builder implementation plan. |
| `planning/sprints/005-azure-provider-integration/acceptance.md` | Create | Done criteria. |
| `planning/sprints/005-azure-provider-integration/handoff-prompt.md` | Create | Builder handoff prompt. |
| `src/backend/MyTranslationApp.Infrastructure/Providers/Azure/AzureTextTranslationProvider.cs` | Create | Azure text translation adapter. |
| `src/backend/MyTranslationApp.Infrastructure/Providers/Azure/AzureSpeechToTextProvider.cs` | Create | Azure speech-to-text adapter. |
| `src/backend/MyTranslationApp.Infrastructure/Configuration/AzureTranslationOptions.cs` | Create | Azure provider configuration model. |
| `src/backend/MyTranslationApp.Infrastructure/Configuration/AzureSpeechOptions.cs` | Create | Azure speech configuration model. |
| `src/backend/MyTranslationApp.Api/Program.cs` | Modify | Provider selection and DI wiring. |
| `src/backend/MyTranslationApp.Api/appsettings.json` | Modify | Add non-secret provider config placeholders only. |
| `src/backend/MyTranslationApp.Api/appsettings.Development.json` | Modify if needed | Non-secret local defaults only. |
| `src/backend/MyTranslationApp.Api/README.md` | Create/Modify | Backend local setup instructions. |
| `tests/backend/MyTranslationApp.Tests/` | Modify/Create tests | Provider config/selection tests. |
| `planning/STATE.md` | Modify | Mark Sprint 004 complete and Sprint 005 active/completed as appropriate. |
| `planning/DECISIONS.md` | Modify | Add Azure provider decisions. |
| `planning/RISKS.md` | Modify | Add provider integration risks. |
| `planning/QUESTIONS.md` | Modify | Resolve or add Azure setup questions. |
| `planning/FILE_INVENTORY.md` | Modify | Add Sprint 005 files. |
| `docs/ARCHITECTURE.md` | Modify | Add Azure provider architecture notes. |
| `docs/API.md` | Modify if needed | Confirm no public contract change. |
| `docs/VALIDATION.md` | Modify | Add provider failure/config validation notes. |

If exact file paths differ in the repository, preserve the current structure and document deviations in the completion report.

---

## 12. Blueprint

### Step 1: Inspect Existing Backend Contracts

- Read provider interfaces, DTOs, validation, services, controllers, and DI setup.
- Confirm the Application layer remains provider-neutral.
- Confirm existing mock provider behaviour remains intact.
- Confirm tests currently pass before making changes if practical.

### Step 2: Add Azure Configuration Models

Add configuration classes in Infrastructure or Api configuration area, keeping secrets out of source control.

Recommended logical config shape:

```json
{
  "Translation": {
    "Provider": "Mock"
  },
  "AzureTranslator": {
    "Endpoint": "https://api.cognitive.microsofttranslator.com/",
    "Region": "YOUR_REGION_HERE",
    "Key": "DO_NOT_COMMIT_REAL_KEY"
  },
  "AzureSpeech": {
    "Region": "YOUR_REGION_HERE",
    "Key": "DO_NOT_COMMIT_REAL_KEY"
  }
}
```

Important:

- Real keys must not be committed.
- `appsettings.json` may contain placeholders only if clearly non-secret.
- Prefer documenting User Secrets commands.

### Step 3: Implement Azure Text Translation Provider

- Implement `ITextTranslationProvider` in Infrastructure.
- Use official Azure Translator SDK or HTTP client, whichever is cleaner and stable for .NET 8.
- Keep provider output provider-neutral.
- Return plain translated text to Application layer.
- Avoid logging raw input or output.
- Map provider errors to safe exceptions/results for the API error middleware.

### Step 4: Implement Azure Speech-To-Text Provider

- Implement `ISpeechToTextProvider` in Infrastructure.
- Accept the existing stream/file metadata contract.
- Use official Azure Speech SDK if appropriate.
- Return transcript string only.
- Avoid persistent audio storage.
- Avoid logging file name or audio content.
- Handle unsupported/failed recognition safely.

### Step 5: Add Provider Selection

In the Api composition root:

- If configured provider is `Mock`, register mock providers.
- If configured provider is `Azure`, register Azure providers.
- If provider value is unsupported, fail fast with a safe configuration error during startup.
- Keep `ITextToSpeechProvider` placeholder/mock unchanged unless required by DI.

### Step 6: Add Tests

Add tests that do not require real Azure credentials:

- `Mock` provider selection registers mock implementations.
- Unsupported provider value fails validation or startup in a controlled way.
- Azure options validation detects missing required values when provider is Azure.
- Existing validation tests still pass.
- Existing controller contract tests still pass, at least with mock provider.

If direct DI startup tests are difficult, document what was tested and why.

### Step 7: Manual Live Validation Instructions

Add README instructions for live Azure validation:

- How to set local User Secrets.
- How to set provider to Azure.
- How to run backend and frontend.
- How to test text translation.
- How to test audio upload transcription + translation.
- How to revert provider to Mock.

### Step 8: Update Durable Project Files

Update:

- `planning/STATE.md`
- `planning/DECISIONS.md`
- `planning/RISKS.md`
- `planning/QUESTIONS.md`
- `planning/FILE_INVENTORY.md`
- `docs/ARCHITECTURE.md`
- `docs/API.md` if contract notes change
- `docs/VALIDATION.md`

---

## 13. Data Flow / Logic Flow

Current mock flow:

```text
Frontend -> API Controller -> TranslationService -> Mock Provider -> DTO Response
```

Sprint 005 target flow:

```text
Frontend -> API Controller -> TranslationService -> Provider Interface -> Azure Provider -> Azure Service -> Provider Interface Result -> DTO Response
```

Provider selection:

```text
appsettings/User Secrets -> Provider Selection -> DI Registration -> Runtime Provider
```

No frontend contract change should be required.

---

## 14. UI / UX Notes

Sprint 005 does not include frontend UI changes.

The existing Sprint 004 UI should continue to work with real provider responses once the backend provider is set to Azure.

If the Builder discovers a frontend bug caused by real provider output, record it in `planning/QUESTIONS.md` or `planning/RISKS.md` unless it blocks validation.

---

## 15. API / Integration Notes

Existing API endpoints must remain unchanged:

### `GET /api/languages`

No expected change.

### `POST /api/translate/text`

Existing request:

```json
{
  "sourceText": "Hello",
  "sourceLanguage": "en",
  "targetLanguage": "es"
}
```

Existing response shape must remain:

```json
{
  "translatedText": "Hola",
  "sourceLanguage": "en",
  "targetLanguage": "es",
  "provider": "azure",
  "correlationId": "..."
}
```

### `POST /api/translate/audio`

Existing multipart fields:

- `audio`
- `targetLanguage`
- `sourceLanguage` optional

Existing response shape must remain:

```json
{
  "transcribedText": "Hello",
  "translatedText": "Hola",
  "sourceLanguage": "en",
  "targetLanguage": "es",
  "provider": "azure",
  "correlationId": "..."
}
```

### Error Responses

Existing Sprint 003/004 error shape must remain:

```json
{
  "errorCode": "PROVIDER_ERROR",
  "message": "A safe user-facing message.",
  "details": [
    {
      "field": "provider",
      "message": "Safe detail only."
    }
  ],
  "correlationId": "..."
}
```

Do not expose Azure keys, endpoints containing secrets, stack traces, raw SDK error dumps, or raw user content.

---

## 16. Validation Plan

| Check | Method | Expected Result |
|---|---|---|
| Existing backend tests | `dotnet test` | Pass. |
| Backend build | `dotnet build` | 0 errors. |
| Provider selection: Mock | Automated or manual config test | Mock providers still work. |
| Provider selection: Azure missing config | Automated or manual config test | Fails safely with clear config error. |
| Provider selection: invalid value | Automated or manual config test | Fails safely. |
| Text translation live test | Manual with Azure credentials | Real translation returned, same DTO shape. |
| Audio translation live test | Manual with Azure credentials and sample audio | Transcript and translated text returned, same DTO shape. |
| No secrets committed | File inspection | No keys/tokens/connection strings in repo. |
| No raw user content logged | Code review | No source text/transcripts/audio details logged. |
| Application layer remains provider-neutral | Dependency inspection | No Azure packages referenced from Application. |
| Frontend build still works if run | Optional `npm run build` | No contract-driven breakage. |

---

## 17. Acceptance Criteria

Sprint is complete when:

- [ ] Sprint 005 planning files exist under `planning/sprints/005-azure-provider-integration/`.
- [ ] Azure text translation provider is implemented behind `ITextTranslationProvider`.
- [ ] Azure speech-to-text provider is implemented behind `ISpeechToTextProvider`.
- [ ] Mock providers remain available.
- [ ] Provider selection supports at least `Mock` and `Azure`.
- [ ] Invalid provider configuration fails safely.
- [ ] Missing Azure configuration fails safely when provider is Azure.
- [ ] Public API endpoint paths remain unchanged.
- [ ] Public request/response DTO shapes remain unchanged.
- [ ] Existing mock-mode tests pass.
- [ ] New provider-selection/config validation tests are added where practical.
- [ ] `dotnet build` succeeds.
- [ ] `dotnet test` succeeds, unless Azure live tests are explicitly documented as manual-only.
- [ ] No real secrets, keys, tokens, or credentials are committed.
- [ ] `appsettings.json` contains placeholders or non-secret defaults only.
- [ ] Backend README documents Azure local setup using User Secrets or environment variables.
- [ ] README documents how to switch between Mock and Azure provider modes.
- [ ] README documents manual live validation steps.
- [ ] No raw text, transcripts, audio filenames, audio content, or request bodies are logged.
- [ ] `planning/STATE.md` is updated.
- [ ] `planning/DECISIONS.md` is updated.
- [ ] `planning/RISKS.md` is updated.
- [ ] `planning/QUESTIONS.md` is updated.
- [ ] `planning/FILE_INVENTORY.md` is updated.
- [ ] `docs/ARCHITECTURE.md` is updated.
- [ ] `docs/VALIDATION.md` is updated.

---

## 18. Risks

| Risk | Impact | Likelihood | Mitigation | Owner |
|---|---|---|---|---|
| Azure credentials accidentally committed | High | Medium | Use User Secrets/env vars; inspect files before completion. | Builder |
| Azure SDK leaks into Application layer | Medium | Medium | Keep SDK references Infrastructure-only; verify project references. | Builder |
| Real provider error messages expose internals | Medium | Medium | Map to safe errors; preserve correlation IDs. | Builder |
| Audio format accepted by frontend/backend but rejected by Azure Speech | Medium | Medium | Document supported formats and manual validation sample requirements. | Builder |
| Provider costs/quota limits | Medium | Medium | Document live validation expectations; do not run unnecessary calls. | Builder/User |
| Region/endpoint mismatch | Medium | Medium | Validate config and document required Azure values. | Builder |
| API contract drift breaks frontend | High | Low | Do not change DTOs/endpoints; run existing tests. | Builder |

---

## 19. Open Questions

| Question | Needed From | Blocking? | Notes |
|---|---|---|---|
| Q-020: Which Azure region will be used for local validation? | User | No | Can proceed with placeholders and docs. |
| Q-021: Will Translator and Speech use one multi-service Azure resource or separate resources? | User | No | Config can support separate sections. |
| Q-022: Which audio formats should be considered officially supported for Azure live validation? | Architect/User | No | Backend currently accepts common types. |
| Q-023: Should provider selection default to Mock or fail if missing? | Architect | No | Recommended: default to Mock for local safety. |
| Q-024: Should Azure live tests be automated later? | Architect/User | No | Defer until CI/secrets strategy exists. |

If a question blocks safe implementation, the Builder must stop and ask. Otherwise proceed with explicit assumptions.

---

## 20. State Updates Required

At the end of the sprint, update:

- `planning/STATE.md`
- `planning/DECISIONS.md`
- `planning/RISKS.md`
- `planning/QUESTIONS.md`
- `planning/FILE_INVENTORY.md`
- active sprint acceptance status
- `docs/ARCHITECTURE.md`
- `docs/API.md` if any contract details need clarification
- `docs/VALIDATION.md`

Recommended decisions to add if confirmed during implementation:

- D-039: Azure provider adapters are Infrastructure-only.
- D-040: Provider selection is configuration-driven.
- D-041: Mock remains the safe default provider for local development.
- D-042: Secrets are supplied by User Secrets/environment variables, never committed.

---

## 21. Builder Dry Run Instructions

Before implementation, the Builder must produce a dry run summary.

The dry run must include:

- files read
- sprint understanding
- confirmed existing provider interfaces
- planned package additions
- planned file changes
- commands to run
- assumptions
- risks or ambiguities
- whether live Azure validation is possible with available credentials
- validation plan
- whether implementation is safe to start

Do not implement until the dry run has been reviewed and approved.

---

## 22. Builder Handoff Prompt

Copy this prompt into the Builder chat.

```markdown
You are the Builder for My Translation App.

You are working on Sprint 005: Azure Provider Integration.

Follow the 120x Architect / Builder methodology.

Read these files first:

1. AGENTS.md
2. planning/STATE.md
3. planning/DECISIONS.md
4. planning/DOMAIN.md
5. planning/RISKS.md
6. planning/QUESTIONS.md
7. docs/ARCHITECTURE.md
8. docs/API.md
9. docs/VALIDATION.md
10. src/backend/MyTranslationApp.Api/Program.cs
11. src/backend/MyTranslationApp.Application/Interfaces/ISpeechToTextProvider.cs
12. src/backend/MyTranslationApp.Application/Interfaces/ITextTranslationProvider.cs
13. src/backend/MyTranslationApp.Application/Services/TranslationService.cs
14. src/backend/MyTranslationApp.Infrastructure/Providers/MockTextTranslationProvider.cs
15. src/backend/MyTranslationApp.Infrastructure/Providers/MockSpeechToTextProvider.cs
16. planning/sprints/005-azure-provider-integration/requirements.md
17. planning/sprints/005-azure-provider-integration/blueprint.md
18. planning/sprints/005-azure-provider-integration/acceptance.md
19. planning/sprints/005-azure-provider-integration/handoff-prompt.md

Your instructions:

1. Do not write implementation code immediately.
2. First perform a dry run against the current repository state.
3. Summarize what this sprint builds.
4. Summarize what is explicitly out of scope.
5. Confirm current backend provider interfaces and API contract.
6. List files you expect to create or modify.
7. List packages you expect to add, and to which project.
8. List commands you will run.
9. List assumptions, risks, and ambiguities.
10. State whether live Azure validation is possible with available credentials.
11. List validation checks you plan to run.
12. Stop and wait for approval before implementation.

Sprint goal:

Implement Azure Translator and Azure Speech-to-Text provider adapters in the backend Infrastructure layer, wire provider selection through safe configuration, and document local setup using .NET User Secrets or environment variables without committing secrets.

Implementation constraints:

- Target .NET 8 only.
- Preserve all existing public API endpoint paths and DTO shapes.
- Keep Application provider-neutral.
- Keep Azure SDK/package references out of Application.
- Infrastructure may contain Azure-specific provider implementations.
- Mock providers must remain available.
- Provider selection must support at least Mock and Azure.
- Do not commit real Azure keys, tokens, endpoints containing secrets, connection strings, or credentials.
- Do not log raw source text, translated text, transcripts, audio filenames, audio content, or full request bodies.
- Do not change frontend files unless a contract-breaking issue is discovered and approved.
- Do not add auth, database, TTS endpoint, streaming, deployment, CI/CD, or production hosting work.

Expected endpoints to preserve:

- GET /api/languages
- POST /api/translate/text
- POST /api/translate/audio

Expected acceptance:

- Azure text translation provider implemented behind ITextTranslationProvider.
- Azure speech-to-text provider implemented behind ISpeechToTextProvider.
- Mock provider mode still works.
- Azure provider mode can be selected through configuration.
- Missing Azure config fails safely when provider is Azure.
- Invalid provider config fails safely.
- Backend documentation explains User Secrets/environment variable setup.
- Tests/build pass.
- Planning/docs are updated.
- No secrets are committed.

After dry run, stop and wait for approval.
```

---

## 23. Completion Report Template

When finished, the Builder should report:

```markdown
# Sprint 005 Completion Report

## Summary

## Files Created

## Files Modified

## Packages Added

## Commands Run

## Tests / Validation

## Azure Live Validation

## Acceptance Criteria Status

## Decisions Added

## Risks Added Or Updated

## Questions Added Or Resolved

## Known Limitations

## Recommended Next Sprint
```

---

## 24. Recommended Next Sprint

Recommended Sprint 006 after Sprint 005:

**Sprint 006 — Audio Capture UX / Push-To-Talk**

Likely scope:

- Browser microphone recording via MediaRecorder.
- Push-to-talk UX.
- Client-side audio duration display/limit.
- More robust audio upload/recording validation.
- Better audio error states.

Do not start Sprint 006 until Sprint 005 is accepted.
