# Sprint 010 Requirements — Deployment Readiness Hardening

## Revised Goal

Harden the application for deployment. Much of the groundwork (startup validation, secrets management, safe logging) was already in place from earlier sprints. Sprint 010 fills the remaining gaps.

## Scope Correction (Recorded)

The Architect Pack was originally titled "Deployment Preparation." The dry run found that:

- Startup validation already existed in `Program.cs` (Azure mode throws `InvalidOperationException` for missing credentials before accepting any requests)
- Secrets management via User Secrets / environment variables was already in place
- Safe logging (no user content in logs) was already enforced

The sprint was retitled "Deployment Readiness Hardening" to reflect the actual scope: filling gaps rather than building from scratch.

## In Scope

- `GET /health` endpoint (HTTP 200, `{ "status": "healthy" }`)
- Config-driven CORS origins (`AllowedCorsOrigins` array in appsettings)
- Startup provider log (`Provider mode: Azure/Mock`)
- `appsettings.Production.json` (provider selection, log levels)
- `launchSettings.json` cleanup (remove stale `launchUrl: "swagger"`)
- `AzureSpeech:Endpoint` placeholder added to `appsettings.json` (documentation gap from Sprint 005.2)
- `AzureSpeech:Endpoint` added to `src/backend/README.md` User Secrets setup and config table
- `docs/DEPLOYMENT.md` — prerequisites, build, deployment patterns, checklist, verification
- `docs/ENVIRONMENTS.md` — dev/test/production configs, secret management, CORS guide
- `docs/OPERATIONS.md` — health endpoint, startup validation, troubleshooting, logging, correlation IDs

## Out Of Scope

- Docker / container deployment
- CI/CD pipelines
- Azure App Service or specific hosting platform configuration
- Static file serving for frontend (hosting pattern not yet decided — Q-041)
- Kubernetes, auto-scaling, monitoring platforms
- Authentication
- Infrastructure-as-Code
