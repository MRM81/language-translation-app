# Architect Pack: Sprint 011 — AWS Deployment Preparation

## Pack Metadata

| Field | Value |
|---|---|
| Project Name | My Translation App |
| Sprint Number | 011 |
| Sprint Name | AWS Deployment Preparation |
| Created Date | 2026-05-31 |
| Architect | ChatGPT Architect Layer |
| Builder Target | Claude Code / Codex / Cursor |
| Status | Ready For Builder Dry Run |
| Depends On | Sprint 010 — Deployment Preparation |

---

## 1. Project Context

My Translation App is now an MVP-level translation platform with:

- Azure Translator integration
- Azure Speech-to-Text integration
- Azure Text-to-Speech integration
- 37 supported languages
- Capability metadata
- Responsive React frontend
- .NET backend API
- Health endpoint
- Config-driven CORS
- Production appsettings
- 133 automated tests passing
- Deployment, environment, and operations documentation

Sprint 010 prepared the application for deployment in a cloud-agnostic way. The remaining blocking deployment decision was Q-041: hosting target.

The project owner has AWS experience and no Azure deployment experience. Therefore, Sprint 011 will prepare the project for AWS hosting while still using Azure Cognitive Services as the AI provider.

This sprint is not the live deployment itself. It prepares the repository, configuration, documentation, and deployment path so the next Builder sprint can deploy safely.

---

## 2. Sprint Goal

Prepare My Translation App for AWS deployment by documenting and configuring a practical AWS hosting architecture, deployment strategy, environment variable model, frontend/backend separation, and AWS-specific deployment checklist.

Recommended target architecture:

```text
React Frontend
    ↓
S3 + CloudFront
    ↓
.NET Backend API
    ↓
Elastic Beanstalk or ECS/Fargate
    ↓
Azure Translator + Azure Speech Services
```

Primary recommendation for MVP deployment:

```text
Frontend: Amazon S3 + CloudFront
Backend: AWS Elastic Beanstalk for .NET
Secrets: AWS Systems Manager Parameter Store or Elastic Beanstalk environment variables
```

---

## 3. Problem Being Solved

The app is deployment-ready in principle, but not yet prepared for a concrete hosting target.

Open concerns:

- AWS hosting pattern not documented
- Frontend production API base URL strategy not finalized
- Backend AWS environment variable mapping not documented
- CORS production origin not finalized
- Secrets management path not selected
- Build/deploy commands not tailored to AWS
- No AWS deployment checklist exists
- No rollback or verification steps exist for AWS

This sprint resolves deployment preparation for AWS without performing the actual deployment.

---

## 4. Requirements

| ID | Requirement | Priority | Notes |
|---|---|---|---|
| R-001 | Document AWS deployment architecture | Must | Include frontend, backend, secrets, CORS, Azure AI dependency |
| R-002 | Recommend MVP AWS hosting pattern | Must | Prefer S3 + CloudFront frontend, Elastic Beanstalk backend |
| R-003 | Document backend environment variables for AWS | Must | Azure provider settings, CORS origins, ASPNETCORE_ENVIRONMENT |
| R-004 | Document frontend production API URL strategy | Must | Vite environment variable approach |
| R-005 | Prepare deployment checklist | Must | Build, configure, deploy, verify, rollback |
| R-006 | Document AWS secrets strategy | Must | EB env vars acceptable for MVP; SSM Parameter Store preferred for maturity |
| R-007 | Validate existing health endpoint works for AWS health checks | Must | GET /health from Sprint 010 |
| R-008 | Preserve existing local development flow | Must | No regression to localhost setup |
| R-009 | Avoid introducing new infrastructure complexity | Must | No Terraform, Kubernetes, or full CI/CD unless already present |
| R-010 | Update planning and docs | Must | State, decisions, risks, questions, deployment docs |

---

## 5. In Scope

### Documentation

- AWS deployment architecture
- AWS deployment checklist
- AWS environment variable guide
- Frontend hosting strategy
- Backend hosting strategy
- Secrets management strategy
- Rollback and verification steps

### Configuration Review

- Vite production API base URL handling
- Backend CORS environment variable handling
- Backend health endpoint compatibility with AWS health checks
- appsettings.Production.json compatibility with AWS

### Optional Minimal Code Changes

Only if required after dry run:

- Add or document `VITE_API_BASE_URL`
- Add or document frontend production build behavior
- Add sample AWS environment variable files without secrets
- Add `.env.production.example` if useful

---

## 6. Out Of Scope

Builder must not implement:

- Actual AWS deployment
- Terraform
- CloudFormation
- CDK
- Kubernetes
- ECS/Fargate unless explicitly approved after dry run
- Full CI/CD pipeline
- Authentication
- Database
- New product features
- Azure App Service deployment
- Provider rewrites
- New translation workflows

---

## 7. Assumptions

| ID | Assumption | Confidence | Action If Wrong |
|---|---|---|---|
| A-001 | Project owner prefers AWS hosting because of existing AWS experience | High | Revisit hosting target |
| A-002 | Azure Cognitive Services remain the AI provider | High | Do not migrate provider |
| A-003 | Backend is deployable as a .NET web API | High | Validate project structure |
| A-004 | Frontend is deployable as static Vite build output | High | Validate build output path |
| A-005 | MVP can use Elastic Beanstalk before ECS/Fargate | Medium | Document ECS/Fargate as future option |
| A-006 | Secrets will not be committed to repo | High | Use environment variables or AWS secret stores |

---

## 8. Constraints

- No secrets in source control
- No breaking API changes
- No product feature changes
- Existing tests must pass
- Local development must remain unchanged
- CORS must remain config-driven
- AWS preparation must not remove Azure provider integration
- Deployment docs must be practical for a first AWS deployment

---

## 9. Dependencies

| Dependency | Type | Status | Notes |
|---|---|---|---|
| Sprint 010 `/health` endpoint | Backend | Available | Used for AWS health checks |
| Config-driven CORS | Backend | Available | Needed for CloudFront/S3 frontend origin |
| Azure service credentials | Secret | External | Required at deployment time, not stored |
| AWS account | Platform | External | User has AWS experience |
| Vite frontend build | Frontend | Available | Needs production API URL strategy |
| .NET backend publish output | Backend | Available / verify | Needed for EB packaging |

---

## 10. Files To Read First

Builder must read:

1. `AGENTS.md`
2. `planning/STATE.md`
3. `planning/DECISIONS.md`
4. `planning/RISKS.md`
5. `planning/QUESTIONS.md`
6. `planning/FILE_INVENTORY.md`
7. `docs/DEPLOYMENT.md`
8. `docs/ENVIRONMENTS.md`
9. `docs/OPERATIONS.md`
10. `docs/ARCHITECTURE.md`
11. `docs/API.md`
12. `docs/VALIDATION.md`
13. `src/backend/MyTranslationApp.Api/Program.cs`
14. `src/backend/MyTranslationApp.Api/appsettings.json`
15. `src/backend/MyTranslationApp.Api/appsettings.Production.json`
16. `src/frontend/vite.config.ts`
17. `src/frontend/.env.example`, if present
18. `src/frontend/src/services/translationApi.ts`, or equivalent API client file

---

## 11. Files To Create Or Modify

### Create

| Path | Purpose |
|---|---|
| `docs/AWS_DEPLOYMENT.md` | AWS-specific deployment preparation guide |
| `docs/AWS_ARCHITECTURE.md` | AWS hosting architecture and tradeoffs |
| `planning/sprints/011-aws-deployment-preparation/requirements.md` | Sprint requirements |
| `planning/sprints/011-aws-deployment-preparation/blueprint.md` | Sprint implementation blueprint |
| `planning/sprints/011-aws-deployment-preparation/acceptance.md` | Acceptance checklist |
| `planning/sprints/011-aws-deployment-preparation/handoff-prompt.md` | Builder handoff prompt |

### Modify

| Path | Purpose |
|---|---|
| `docs/DEPLOYMENT.md` | Link or summarize AWS deployment path |
| `docs/ENVIRONMENTS.md` | Add AWS environment variable notes |
| `docs/OPERATIONS.md` | Add AWS verification and troubleshooting notes |
| `planning/STATE.md` | Mark Sprint 011 active/completed as appropriate |
| `planning/DECISIONS.md` | Add AWS deployment decisions |
| `planning/RISKS.md` | Add AWS deployment risks |
| `planning/QUESTIONS.md` | Resolve or add hosting/deployment questions |
| `planning/FILE_INVENTORY.md` | Track new docs and sprint files |

### Optional, only if justified by dry run

| Path | Purpose |
|---|---|
| `src/frontend/.env.production.example` | Document production frontend API URL variable |
| `src/frontend/src/services/translationApi.ts` | Only if API base URL is hardcoded and cannot be configured |
| `src/frontend/src/types/api.ts` | Only if no type-safe env handling exists and change is necessary |

---

## 12. Blueprint

### Step 1 — Confirm Current Deployment State

Builder should inspect:

- backend publishability
- frontend build output
- current API base URL handling
- current CORS configuration
- health endpoint behavior
- environment variable names
- existing deployment docs from Sprint 010

### Step 2 — Choose Recommended AWS MVP Pattern

Default recommendation:

```text
Frontend: S3 + CloudFront
Backend: Elastic Beanstalk running .NET API
Secrets: Elastic Beanstalk environment variables for MVP
Future: AWS Systems Manager Parameter Store or Secrets Manager
```

Document why this is preferred:

- simpler than ECS/Fargate
- matches project owner AWS experience
- avoids Kubernetes/IaC overbuild
- supports public portfolio deployment
- uses `/health` endpoint for EB health checks

### Step 3 — Define Production Environment Variables

Document backend values such as:

```text
ASPNETCORE_ENVIRONMENT=Production
Translation__Provider=Azure
AzureTranslator__Key=<from secret store>
AzureTranslator__Region=<region>
AzureSpeech__Key=<from secret store>
AzureSpeech__Region=<region>
AzureSpeech__Endpoint=<endpoint>
AllowedCorsOrigins__0=https://<cloudfront-domain>
```

Confirm exact key names from current options classes before documenting.

### Step 4 — Define Frontend Production API URL Strategy

Inspect current frontend API client.

Preferred pattern:

```text
VITE_API_BASE_URL=https://<backend-domain>
```

Frontend should call:

```text
${VITE_API_BASE_URL}/api/...
```

If this already exists, document it.

If it does not exist, Builder may add minimal support if low risk and covered by build checks.

### Step 5 — Document AWS Deployment Flow

AWS docs must include:

- prerequisites
- backend build/publish command
- frontend build command
- Elastic Beanstalk deployment outline
- S3 upload outline
- CloudFront invalidation notes
- CORS setup
- health check verification
- rollback checklist

### Step 6 — Update Planning Files

Add decisions:

- D-082: AWS selected as deployment target due to project owner experience
- D-083: S3 + CloudFront recommended for frontend hosting
- D-084: Elastic Beanstalk recommended for MVP backend hosting
- D-085: Azure Cognitive Services remain AI provider despite AWS hosting

Add risks:

- R-048: Cross-cloud dependency complexity
- R-049: CORS misconfiguration between CloudFront and backend
- R-050: Secrets misconfiguration in AWS deployment
- R-051: AWS hosting costs if resources are left running

Add or resolve questions:

- Q-041: hosting target resolved to AWS preparation path
- Q-045: exact backend AWS service for live deployment — EB default, ECS optional future
- Q-046: domain name required or use generated AWS/CloudFront domains?
- Q-047: whether to add CI/CD after first manual deployment

---

## 13. API / Integration Notes

No new application API endpoints should be added.

Existing endpoints remain:

- `GET /health`
- `GET /api/languages`
- translation endpoints
- existing TTS endpoint

AWS-specific integration points:

- Elastic Beanstalk health check should use `/health`
- CloudFront frontend origin will call backend API domain
- CORS must include the frontend CloudFront or custom domain origin
- Azure Cognitive Services keys remain configured through environment variables

---

## 14. Validation Plan

| Check | Method | Expected Result |
|---|---|---|
| Backend build | `dotnet build --configuration Release` | Success |
| Backend tests | `dotnet test --configuration Release` | All tests pass |
| Frontend type check | `cd src/frontend && npx tsc --noEmit` | Success |
| Frontend build | `cd src/frontend && npm run build` | Success |
| Health endpoint documentation | Review AWS docs | `/health` used for EB health checks |
| CORS documentation | Review env docs | CloudFront/custom domain origin documented |
| Secrets documentation | Review AWS docs | No secrets committed; env/SSM strategy documented |
| Local dev preservation | Review config | Localhost workflow unchanged |

---

## 15. Acceptance Criteria

Sprint is complete when:

- [ ] AWS deployment architecture is documented.
- [ ] Frontend S3 + CloudFront path is documented.
- [ ] Backend Elastic Beanstalk path is documented.
- [ ] Azure Cognitive Services remain documented as the AI provider.
- [ ] Backend AWS environment variables are documented.
- [ ] Frontend production API URL strategy is documented or minimally implemented if missing.
- [ ] CORS setup for AWS frontend/backend separation is documented.
- [ ] `/health` is documented as the AWS health check endpoint.
- [ ] Secrets strategy is documented with no secrets committed.
- [ ] Rollback and verification checklist exists.
- [ ] Existing build and tests pass.
- [ ] Sprint 011 planning files exist.
- [ ] `planning/STATE.md` updated.
- [ ] Decisions, risks, and questions are updated.
- [ ] No new product features are introduced.

---

## 16. Risks

| Risk | Impact | Likelihood | Mitigation | Owner |
|---|---|---|---|---|
| Cross-cloud architecture creates configuration complexity | Medium | Medium | Document clear environment variables and ownership boundaries | Architect / Builder |
| CloudFront origin not allowed by CORS | High | Medium | Config-driven `AllowedCorsOrigins` with explicit examples | Builder |
| Azure secrets misconfigured in AWS | High | Medium | Deployment checklist and startup validation | Builder |
| AWS resources generate unnecessary cost | Medium | Medium | Document cleanup and cost-control steps | Builder |
| Elastic Beanstalk packaging differs from local assumptions | Medium | Medium | Document publish process and leave actual deployment for next sprint | Builder |

---

## 17. Open Questions

| Question | Needed From | Blocking? | Notes |
|---|---|---|---|
| Should the first live deployment use Elastic Beanstalk or ECS/Fargate? | Owner / Architect | No | EB is default recommendation for MVP |
| Will a custom domain be used? | Owner | No | Generated AWS domains are acceptable for first deployment |
| Should CI/CD be added before or after first manual deployment? | Owner / Architect | No | Recommend after first manual deploy works |
| Should frontend and backend share a domain later? | Architect / Builder | No | Future deployment polish |

---

## 18. State Updates Required

At sprint completion, update:

- `planning/STATE.md`
- `planning/DECISIONS.md`
- `planning/RISKS.md`
- `planning/QUESTIONS.md`
- `planning/FILE_INVENTORY.md`
- active sprint acceptance status

Decision IDs should continue after Sprint 010:

```text
D-082
D-083
D-084
D-085
```

Risk IDs should continue after Sprint 010:

```text
R-048
R-049
R-050
R-051
```

Question IDs should continue after Sprint 010:

```text
Q-045
Q-046
Q-047
```

Q-041 should be updated as partially resolved or resolved depending on the documented AWS recommendation.

---

## 19. Builder Dry Run Instructions

Before implementation, Builder must produce a dry run report covering:

1. Files read.
2. Current frontend API base URL behavior.
3. Current backend CORS and environment configuration behavior.
4. Whether `.env.production.example` or code changes are needed.
5. Recommended AWS hosting pattern.
6. Files expected to change.
7. Commands to run.
8. Risks or blockers.
9. Whether implementation is safe to start.

Builder must not implement until dry run is reviewed.

---

## 20. Builder Handoff Prompt

```markdown
You are the Builder for My Translation App.

You are working on Sprint 011 — AWS Deployment Preparation.

Follow the Architect / Builder methodology.

Read these first:

1. AGENTS.md
2. planning/STATE.md
3. planning/DECISIONS.md
4. planning/RISKS.md
5. planning/QUESTIONS.md
6. planning/FILE_INVENTORY.md
7. docs/DEPLOYMENT.md
8. docs/ENVIRONMENTS.md
9. docs/OPERATIONS.md
10. docs/ARCHITECTURE.md
11. docs/API.md
12. docs/VALIDATION.md
13. Program.cs
14. appsettings.json
15. appsettings.Production.json
16. vite.config.ts
17. frontend API client files

Do not implement immediately.

First produce a dry run report that answers:

- How is the frontend API base URL currently configured?
- How is backend CORS currently configured?
- What AWS hosting pattern do you recommend for this MVP?
- Are code/config changes required, or docs only?
- Which files will you create or modify?
- What commands will you run?
- What risks or blockers exist?
- Is it safe to start implementation?

Sprint scope:

- Prepare AWS deployment documentation.
- Document S3 + CloudFront frontend hosting.
- Document Elastic Beanstalk backend hosting.
- Document AWS environment variables and secrets handling.
- Document CORS configuration for AWS.
- Document verification and rollback steps.
- Add minimal frontend environment variable support only if missing and low risk.

Do not perform actual AWS deployment.
Do not add Terraform, CloudFormation, CDK, Docker, ECS, Kubernetes, or CI/CD unless the Architect explicitly approves after dry run.
Do not change product features.
Do not change providers.
Do not commit secrets.

Stop after the dry run and wait for approval.
```

---

## 21. Completion Report Template

Builder completion report must include:

```markdown
# Sprint 011 Completion Report

## Summary

## Files Created

## Files Modified

## Commands Run

## Build / Test Results

## AWS Deployment Readiness Findings

## Decisions Added

## Risks Added Or Updated

## Questions Added Or Resolved

## Known Limitations

## Recommended Next Sprint
```

Recommended next sprint after completion:

```text
Sprint 012 — AWS Production Deployment
```

Alternative:

```text
Sprint 012 — Conversation Mode
```

Only choose Conversation Mode next if public deployment is deliberately deferred.
