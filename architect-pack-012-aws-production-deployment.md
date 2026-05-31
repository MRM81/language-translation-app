# Architect Pack: Sprint 012 — AWS Production Deployment

## Pack Metadata

| Field         | Value                     |
| ------------- | ------------------------- |
| Project Name  | My Translation App        |
| Sprint Number | 012                       |
| Sprint Name   | AWS Production Deployment |
| Status        | Ready For Builder         |
| Depends On    | Sprint 010, Sprint 011    |
| Created       | 2026-05-31                |

---

# 1. Project Context

The project is now deployment-ready.

Completed foundations include:

* Azure Translation integration
* Azure Speech-to-Text integration
* Azure Text-to-Speech integration
* 37-language support
* Health endpoint
* Production configuration strategy
* AWS deployment documentation
* AWS architecture documentation
* Configurable frontend API URL
* Configurable CORS
* 133 automated tests

Sprint 011 resolved Q-041 and selected:

```text
AWS S3 + CloudFront + Elastic Beanstalk
```

as the official deployment target.

This sprint performs the first real deployment.

---

# 2. Sprint Goal

Deploy the application to AWS and verify that all major functionality works in a live environment.

---

# 3. Problem Being Solved

The application is currently production-ready on paper but has never been deployed.

Real deployment validation is required for:

* environment configuration
* AWS integration
* Azure service connectivity
* frontend/backend communication
* CORS behavior
* health monitoring
* production diagnostics

---

# 4. Requirements

| ID    | Requirement                                 | Priority |
| ----- | ------------------------------------------- | -------- |
| R-001 | Deploy backend to Elastic Beanstalk         | Must     |
| R-002 | Deploy frontend to S3                       | Must     |
| R-003 | Configure CloudFront distribution           | Must     |
| R-004 | Configure production environment variables  | Must     |
| R-005 | Configure health check path                 | Must     |
| R-006 | Configure production CORS origins           | Must     |
| R-007 | Validate Azure Translation in production    | Must     |
| R-008 | Validate Azure Speech-to-Text in production | Must     |
| R-009 | Validate Azure Text-to-Speech in production | Must     |
| R-010 | Produce deployment verification report      | Must     |

---

# 5. In Scope

### AWS

* Elastic Beanstalk
* S3
* CloudFront
* Environment variables
* Health checks

### Validation

* Translation
* STT
* TTS
* Language catalog
* CORS
* Health endpoint

### Documentation

* Deployment results
* Environment configuration
* Production URLs

---

# 6. Out Of Scope

Do not implement:

* Docker
* ECS
* Fargate
* Terraform
* Kubernetes
* CI/CD pipelines
* Auto scaling tuning
* Monitoring platforms
* Authentication
* User accounts

---

# 7. Assumptions

| ID    | Assumption                                         |
| ----- | -------------------------------------------------- |
| A-001 | AWS account exists                                 |
| A-002 | Azure credentials are available                    |
| A-003 | DNS configuration is optional                      |
| A-004 | MVP deployment is acceptable without custom domain |

---

# 8. Constraints

* No secrets committed to source control
* No production credentials stored in project files
* Existing tests must remain passing
* Existing APIs must remain unchanged
* Deployment must be reproducible using documentation

---

# 9. Files To Read First

Builder must read:

```text
docs/AWS_DEPLOYMENT.md
docs/AWS_ARCHITECTURE.md
docs/DEPLOYMENT.md
docs/ENVIRONMENTS.md
docs/OPERATIONS.md

planning/STATE.md
planning/DECISIONS.md
planning/RISKS.md
planning/QUESTIONS.md
```

---

# 10. Files To Create

```text
docs/PRODUCTION_DEPLOYMENT_REPORT.md

planning/sprints/012-aws-production-deployment/
    requirements.md
    blueprint.md
    acceptance.md
    handoff-prompt.md
```

---

# 11. Files To Modify

```text
planning/STATE.md
planning/DECISIONS.md
planning/RISKS.md
planning/QUESTIONS.md
planning/FILE_INVENTORY.md

docs/DEPLOYMENT.md
docs/OPERATIONS.md
docs/ENVIRONMENTS.md
```

Only modify application code if deployment uncovers a real defect.

---

# 12. Blueprint

## Step 1 — Backend Deployment

Deploy:

```text
MyTranslationApp.Api
```

to:

```text
AWS Elastic Beanstalk
```

Configure:

```text
ASPNETCORE_ENVIRONMENT=Production

Translation__Provider=Azure

AzureTranslation__Key
AzureTranslation__Region

AzureSpeech__Key
AzureSpeech__Region
AzureSpeech__Endpoint

AllowedCorsOrigins__0=<CloudFront URL>
```

Configure:

```text
Health Check Path = /health
```

Verify:

```text
GET /health
```

returns:

```json
{
  "status": "healthy"
}
```

---

## Step 2 — Frontend Deployment

Build frontend using:

```text
VITE_API_BASE_URL=<Elastic Beanstalk URL>
```

Deploy:

```text
dist/
```

to:

```text
Amazon S3
```

Verify static hosting.

---

## Step 3 — CloudFront

Create distribution.

Verify:

* HTTPS works
* Static assets load
* Cache invalidation works

Document:

```text
CloudFront URL
```

---

## Step 4 — End-to-End Validation

Validate:

### Translation

```text
English → Spanish
English → Czech
English → Chinese
```

### Speech To Text

Upload sample audio.

Validate transcript.

### Text To Speech

Generate speech.

Validate playback.

### Language Catalog

Verify:

```text
37 languages returned
```

---

## Step 5 — Production Report

Create:

```text
docs/PRODUCTION_DEPLOYMENT_REPORT.md
```

Include:

* deployment date
* AWS URLs
* environment summary
* validation results
* known issues
* lessons learned

---

# 13. Validation Plan

| Check            | Expected Result |
| ---------------- | --------------- |
| dotnet build     | Pass            |
| dotnet test      | 133+ pass       |
| Frontend build   | Pass            |
| Health endpoint  | Healthy         |
| Translation      | Working         |
| STT              | Working         |
| TTS              | Working         |
| CORS             | Working         |
| CloudFront HTTPS | Working         |

---

# 14. Acceptance Criteria

Sprint complete when:

* [ ] Backend deployed
* [ ] Frontend deployed
* [ ] CloudFront configured
* [ ] Health endpoint verified
* [ ] Translation verified
* [ ] STT verified
* [ ] TTS verified
* [ ] 37-language catalog verified
* [ ] Production report created
* [ ] No secrets committed
* [ ] Planning files updated

---

# 15. Risks

| Risk                          | Impact | Mitigation                 |
| ----------------------------- | ------ | -------------------------- |
| Azure credential issues       | High   | Validate credentials early |
| CORS misconfiguration         | High   | Verify CloudFront origin   |
| CloudFront caching issues     | Medium | Invalidate cache           |
| Environment variable mistakes | High   | Document configuration     |

---

# 16. Open Questions

| ID    | Question                      |
| ----- | ----------------------------- |
| Q-045 | Custom domain required?       |
| Q-046 | Monitoring platform required? |
| Q-047 | CI/CD pipeline priority?      |

These do not block deployment.

---

# 17. State Updates Required

Add:

```text
D-086
D-087
D-088
D-089
```

Production deployment decisions.

Add:

```text
R-052
R-053
R-054
```

Production deployment risks.

Update:

```text
Q-045
Q-046
Q-047
```

as appropriate.

---

# 18. Builder Dry Run Instructions

Before deployment:

1. Read AWS deployment documentation.
2. Verify AWS account prerequisites.
3. Verify Azure credentials availability.
4. Verify deployment commands.
5. Identify deployment blockers.
6. Produce dry run report.
7. Wait for approval.

Do not deploy before dry run approval.

---

# 19. Builder Handoff Prompt

You are the Builder for My Translation App.

Sprint 012 — AWS Production Deployment.

Read all deployment documentation first.

Perform a deployment dry run before making changes.

Your objective is to deploy the application to AWS using:

* Elastic Beanstalk
* S3
* CloudFront

Validate:

* translation
* speech-to-text
* text-to-speech
* language catalog
* health endpoint

Create a production deployment report.

Do not commit secrets.

Do not introduce new features.

Wait for approval after the dry run before deploying.

---

# 20. Completion Report Requirements

Provide:

* AWS URLs
* deployment steps executed
* files modified
* environment variables configured
* validation results
* production issues discovered
* deployment report location
* decisions added
* risks added
* recommended next sprint

Recommended next sprint:

```text
Sprint 013 — Conversation Mode
```

if deployment succeeds.
