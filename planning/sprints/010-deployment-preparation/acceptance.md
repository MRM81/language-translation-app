# Sprint 010 Acceptance — Deployment Readiness Hardening

## Health Endpoint

- [x] `GET /health` returns HTTP 200.
- [x] Response body is `{ "status": "healthy" }`.
- [x] Endpoint works in Mock mode (no Azure credentials required).
- [x] Endpoint works regardless of active provider.

## Configuration

- [x] CORS origins are config-driven (`AllowedCorsOrigins` in `appsettings.json`).
- [x] Default CORS origin (`http://localhost:5173`) preserved — no dev behavior change.
- [x] `appsettings.Production.json` created: `Translation:Provider = "Azure"`, Warning log levels.
- [x] `AzureSpeech:Endpoint` placeholder added to `appsettings.json`.
- [x] `launchSettings.json` stale `launchUrl: "swagger"` removed.

## Startup

- [x] Startup log emits `Provider mode: {provider}` on every start.
- [x] Startup validation unchanged and still enforced (confirmed from existing tests).

## Documentation

- [x] `docs/DEPLOYMENT.md` — prerequisites, build, both deployment patterns, checklist, verification.
- [x] `docs/ENVIRONMENTS.md` — dev/test/production configs, CORS guide, secret management, AllowedHosts note.
- [x] `docs/OPERATIONS.md` — health endpoint, startup validation, provider switching, troubleshooting, logging, correlation IDs.
- [x] `src/backend/README.md` — `AzureSpeech:Endpoint` added to User Secrets setup and configuration reference.

## Tests

- [x] Backend build — 0 warnings, 0 errors.
- [x] Backend tests — 133/133 pass (129 prior + 4 new health endpoint tests).
- [x] No secrets committed.

## Planning

- [x] `planning/STATE.md` updated.
- [x] `planning/DECISIONS.md` — D-078 to D-081 added.
- [x] `planning/RISKS.md` — R-045 to R-047 added.
- [x] `planning/QUESTIONS.md` — Q-041 to Q-044 added.
