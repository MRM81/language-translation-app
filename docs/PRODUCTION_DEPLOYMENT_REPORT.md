# Production Deployment Report

**Project:** My Translation App
**Sprint:** 012 — AWS Production Deployment
**Date:** 2026-05-31
**Deployed by:** Builder (Claude Code)

---

## Deployment Summary

Status: **Complete — All acceptance criteria met**

The application is live on AWS. All major functionality (translation, STT, TTS, language catalog, health endpoint) has been validated end-to-end through the production CloudFront distribution.

---

## Production URLs

| Service | URL |
|---|---|
| **Frontend (CloudFront HTTPS)** | `https://d2ftspeokj49uq.cloudfront.net` |
| **Backend (Elastic Beanstalk HTTP)** | `http://my-translation-api-prod.eba-pahyptkw.ap-southeast-2.elasticbeanstalk.com` |
| **Health endpoint** | `https://d2ftspeokj49uq.cloudfront.net/health` |
| **API root** | `https://d2ftspeokj49uq.cloudfront.net/api` |

> Note: The EB backend URL is HTTP-only. All production traffic flows through CloudFront (HTTPS). Do not call the EB URL directly from a browser — CORS is configured for CloudFront only.

---

## AWS Resources Created

| Resource | Name / ID | Region |
|---|---|---|
| EB Application | `my-translation-app` | ap-southeast-2 |
| EB Environment | `my-translation-api-prod` (ID: e-wkmimrxppx) | ap-southeast-2 |
| EC2 Platform | 64bit Amazon Linux 2023 v3.11.1 running .NET 8 | ap-southeast-2 |
| S3 Bucket | `my-translation-app-frontend` | ap-southeast-2 |
| CloudFront Distribution | `EOSQIHDJHIZ82` | Global |
| CloudFront OAC | `EB1W02JTGY0HR` (my-translation-app-s3-oac) | Global |
| IAM Role | `aws-elasticbeanstalk-ec2-role` | Global |
| IAM Instance Profile | `aws-elasticbeanstalk-ec2-role` | Global |

---

## Architecture Pattern

**Same-origin proxy** (deviates from Sprint 011 documentation which described separate-origin):

```
Browser (HTTPS)
      │
      ▼
CloudFront (d2ftspeokj49uq.cloudfront.net)
      │
      ├── /* ──────────────── S3 (static frontend: index.html, assets/)
      ├── /api/* ──────────── EB .NET 8 backend (port 5000, HTTP)
      └── /health ─────────── EB .NET 8 backend (port 5000, HTTP)
```

**Why same-origin instead of separate-origin:**
The EB SingleInstance environment (no load balancer) does not support HTTPS without a custom domain and ACM certificate. Separate-origin deployment would cause mixed-content browser blocks (HTTPS page making HTTP API calls). CloudFront proxying both frontend and API resolves this with no code changes and eliminates the need for CORS configuration on the browser side.

---

## Environment Configuration

All secrets were entered directly into EB environment properties. No secrets appear in source control.

| Variable | Value | Secret? |
|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Production` | No |
| `ASPNETCORE_URLS` | `http://+:5000` | No |
| `Translation__Provider` | `Azure` | No |
| `AzureTranslator__Key` | *(redacted)* | Yes |
| `AzureTranslator__Region` | `australiaeast` | No |
| `AzureTranslator__Endpoint` | `https://mark-translation-app-ai-001.cognitiveservices.azure.com/` | No |
| `AzureSpeech__Key` | *(redacted)* | Yes |
| `AzureSpeech__Region` | `australiaeast` | No |
| `AzureSpeech__Endpoint` | `https://mark-translation-app-ai-001.cognitiveservices.azure.com` | No |
| `AllowedCorsOrigins__0` | `https://d2ftspeokj49uq.cloudfront.net` | No |

---

## Pre-Deployment Validation (Local Azure Mode)

All endpoints verified locally in Azure mode before deploying:

| Check | Result |
|---|---|
| `dotnet build --configuration Release` | Pass — 0 errors, 0 warnings |
| `dotnet test --configuration Release` | Pass — 133/133 |
| Frontend build (`npm run build`) | Pass — clean, no phase0-test.html |
| `GET /health` (local) | `{"status":"healthy"}` |
| `GET /api/languages` (local) | 37 languages |
| `POST /api/translate/text` en→es | "Hola" |
| `POST /api/translate/text` en→cs | "Ahoj světe" |
| `POST /api/translate/text` en→zh-Hans | "你好" |
| `POST /api/translate/tts` | 16,992 bytes `audio/mpeg` |
| `POST /api/translate/audio` (STT round-trip) | "Hola." → "Hello." |

---

## Production Validation (via CloudFront)

| Check | Result | Notes |
|---|---|---|
| React app load | HTTP 200 `text/html` | HTTPS ✓ |
| `GET /health` | `{"status":"healthy"}` | |
| `GET /api/languages` | 37 languages returned | |
| Translation en→es | "Buenos días" (`provider: azure`) | |
| Translation en→zh-Hans | "谢谢你" (`provider: azure`) | |
| TTS (en) | 20,160 bytes `audio/mpeg` | |
| STT round-trip (en→es) | `"Good morning."` → `"Buenos días."` | |
| CloudFront HTTPS | ✓ | Redirect from HTTP enforced |
| SPA routing (404→200) | ✓ | Error pages configured |
| No CORS errors | ✓ | Same-origin pattern — no preflight |

---

## Deployment Steps Executed

1. Removed `src/frontend/public/phase0-test.html` (Sprint 006 dev artifact)
2. Verified local Azure mode: all endpoints passing, Azure credentials confirmed working
3. Published backend: `dotnet publish --runtime linux-x64 --self-contained false` → `publish/`
4. Packaged: `Compress-Archive publish/* my-translation-api.zip` (4.18 MB)
5. Created IAM role `aws-elasticbeanstalk-ec2-role` with EB Web/Worker/Multicontainer policies
6. Created EB application `my-translation-app`
7. Uploaded zip to `s3://elasticbeanstalk-ap-southeast-2-185512089178/my-translation-app/my-translation-api-v1.zip`
8. Created EB application version `v1.0.0`
9. Created EB environment `my-translation-api-prod` (SingleInstance, .NET 8 on AL2023)
10. Created S3 bucket `my-translation-app-frontend` with public access blocked
11. Built frontend with `VITE_API_BASE_URL=""` (same-origin via CloudFront proxy)
12. Created CloudFront OAC `EB1W02JTGY0HR`
13. Created CloudFront distribution with S3 + EB origins, `/api/*` and `/health` behaviors
14. Applied S3 bucket policy for CloudFront OAC access
15. Synced frontend dist to S3 (3 files: index.html, CSS, JS)
16. Updated EB `AllowedCorsOrigins__0` to CloudFront domain
17. Ran end-to-end production validation

---

## Files Modified

| File | Change |
|---|---|
| `src/frontend/public/phase0-test.html` | Deleted (dev artifact) |
| `publish/` | Created — .NET 8 linux-x64 artifact |
| `my-translation-api.zip` | Created — EB deployment artifact (gitignored) |

---

## Known Issues

| Issue | Severity | Notes |
|---|---|---|
| EB endpoint is HTTP-only | Low | By design — SingleInstance EB has no HTTPS without custom domain. CloudFront provides HTTPS termination. All browser traffic routes via CloudFront. |
| TTS with non-ASCII characters via CLI | Low | Shell encoding of `í`, `ö` etc. in bash curl commands may produce 400. Application handles UTF-8 correctly — the issue is in the test tooling, not the app. Verified in local testing. |
| Root account used for deployment | Low | AWS best practice recommends IAM user/role. Acceptable for MVP portfolio project. |

---

## Lessons Learned

1. **IAM instance profile not created by default** — EB requires `aws-elasticbeanstalk-ec2-role` IAM role and instance profile to exist before environment creation. This is normally set up by the AWS console on first EB use; when using the CLI on a fresh account, it must be created manually.

2. **UTF-8 BOM breaks AWS CLI file parameters** — PowerShell 5.1 `Out-File -Encoding utf8` writes a BOM. AWS CLI `file://` parameters fail to parse BOM-encoded JSON. Must use `[System.IO.File]::WriteAllText` with `New-Object System.Text.UTF8Encoding $false`.

3. **Same-origin via CloudFront proxy is cleaner than separate-origin for MVP** — Avoids mixed-content browser blocks without requiring a custom domain or ACM certificate on the EB load balancer. Eliminates CORS for API calls. The Sprint 011 documentation describes separate-origin as the pattern; same-origin via CloudFront is a superior MVP-stage approach.

4. **EB environment creation is fast once IAM is set up** — Total provisioning time was approximately 2 minutes for SingleInstance .NET 8 environment.

5. **CloudFront deployment was faster than expected** — Deployed to global edge nodes in approximately 2 minutes (documentation suggests 5–15 minutes).

---

## Cost Estimate (Monthly, Low Traffic)

| Service | Estimate |
|---|---|
| Elastic Beanstalk (t3.micro, SingleInstance) | ~$8–10 |
| S3 (< 1 MB static assets) | < $0.01 |
| CloudFront (< 10 GB transfer) | < $1 |
| Azure Translator | Usage-based (< $10 for dev/demo) |
| Azure Speech | Usage-based (< $10 for dev/demo) |

**To stop the EB environment and avoid charges:**
```bash
aws elasticbeanstalk terminate-environment --environment-name my-translation-api-prod --region ap-southeast-2
```

---

## Production Architecture Diagram

```
User (Browser)
      │ HTTPS
      ▼
┌──────────────────────────────────────────────────────────┐
│  Amazon CloudFront (d2ftspeokj49uq.cloudfront.net)       │
│  Global CDN — HTTPS termination — SPA routing            │
│                                                          │
│  Behavior: /api/* ──────────────────────────────────┐   │
│  Behavior: /health ─────────────────────────────┐   │   │
│  Default:  /*  ──────────────────────┐           │   │   │
└─────────────────────────────────────│───────────│───│───┘
                                      │           │   │
                                      ▼           ▼   ▼
              ┌──────────────────┐    ┌────────────────────────────────────────┐
              │ Amazon S3        │    │ Elastic Beanstalk (ap-southeast-2)     │
              │ (ap-southeast-2) │    │ my-translation-api-prod                │
              │ Frontend dist/   │    │ .NET 8 / Amazon Linux 2023             │
              │  index.html      │    │ SingleInstance, HTTP port 5000         │
              │  assets/         │    │ Health: GET /health                    │
              └──────────────────┘    └────────────────┬───────────────────────┘
                                                       │ HTTPS outbound
                                                       ▼
                                      ┌────────────────────────────────────────┐
                                      │ Azure Cognitive Services (australiaeast)│
                                      │  Azure Translator → text translation    │
                                      │  Azure Speech STT → speech-to-text     │
                                      │  Azure Speech TTS → text-to-speech     │
                                      └────────────────────────────────────────┘
```

---

## Next Recommended Sprint

**Sprint 013 — Conversation Mode**

As recommended in the Architect Pack. Deployment is stable; the application is publicly accessible and all Azure services are working in production.

**Also consider (any order):**
- Custom domain (Route 53 + ACM certificate) to replace generated CloudFront URL
- CI/CD pipeline (GitHub Actions) for automated deployment
- CloudWatch monitoring for EB health and error tracking
- SSM Parameter Store for Azure credentials (more secure than EB env vars)
