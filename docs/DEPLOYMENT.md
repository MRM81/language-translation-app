# Deployment Guide

**Project:** My Translation App
**Sprint:** 010 — Deployment Preparation (updated Sprint 011)
**Date:** 2026-05-31

---

## AWS Deployment

**The application is deployed on AWS.** See the AWS-specific guides:

- [docs/AWS_DEPLOYMENT.md](AWS_DEPLOYMENT.md) — Step-by-step AWS deployment (S3 + CloudFront + Elastic Beanstalk)
- [docs/AWS_ARCHITECTURE.md](AWS_ARCHITECTURE.md) — Architecture diagram, component rationale, environment variable reference
- [docs/PRODUCTION_DEPLOYMENT_REPORT.md](PRODUCTION_DEPLOYMENT_REPORT.md) — Sprint 012 deployment record, URLs, validation results

### Live Production Environment (Sprint 012)

| Resource | URL |
|---|---|
| Frontend (CloudFront) | https://d2ftspeokj49uq.cloudfront.net |
| Backend (Elastic Beanstalk) | http://my-translation-api-prod.eba-pahyptkw.ap-southeast-2.elasticbeanstalk.com |
| AWS Region | ap-southeast-2 (Sydney) |

**Architecture pattern:** Same-origin proxy — CloudFront routes `/api/*` and `/health` to EB, `/*` to S3. The frontend calls the same CloudFront domain (no CORS). See [docs/PRODUCTION_DEPLOYMENT_REPORT.md](PRODUCTION_DEPLOYMENT_REPORT.md) for full details.

The sections below document the general build and configuration approach applicable to any hosting target.

---

## Prerequisites

| Dependency | Minimum Version | Purpose |
|---|---|---|
| .NET SDK | 8.0 | Backend build and runtime |
| Node.js | 18.x | Frontend build |
| npm | 9.x | Frontend package management |
| Azure Translator resource | — | Text translation (production) |
| Azure AI Services resource | — | Speech-to-text and text-to-speech (production) |

Verify installed versions:

```bash
dotnet --version    # Must show 8.x.x
node --version      # Must show 18.x or higher
npm --version
```

---

## Build

### Backend

```bash
cd src/backend
dotnet build --configuration Release
```

Publishes to `bin/Release/net8.0/`. For a self-contained deployment artifact:

```bash
dotnet publish MyTranslationApp.Api/MyTranslationApp.Api.csproj \
  --configuration Release \
  --output ./publish
```

### Frontend

```bash
cd src/frontend
npm install
npm run build
```

Output is written to `src/frontend/dist/`.

---

## Configuration

All configuration follows the standard ASP.NET Core layering (lowest to highest precedence):

```
appsettings.json                  ← base defaults
appsettings.{Environment}.json    ← environment overrides
Environment variables             ← deployment credentials
```

**Set `ASPNETCORE_ENVIRONMENT=Production`** on the hosting platform. This activates `appsettings.Production.json`, which sets `Translation:Provider = "Azure"` and appropriate log levels.

### Required environment variables (Azure mode)

| Variable | Description |
|---|---|
| `AzureTranslator__Key` | Azure Translator API key |
| `AzureTranslator__Region` | Azure region (e.g. `eastus`) |
| `AzureSpeech__Key` | Azure Speech API key |
| `AzureSpeech__Region` | Azure Speech region |
| `AzureSpeech__Endpoint` | Azure AI Services base URL (e.g. `https://my-resource.cognitiveservices.azure.com`) |

Note: Use double underscores (`__`) as the separator for nested keys in environment variables.

### Optional environment variables

| Variable | Default | Description |
|---|---|---|
| `AllowedCorsOrigins__0` | `http://localhost:5173` | Allowed CORS origin for the frontend. Set to the production frontend URL. |

For multiple origins, use indexed keys: `AllowedCorsOrigins__0`, `AllowedCorsOrigins__1`, etc.

---

## Deployment Patterns

Two patterns are supported depending on hosting choice (Q-041).

### Pattern A — Same-Origin (Recommended for simplicity)

Both frontend and backend served from the same domain. No CORS configuration needed.

```
https://yourdomain.com/          → frontend (static files)
https://yourdomain.com/api/...   → backend API
https://yourdomain.com/health    → health check
```

Implementation: configure the hosting platform to serve `src/frontend/dist/` as static files from the same .NET process (add `UseStaticFiles()` to `Program.cs`) or via a reverse proxy that maps `/` to the frontend and `/api/` to the backend.

`AllowedCorsOrigins` does not need to be set — same-origin requests don't trigger CORS.

### Pattern B — Separate Origins

Frontend on CDN (Vercel, Netlify, Azure Static Web Apps) and backend on App Service or container.

```
https://my-translation-app.vercel.app  → frontend
https://my-translation-api.azurewebsites.net → backend
```

Set on the backend:
```
AllowedCorsOrigins__0 = https://my-translation-app.vercel.app
```

Set on the frontend build:
```
VITE_API_BASE_URL = https://my-translation-api.azurewebsites.net
```

---

## Deployment Checklist

Before deploying:

- [ ] `ASPNETCORE_ENVIRONMENT` is set to `Production`
- [ ] All 5 Azure environment variables are set (Key, Region, Endpoint for Speech; Key and Region for Translator)
- [ ] `AllowedCorsOrigins` is configured for production frontend URL (Pattern B) or omitted (Pattern A)
- [ ] No secrets in `appsettings.json` or source control (placeholder values only)
- [ ] Backend build succeeds: `dotnet build --configuration Release`
- [ ] All tests pass: `dotnet test --configuration Release`
- [ ] Frontend build succeeds: `npm run build`

After deploying:

- [ ] `GET /health` returns `{ "status": "healthy" }` — HTTP 200
- [ ] `GET /api/languages` returns 37 languages
- [ ] `POST /api/translate/text` returns a real translation
- [ ] Startup log shows `Provider mode: Azure`
- [ ] No secrets appear in any log output

---

## Verification

Once deployed, verify the application:

```bash
# Health check
curl https://yourdomain.com/health
# Expected: { "status": "healthy" }

# Language list
curl https://yourdomain.com/api/languages
# Expected: 37 languages with capability flags

# Text translation (requires Azure credentials active)
curl -X POST https://yourdomain.com/api/translate/text \
  -H "Content-Type: application/json" \
  -d '{"sourceText":"Hello","sourceLanguage":"en","targetLanguage":"es"}'
# Expected: { "translatedText": "Hola", "provider": "azure", ... }
```

---

## Running Tests

```bash
dotnet test tests/backend/MyTranslationApp.Tests --configuration Release
```

All 133 tests use Mock mode. No Azure credentials required.

---

## Rollback

If a deployment fails, revert to the previous artifact. Environment variables are the only external dependency — code changes are fully self-contained in the published binary and `dist/` folder.
