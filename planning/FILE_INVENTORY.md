# File Inventory

**Project:** My Translation App

---

## Root Files

| File | Purpose |
|------|---------|
| README.md | Project overview |
| AGENTS.md | Agent behaviour rules |
| CLAUDE.md | Claude Code adapter |
| CODEX.md | Codex adapter |
| project-start.md | Project start reference |
| llm-project-instructions.md | Paste into ChatGPT / Claude project settings as the Architect context |
| architect-chat-starter-prompt.md | Starter prompt for Architect chat |
| claude-code-builder-prompt.md | Builder prompt for Claude Code after Architect Pack is ready |
| architect-pack-001-discovery.md | Architect Pack for Sprint 001 — Discovery Architecture |
| architect-pack-002-implementation-architecture.md | Architect Pack for Sprint 002 — Implementation Architecture |
| architect-pack-003-backend-api-skeleton.md | Architect Pack for Sprint 003 — Backend API Skeleton |

---

## Planning Files

| File | Purpose |
|------|---------|
| planning/INTAKE.md | Architect intake context |
| planning/STATE.md | Current project state and active sprint |
| planning/DOMAIN.md | Domain notes: business problem, users, MVP workflow, terminology |
| planning/DECISIONS.md | Durable project and architecture decisions |
| planning/RISKS.md | Product, technical, privacy, API, and implementation risks |
| planning/QUESTIONS.md | Open questions requiring client or stakeholder input |
| planning/FILE_INVENTORY.md | This file — tracks all project files |

---

## Sprint Files

| File | Purpose |
|------|---------|
| planning/sprints/001-discovery-architecture/requirements.md | Sprint 001 requirements |
| planning/sprints/001-discovery-architecture/blueprint.md | Sprint 001 documentation plan |
| planning/sprints/001-discovery-architecture/acceptance.md | Sprint 001 acceptance criteria |
| planning/sprints/001-discovery-architecture/handoff-prompt.md | Sprint 001 Builder handoff prompt |
| planning/sprints/002-implementation-architecture/requirements.md | Sprint 002 requirements |
| planning/sprints/002-implementation-architecture/blueprint.md | Sprint 002 documentation steps |
| planning/sprints/002-implementation-architecture/acceptance.md | Sprint 002 acceptance criteria |
| planning/sprints/002-implementation-architecture/handoff-prompt.md | Sprint 002 Builder handoff prompt |
| planning/sprints/003-backend-api-skeleton/requirements.md | Sprint 003 requirements |
| planning/sprints/003-backend-api-skeleton/blueprint.md | Sprint 003 Builder implementation plan |
| planning/sprints/003-backend-api-skeleton/acceptance.md | Sprint 003 acceptance criteria |
| planning/sprints/003-backend-api-skeleton/handoff-prompt.md | Sprint 003 Builder handoff prompt |

---

## Documentation Files

| File | Purpose |
|------|---------|
| docs/ARCHITECTURE.md | MVP implementation architecture — frontend/backend structures, service boundaries, provider abstractions, request flows, state management, sequencing (updated Sprint 002) |
| docs/API.md | Planned API endpoints, DTO conventions, DTO versioning guidance, error codes, provider boundary rules (updated Sprint 002) |
| docs/VALIDATION.md | Validation rules for text, audio, language, MIME types, logging constraints, test strategy (updated Sprint 002) |
| docs/DATA_MODEL.md | Data model notes (scaffold — populated in a future sprint if persistence added) |
| docs/PERMISSIONS.md | Permissions notes (scaffold — populated if auth is added) |
| docs/SECURITY.md | Security notes (scaffold) |
| docs/ENGINEERING_PRINCIPLES.md | Engineering principles (scaffold) |
| docs/ARCHITECT_PACK_WORKFLOW.md | How Architect Packs are created and applied |

---

## Backend Source Files (Sprint 003)

| File | Purpose |
|------|---------|
| src/backend/MyTranslationApp.sln | .NET 8 solution file |
| src/backend/README.md | Local development startup instructions |
| src/backend/MyTranslationApp.Api/MyTranslationApp.Api.csproj | API project — HTTP layer |
| src/backend/MyTranslationApp.Api/Program.cs | Composition root — DI registration, middleware pipeline |
| src/backend/MyTranslationApp.Api/appsettings.json | Non-secret application configuration |
| src/backend/MyTranslationApp.Api/appsettings.Development.json | Development log level overrides |
| src/backend/MyTranslationApp.Api/Controllers/TranslationController.cs | POST /api/translate/text and POST /api/translate/audio |
| src/backend/MyTranslationApp.Api/Controllers/LanguagesController.cs | GET /api/languages |
| src/backend/MyTranslationApp.Api/Middleware/CorrelationIdMiddleware.cs | X-Correlation-ID injection middleware |
| src/backend/MyTranslationApp.Api/Middleware/ExceptionHandlingMiddleware.cs | Safe error envelope middleware |
| src/backend/MyTranslationApp.Application/MyTranslationApp.Application.csproj | Application project — business logic |
| src/backend/MyTranslationApp.Application/Interfaces/ISpeechToTextProvider.cs | Speech-to-text provider interface |
| src/backend/MyTranslationApp.Application/Interfaces/ITextTranslationProvider.cs | Text translation provider interface |
| src/backend/MyTranslationApp.Application/Interfaces/ITextToSpeechProvider.cs | TTS provider interface (future placeholder) |
| src/backend/MyTranslationApp.Application/Interfaces/ILanguageCatalogService.cs | Language catalog service interface |
| src/backend/MyTranslationApp.Application/Services/TranslationService.cs | Application service orchestrating translation and transcription |
| src/backend/MyTranslationApp.Application/DTOs/TextTranslationRequestDto.cs | Text translation inbound DTO |
| src/backend/MyTranslationApp.Application/DTOs/TextTranslationResponseDto.cs | Text translation outbound DTO |
| src/backend/MyTranslationApp.Application/DTOs/AudioTranslationResponseDto.cs | Audio translation outbound DTO |
| src/backend/MyTranslationApp.Application/DTOs/ApiErrorResponseDto.cs | Error envelope DTO |
| src/backend/MyTranslationApp.Application/DTOs/ApiErrorDetailDto.cs | Error detail DTO (nested in error envelope) |
| src/backend/MyTranslationApp.Application/DTOs/LanguageOptionDto.cs | Single language entry DTO |
| src/backend/MyTranslationApp.Application/DTOs/LanguageListResponseDto.cs | Language list response DTO |
| src/backend/MyTranslationApp.Application/Validation/ErrorCodes.cs | Centralised error code string constants |
| src/backend/MyTranslationApp.Application/Validation/ValidationResult.cs | Validation result model |
| src/backend/MyTranslationApp.Application/Validation/TranslationValidationOptions.cs | Configuration-bound validation limits |
| src/backend/MyTranslationApp.Application/Validation/TranslationRequestValidator.cs | Text and audio request validation logic |
| src/backend/MyTranslationApp.Infrastructure/MyTranslationApp.Infrastructure.csproj | Infrastructure project — provider implementations |
| src/backend/MyTranslationApp.Infrastructure/Providers/MockTextTranslationProvider.cs | Mock text translation provider |
| src/backend/MyTranslationApp.Infrastructure/Providers/MockSpeechToTextProvider.cs | Mock speech-to-text provider |
| src/backend/MyTranslationApp.Infrastructure/Providers/MockTextToSpeechProvider.cs | Mock TTS provider (placeholder stub) |
| src/backend/MyTranslationApp.Infrastructure/Providers/StaticLanguageCatalogService.cs | Static language catalog (10 common languages) |

---

## Backend Test Files (Sprint 003)

| File | Purpose |
|------|---------|
| tests/backend/MyTranslationApp.Tests/MyTranslationApp.Tests.csproj | xUnit test project |
| tests/backend/MyTranslationApp.Tests/Validation/TranslationValidationTests.cs | Validation unit tests (20 tests) |
| tests/backend/MyTranslationApp.Tests/Services/TranslationServiceTests.cs | Service unit tests (5 tests) |
| tests/backend/MyTranslationApp.Tests/Controllers/TranslationControllerTests.cs | Controller integration tests using WebApplicationFactory (10 tests) |

---

## Sprint Files (Sprint 004)

| File | Purpose |
|------|---------|
| planning/sprints/004-frontend-mvp-shell/requirements.md | Sprint 004 requirements |
| planning/sprints/004-frontend-mvp-shell/blueprint.md | Sprint 004 Builder implementation plan |
| planning/sprints/004-frontend-mvp-shell/acceptance.md | Sprint 004 acceptance criteria |
| planning/sprints/004-frontend-mvp-shell/handoff-prompt.md | Sprint 004 Builder handoff prompt |

---

## Frontend Source Files (Sprint 004)

| File | Purpose |
|------|---------|
| src/frontend/README.md | Local development startup instructions |
| src/frontend/package.json | Frontend dependencies and scripts |
| src/frontend/index.html | Vite HTML entry point |
| src/frontend/vite.config.ts | Vite config — /api proxy to localhost:5074 |
| src/frontend/tsconfig.json | TypeScript project references root |
| src/frontend/tsconfig.app.json | TypeScript config for application source |
| src/frontend/tsconfig.node.json | TypeScript config for Vite config file |
| src/frontend/.env.example | Documents VITE_API_BASE_URL (no secrets) |
| src/frontend/src/vite-env.d.ts | Vite client type declarations |
| src/frontend/src/main.tsx | React app entry point |
| src/frontend/src/App.tsx | Root component — language loading, state routing |
| src/frontend/src/types/api.ts | TypeScript types matching backend DTOs |
| src/frontend/src/api/translationApi.ts | API client for all three backend endpoints |
| src/frontend/src/components/LanguageSelect.tsx | Reusable language selector component |
| src/frontend/src/components/TextTranslationForm.tsx | Text translation form with validation |
| src/frontend/src/components/AudioTranslationForm.tsx | Audio translation form — Record (push-to-talk, default) and Upload File tabs |
| src/frontend/src/components/ResultPanel.tsx | Translation result display (text and audio) |
| src/frontend/src/components/ErrorPanel.tsx | Structured error display |
| src/frontend/src/styles/app.css | MVP styles |

---

## Frontend Source Files (Sprint 006)

| File | Purpose |
|------|---------|
| src/frontend/src/services/AudioCaptureService.ts | MIME detection, MediaRecorder lifecycle, blob generation |
| src/frontend/src/components/PushToTalkButton.tsx | Record/Stop toggle button — idle, recording, uploading states |
| src/frontend/src/components/RecordingIndicator.tsx | Pulsing indicator shown during active recording |
| src/frontend/src/components/RecordingTimer.tsx | Elapsed/max time counter during recording |

---

## Phase 0 Test Tool (Sprint 006) — DELETED in Sprint 012

| File | Purpose |
|------|---------|
| ~~src/frontend/public/phase0-test.html~~ | Deleted in Sprint 012 — was a development-only audio validation page; removed before production build |

---

## Architect Packs (Sprint 004)

| File | Purpose |
|------|---------|
| architect-pack-004-frontend-mvp-shell.md | Architect Pack for Sprint 004 — Frontend MVP Shell |

---

## Sprint Files (Sprint 005)

| File | Purpose |
|------|---------|
| planning/sprints/005-azure-provider-integration/requirements.md | Sprint 005 requirements |
| planning/sprints/005-azure-provider-integration/blueprint.md | Sprint 005 Builder implementation plan |
| planning/sprints/005-azure-provider-integration/acceptance.md | Sprint 005 acceptance criteria |
| planning/sprints/005-azure-provider-integration/handoff-prompt.md | Sprint 006 Builder handoff prompt |

---

## Architect Packs (Sprint 005)

| File | Purpose |
|------|---------|
| architect-pack-005-azure-provider-integration.md | Architect Pack for Sprint 005 — Azure Provider Integration |

---

## Backend Source Files (Sprint 005)

| File | Purpose |
|------|---------|
| src/backend/MyTranslationApp.Application/Exceptions/ProviderException.cs | Provider exception type with ErrorCode property (Application layer) |
| src/backend/MyTranslationApp.Infrastructure/Configuration/AzureTranslationOptions.cs | Azure Translator config model (Endpoint, Region, Key, IsConfigured) |
| src/backend/MyTranslationApp.Infrastructure/Configuration/AzureSpeechOptions.cs | Azure Speech config model (Region, Key, IsConfigured) |
| src/backend/MyTranslationApp.Infrastructure/Providers/Azure/AzureTextTranslationProvider.cs | Azure Translator adapter implementing ITextTranslationProvider |
| src/backend/MyTranslationApp.Infrastructure/Providers/Azure/AzureSpeechToTextProvider.cs | Azure Speech STT adapter implementing ISpeechToTextProvider |

## Backend Test Files (Sprint 005)

| File | Purpose |
|------|---------|
| tests/backend/MyTranslationApp.Tests/Providers/ProviderSelectionTests.cs | Provider selection and startup validation tests (15 tests) |

---

## Reference / Support Files

| File | Purpose |
|------|---------|
| references/README.md | Reference materials directory |
| samples/README.md | Sample data directory |
| scripts/README.md | Scripts directory |
| scripts/apply-architect-pack.js | Script for applying Architect Packs |
| src/README.md | Source code directory (empty — no production code yet) |
| planning/meetings/README.md | Meetings and notes directory |

---

## Sprint Files (Sprint 012)

| File | Purpose |
|------|---------|
| planning/sprints/012-aws-production-deployment/requirements.md | Sprint 012 requirements |
| planning/sprints/012-aws-production-deployment/blueprint.md | Sprint 012 deployment steps and architecture decisions |
| planning/sprints/012-aws-production-deployment/acceptance.md | Sprint 012 acceptance criteria and production validation results |
| planning/sprints/012-aws-production-deployment/handoff-prompt.md | Sprint 012 handoff prompt |

---

## Architect Packs (Sprint 012)

| File | Purpose |
|------|---------|
| architect-pack-012-aws-production-deployment.md | Architect Pack for Sprint 012 — AWS Production Deployment |

---

## Documentation Files (Sprint 012)

| File | Purpose |
|------|---------|
| docs/PRODUCTION_DEPLOYMENT_REPORT.md | Sprint 012 production deployment report — URLs, validation results, architecture, lessons learned |

---

## Architect Packs (Sprint 013)

| File | Purpose |
|------|---------|
| architect-pack-013-conversation-mode.md | Architect Pack for Sprint 013 — Conversation Mode |

---

## Frontend Source Files (Sprint 013)

| File | Purpose |
|------|---------|
| src/frontend/src/types/conversation.ts | `ConversationMessage` TypeScript interface |
| src/frontend/src/components/ConversationMode.tsx | Conversation Mode container — language selectors, turn orchestration, auto-play |
| src/frontend/src/components/ConversationHistory.tsx | Scrollable conversation history list |
| src/frontend/src/components/ConversationInput.tsx | Active speaker display, text input, audio file upload, auto-play toggle |
| src/frontend/src/components/ConversationMessage.tsx | Individual conversation message bubble |

---

## Modified Files (Sprint 013)

| File | Change |
|------|---------|
| src/frontend/src/App.tsx | Added mode toggle nav (Translation / Conversation); conditionally renders ConversationMode |
| src/frontend/src/styles/app.css | Added conversation mode styles (nav, history, message bubbles, input, mobile) |

---

## To Be Discovered

- Implementation repository location (GitHub / TBD)
- Provider SDK packages (Azure SDK, etc. — after provider choice confirmed)
- Environment configuration files (.env structure — after implementation sprint)
