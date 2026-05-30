# Environments

**Project:** My Translation App
**Sprint:** 010 — Deployment Preparation
**Date:** 2026-05-31

---

## Overview

The application supports three environments: Development, Test, and Production. Environment selection is controlled by `ASPNETCORE_ENVIRONMENT`.

---

## Development

**Purpose:** Local development and feature work.

**Provider:** Mock (default). No Azure credentials required.

**Activation:** `ASPNETCORE_ENVIRONMENT=Development` (set automatically by `launchSettings.json`).

**Configuration files active:**
```
appsettings.json                ← base defaults
appsettings.Development.json    ← Debug log level
User Secrets                    ← real Azure credentials (if testing Azure mode locally)
```

**Starting the backend:**
```bash
cd src/backend/MyTranslationApp.Api
dotnet run
# API at http://localhost:5074
```

**Starting the frontend:**
```bash
cd src/frontend
npm install
npm run dev
# UI at http://localhost:5173
```

The Vite dev server proxies `/api/*` to `http://localhost:5074` — no CORS configuration needed in dev.

**To use Azure mode locally:** set User Secrets (see `src/backend/README.md`).

---

## Test

**Purpose:** Automated test suite (CI or local).

**Provider:** Mock. All 133 tests pin `Translation:Provider=Mock` via `WebApplicationFactory.UseSetting`.

**No Azure credentials required.** Tests are safe to run in any environment.

```bash
dotnet test tests/backend/MyTranslationApp.Tests --configuration Release
```

---

## Production

**Purpose:** Live deployment for real users.

**Provider:** Azure (set in `appsettings.Production.json`).

**Activation:** `ASPNETCORE_ENVIRONMENT=Production`.

**Configuration files active:**
```
appsettings.json                ← base defaults
appsettings.Production.json     ← Provider=Azure, Warning log level
Environment variables           ← Azure credentials (injected by hosting platform)
```

### Required production configuration

All five values must be set as environment variables (never in committed files):

```
ASPNETCORE_ENVIRONMENT = Production

AzureTranslator__Key    = <Azure Translator API key>
AzureTranslator__Region = <region, e.g. eastus>

AzureSpeech__Key        = <Azure Speech API key>
AzureSpeech__Region     = <region, e.g. eastus>
AzureSpeech__Endpoint   = <resource base URL, e.g. https://my-resource.cognitiveservices.azure.com>
```

### CORS configuration

Set the frontend origin on the backend. Two patterns:

**Pattern A — Same-origin (frontend + backend on same domain):**
No CORS configuration needed. `AllowedCorsOrigins` not required.

**Pattern B — Separate origins:**
```
AllowedCorsOrigins__0 = https://your-frontend-domain.com
```

On the frontend build, set:
```
VITE_API_BASE_URL = https://your-backend-api.com
```

### Production log levels (`appsettings.Production.json`)

| Namespace | Level | Rationale |
|---|---|---|
| Default | Warning | Framework noise suppressed |
| Microsoft.AspNetCore | Warning | HTTP pipeline noise suppressed |
| MyTranslationApp | Information | Application events retained |

Application events at Information include: provider mode at startup, request start/complete per endpoint, validation failures, provider exceptions (type only — no content).

### Startup validation

When `Translation:Provider = "Azure"`, the application validates on startup:

- `AzureTranslator:Key` — non-empty
- `AzureTranslator:Region` — non-empty
- `AzureSpeech:Key` — non-empty
- `AzureSpeech:Region` — non-empty
- `AzureSpeech:Endpoint` — non-empty

If any value is missing, the application throws `InvalidOperationException` at startup and does not accept any requests. The error message names the missing configuration key.

Placeholder values (e.g. `DO_NOT_COMMIT_REAL_KEY`) are non-empty and pass startup validation — they will fail on the first provider call. See D-045.

---

## Secret Management

| Environment | Secret mechanism | Committed? |
|---|---|---|
| Development | .NET User Secrets (`dotnet user-secrets`) | Never — stored outside the repo |
| Test | None (Mock mode — no secrets required) | N/A |
| Production | Hosting platform environment variables | Never — injected at runtime |

`appsettings.json` contains placeholder strings only (`YOUR_REGION_HERE`, `DO_NOT_COMMIT_REAL_KEY`, `https://YOUR_RESOURCE_NAME.cognitiveservices.azure.com`). These are safe to commit.

`.gitignore` excludes `.env`, `.env.*` (except `.env.example`) and all credential file patterns.

---

## AllowedHosts

`AllowedHosts: "*"` in `appsettings.json` allows any host header. For production, consider restricting this to the known domain via an environment variable or `appsettings.Production.json` override, depending on the hosting platform's reverse proxy configuration.

For most Azure App Service deployments, the platform handles host header validation at the load balancer level. Review this with the hosting team before go-live.
