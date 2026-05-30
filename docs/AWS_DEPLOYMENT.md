# AWS Deployment Guide

**Project:** My Translation App
**Sprint:** 011 — AWS Deployment Preparation
**Date:** 2026-05-31

---

## Overview

This guide covers deploying My Translation App to AWS using:

- **Frontend:** Amazon S3 + CloudFront (static Vite build)
- **Backend:** AWS Elastic Beanstalk running .NET 8 on Amazon Linux 2023
- **AI Provider:** Azure Translator + Azure Speech (unchanged — cross-cloud)
- **Secrets:** Elastic Beanstalk environment variables (MVP) or AWS SSM Parameter Store (recommended for maturity)

See [docs/AWS_ARCHITECTURE.md](AWS_ARCHITECTURE.md) for architecture rationale and tradeoffs.

---

## Prerequisites

| Tool | Install |
|---|---|
| AWS CLI v2 | https://docs.aws.amazon.com/cli/latest/userguide/install-cliv2.html |
| EB CLI | `pip install awsebcli` |
| .NET 8 SDK | https://dotnet.microsoft.com/download/dotnet/8.0 |
| Node.js 18+ | https://nodejs.org |
| AWS account | With IAM permissions for S3, CloudFront, Elastic Beanstalk |

Configure the AWS CLI:

```bash
aws configure
# Enter: AWS Access Key, Secret, Region, Output format (json)
```

---

## Part 1 — Backend: Elastic Beanstalk

### Step 1 — Publish the Backend

From the project root:

```bash
dotnet publish src/backend/MyTranslationApp.Api/MyTranslationApp.Api.csproj \
  --configuration Release \
  --runtime linux-x64 \
  --self-contained false \
  --output ./publish
```

This produces a framework-dependent build targeting Linux. The .NET 8 runtime is provided by the EB platform.

### Step 2 — Package for Elastic Beanstalk

EB requires a zip of the published output:

```bash
# PowerShell
Compress-Archive -Path ./publish/* -DestinationPath ./my-translation-api.zip -Force

# Bash
cd publish && zip -r ../my-translation-api.zip . && cd ..
```

### Step 3 — Create the Elastic Beanstalk Application

In the AWS Console:

1. Open **Elastic Beanstalk** → **Create Application**
2. Application name: `my-translation-app`
3. Platform: **.NET on Linux** → Platform branch: **.NET 8 on Amazon Linux 2023**
4. Application code: Upload `my-translation-api.zip`

Or using the EB CLI (from the `publish/` directory):

```bash
cd publish
eb init my-translation-app --platform "dotnet-linux/coreclr-8.0" --region us-east-1
eb create my-translation-api-prod
```

### Step 4 — Configure Elastic Beanstalk Environment Variables

In AWS Console → Elastic Beanstalk → Environment → **Configuration** → **Software** → **Environment properties**:

| Key | Value |
|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ASPNETCORE_URLS` | `http://+:5000` |
| `Translation__Provider` | `Azure` |
| `AzureTranslator__Key` | `<your Azure Translator key>` |
| `AzureTranslator__Region` | `<region, e.g. eastus>` |
| `AzureSpeech__Key` | `<your Azure Speech key>` |
| `AzureSpeech__Region` | `<region, e.g. eastus>` |
| `AzureSpeech__Endpoint` | `https://your-resource.cognitiveservices.azure.com` |
| `AllowedCorsOrigins__0` | `https://your-cloudfront-domain.cloudfront.net` |

**Important:** Set `ASPNETCORE_URLS=http://+:5000`. EB's reverse proxy (nginx) forwards to port 5000. The backend defaults to port 5074 in development — this environment variable overrides it for production.

**Never** enter Azure API keys in source control. Set them only in the EB environment properties console or via SSM (see Secrets section below).

### Step 5 — Configure the Health Check Path

Elastic Beanstalk's default health check target is `GET /`. The backend has no route at `/` and would return 404.

In AWS Console → EB Environment → **Configuration** → **Load balancer** → **Processes** → Edit the default process:

- **Health check path:** `/health`
- **HTTP codes:** `200`

Or using the EB CLI / `.ebextensions/healthcheck.config` (optional):

```yaml
option_settings:
  aws:elasticbeanstalk:application:
    Application Healthcheck URL: /health
```

After configuring, the EB health dashboard should show **Ok** once the application starts.

### Step 6 — Verify Backend

Once the EB environment is healthy:

```bash
# Replace with your EB environment URL
EB_URL=http://my-translation-api-prod.us-east-1.elasticbeanstalk.com

# Health check
curl $EB_URL/health
# Expected: { "status": "healthy" }

# Language list
curl $EB_URL/api/languages
# Expected: 37 languages with capability flags

# Text translation
curl -X POST $EB_URL/api/translate/text \
  -H "Content-Type: application/json" \
  -d '{"sourceText":"Hello","sourceLanguage":"en","targetLanguage":"es"}'
# Expected: { "translatedText": "Hola", "provider": "azure", ... }
```

Startup log should confirm: `Provider mode: Azure`

---

## Part 2 — Frontend: S3 + CloudFront

### Step 1 — Build the Frontend for Production

```bash
cd src/frontend

# Set the backend URL to your EB environment URL (no trailing slash)
VITE_API_BASE_URL=https://your-backend.elasticbeanstalk.com npm run build

# Or create .env.production.local (excluded from git) and run npm run build
# File contents: VITE_API_BASE_URL=https://your-backend.elasticbeanstalk.com
```

Build output is in `src/frontend/dist/`. Verify it contains `index.html` and an `assets/` folder.

### Step 2 — Create an S3 Bucket

```bash
aws s3 mb s3://my-translation-app-frontend --region us-east-1
```

Do not enable public access — CloudFront will serve the content.

### Step 3 — Upload the Frontend Build

```bash
aws s3 sync src/frontend/dist/ s3://my-translation-app-frontend --delete
```

### Step 4 — Create a CloudFront Distribution

In AWS Console → **CloudFront** → **Create distribution**:

| Setting | Value |
|---|---|
| **Origin domain** | Select your S3 bucket |
| **Origin access** | **Origin access control (OAC)** — create new OAC |
| **Default root object** | `index.html` |
| **Viewer protocol policy** | Redirect HTTP to HTTPS |
| **Cache policy** | CachingOptimized |

After creation, update the S3 bucket policy to allow CloudFront OAC access (AWS will generate the policy statement — apply it).

**Configure the error page for SPA routing:**

CloudFront → Distribution → **Error pages** → Create custom error response:

| HTTP error code | Response page path | HTTP response code |
|---|---|---|
| 403 | `/index.html` | 200 |
| 404 | `/index.html` | 200 |

This ensures React Router handles client-side routes correctly.

### Step 5 — Configure CORS on the Backend (Critical)

The CloudFront distribution has its own domain (e.g. `d1abc123xyz.cloudfront.net`). The browser sends this as the `Origin` header on API requests. The backend must allow it.

Update the EB environment variable:

```
AllowedCorsOrigins__0 = https://d1abc123xyz.cloudfront.net
```

**CloudFront Origin header forwarding — required for CORS:**

By default CloudFront does not forward the `Origin` request header to the backend, which causes CORS preflight failures. You must configure CloudFront to forward `Origin`.

In AWS Console → CloudFront → Distribution → **Behaviors** → Edit the default behavior:

1. **Cache policy:** Use a policy that includes `Origin` in the cache key — or create a custom policy.
2. **Origin request policy:** Choose **CORS-CustomOrigin** (AWS managed) — this forwards `Origin`, `Access-Control-Request-Headers`, and `Access-Control-Request-Method` to the backend.

Without this step, `OPTIONS` preflight requests will not carry the `Origin` header to the backend and CORS will fail silently.

### Step 6 — Verify Frontend

```bash
# Replace with your CloudFront domain
CF_URL=https://d1abc123xyz.cloudfront.net

# Should load the React app
curl -I $CF_URL
# Expected: HTTP/2 200, Content-Type: text/html

# API call from the frontend should reach the backend
# (test in the browser — open DevTools → Network tab, look for /api/languages)
```

---

## Part 3 — Secrets Management

### MVP: EB Environment Variables

For the first deployment, setting Azure credentials directly as EB environment variables is acceptable. They are encrypted at rest and not exposed in source control.

In AWS Console → EB Environment → Configuration → Software → Environment properties, set all Azure keys.

**Limitation:** Credentials appear in plain text in the AWS console and EB configuration history.

### Recommended: AWS Systems Manager Parameter Store

For improved security and auditability, store Azure credentials in SSM and reference them from EB:

```bash
aws ssm put-parameter \
  --name "/my-translation-app/production/AzureTranslatorKey" \
  --value "YOUR_REAL_KEY" \
  --type "SecureString"
```

Then configure EB to pull from SSM (requires an IAM role with `ssm:GetParameter` permission on the EB instance profile). SSM Parameter Store provides audit logging, version history, and stricter access control.

---

## CORS Configuration Reference

| Scenario | Backend AllowedCorsOrigins | Notes |
|---|---|---|
| Local dev (Vite proxy) | `http://localhost:5173` (default) | No CORS needed — Vite proxies the request |
| CloudFront production | `https://d1xxx.cloudfront.net` | Set as EB env var `AllowedCorsOrigins__0` |
| Custom domain | `https://yourdomain.com` | Replace CloudFront domain if using Route 53 custom domain |
| Multiple origins | `AllowedCorsOrigins__0`, `AllowedCorsOrigins__1` | Array — add each origin as a separate EB env var |

---

## Deployment Checklist

### Pre-deployment

- [ ] Azure credentials available and tested (not expired, correct region)
- [ ] `ASPNETCORE_ENVIRONMENT=Production` will be set on EB
- [ ] `ASPNETCORE_URLS=http://+:5000` will be set on EB
- [ ] `VITE_API_BASE_URL` set to the EB backend URL before frontend build
- [ ] `AllowedCorsOrigins__0` set to the CloudFront domain
- [ ] Backend build clean: `dotnet build --configuration Release`
- [ ] All 133 tests pass: `dotnet test --configuration Release`
- [ ] Frontend build clean: `VITE_API_BASE_URL=https://... npm run build`
- [ ] No secrets in `dist/`, `publish/`, or any committed file

### Post-deployment

- [ ] `GET /health` returns HTTP 200 on EB URL
- [ ] EB environment health shows **Ok**
- [ ] `GET /api/languages` returns 37 languages on EB URL
- [ ] CloudFront URL loads the React app (HTTPS)
- [ ] Translation works end-to-end in the browser
- [ ] Browser DevTools shows no CORS errors
- [ ] Startup log on EB shows `Provider mode: Azure`

---

## Rollback

### Backend Rollback

In AWS Console → EB Environment → **Application versions** → select a previous version → **Deploy**.

Or using EB CLI:

```bash
eb deploy --version <previous-version-label>
```

### Frontend Rollback

Re-upload a previous `dist/` build to S3:

```bash
aws s3 sync ./previous-dist/ s3://my-translation-app-frontend --delete
aws cloudfront create-invalidation \
  --distribution-id YOUR_DISTRIBUTION_ID \
  --paths "/*"
```

The CloudFront invalidation clears cached content globally (takes ~1 minute).

---

## Cost Notes

Running costs (estimated for minimal MVP usage):

| Service | Estimated monthly cost |
|---|---|
| Elastic Beanstalk (t3.small) | ~$15–$20 |
| S3 (< 1 GB static assets) | < $1 |
| CloudFront (low traffic) | < $1 |
| Azure Translator (per character) | Usage-based |
| Azure Speech (per second) | Usage-based |

**Stop or delete EB environment when not in use** to avoid ongoing charges. S3 and CloudFront have negligible cost at low traffic levels.

To stop the EB environment:

```bash
eb terminate my-translation-api-prod
```
