# AWS Architecture

**Project:** My Translation App
**Sprint:** 011 — AWS Deployment Preparation
**Date:** 2026-05-31

---

## Architecture Overview

```
User (Browser)
       │ HTTPS
       ▼
┌─────────────────────┐
│   Amazon CloudFront │  CDN + HTTPS termination + caching
│   (Global Edge)     │
└──────────┬──────────┘
           │ S3 origin
           ▼
┌─────────────────────┐
│   Amazon S3         │  Static file hosting — Vite dist/
│   (us-east-1)       │
└─────────────────────┘

User (Browser JS)
       │ HTTPS cross-origin (CORS)
       │ VITE_API_BASE_URL/api/...
       ▼
┌─────────────────────┐
│ Elastic Beanstalk   │  .NET 8 API — Amazon Linux 2023
│ Load Balancer       │  Health check: GET /health
│ .NET 8 Application  │  Kestrel on port 5000
└──────────┬──────────┘
           │ HTTPS — outbound to Azure
           ▼
┌─────────────────────────────────────────────────┐
│ Azure Cognitive Services (unchanged AI provider) │
│   Azure Translator  →  Text translation          │
│   Azure Speech STT  →  Speech-to-text            │
│   Azure Speech TTS  →  Text-to-speech            │
└─────────────────────────────────────────────────┘
```

**Pattern:** Separate-origin deployment. Frontend and backend on different AWS domains. Cross-cloud AI provider.

---

## Component Responsibilities

### Amazon S3

- Hosts the static frontend build (`src/frontend/dist/`)
- No public access — served exclusively through CloudFront
- Content updated by `aws s3 sync` on each frontend deployment
- Storage: < 1 MB of static assets

### Amazon CloudFront

- Global CDN — serves the React SPA with low latency worldwide
- Terminates HTTPS — required for production; no SSL config on S3
- Origin Access Control (OAC) — secures S3 access
- Custom error pages — routes 403/404 to `/index.html` for React Router
- **Must forward `Origin` header to backend** when the frontend makes CORS API calls (via Origin Request Policy — see docs/AWS_DEPLOYMENT.md)

### Elastic Beanstalk (.NET 8)

- Runs the .NET 8 backend API on Amazon Linux 2023
- Managed nginx reverse proxy forwards port 80 → port 5000 (Kestrel)
- Managed load balancer — health checks at `/health` (must be configured; default is `GET /`)
- Managed scaling — can add instances if traffic grows
- Environment variables — Azure credentials + CORS config injected at runtime, not committed

### Azure Cognitive Services (Cross-cloud AI provider)

- Azure Translator — text translation for all 37 languages
- Azure Speech STT — speech-to-text via Fast Transcription REST API
- Azure Speech TTS — text-to-speech via Neural voices
- Credentials held server-side in EB environment variables only — never exposed to browser or S3

---

## Cross-Cloud Architecture

This app uses AWS for hosting and Azure for AI services. This is an intentional design choice, not a constraint:

| Concern | Platform | Reason |
|---|---|---|
| Frontend hosting | AWS | S3+CloudFront: mature, low-cost, globally distributed |
| Backend hosting | AWS | EB: managed, familiar to project owner |
| Text translation | Azure | Azure Translator: high quality, 37-language coverage |
| Speech-to-text | Azure | Azure Speech: Fast Transcription API, broad format support |
| Text-to-speech | Azure | Azure Speech: Neural voices, 37 languages covered |

The backend is the trust boundary. Azure credentials never leave the EB environment. The browser never calls Azure directly.

**Portfolio value:** Demonstrates multi-cloud integration — an AWS/Azure hybrid architecture — which is a pattern used in real enterprise environments.

---

## Why Elastic Beanstalk over ECS/Fargate

| Concern | Elastic Beanstalk | ECS/Fargate |
|---|---|---|
| Setup complexity | Low — managed platform | High — requires task definitions, service config, VPC |
| Docker required | No — native .NET 8 platform | Yes |
| Cost at MVP scale | Equivalent | Slightly higher |
| Operational overhead | Low — AWS manages patching | Medium |
| Portfolio readiness | Good | Better for containerized workflows |

ECS/Fargate is the recommended upgrade path if the project later adds Docker, CI/CD pipelines, or needs stronger isolation. For an MVP first deployment, EB is appropriate and practical.

---

## Why S3 + CloudFront over AWS Amplify

| Concern | S3 + CloudFront | Amplify Hosting |
|---|---|---|
| Transparency | High — you see and control each component | Lower — Amplify abstracts the pipeline |
| Portfolio visibility | Demonstrates S3 and CloudFront knowledge separately | Shows Amplify usage |
| Custom control | Full — cache policy, origin headers, error pages | Partial |
| CI/CD integration | Must configure manually (or add later) | Built-in |
| Cost | Minimal (S3 + CF) | Slightly higher (Amplify management overhead) |

S3 + CloudFront is the professional choice for a portfolio project demonstrating AWS hosting knowledge.

---

## Data Flow — Text Translation

```
1. User types text, selects languages, clicks Translate
2. Frontend (CloudFront/S3) sends POST to VITE_API_BASE_URL/api/translate/text
3. Browser includes Origin: https://d1xxx.cloudfront.net header
4. CloudFront behavior forwards Origin header to EB (requires Origin Request Policy)
5. EB load balancer forwards to .NET Kestrel on port 5000
6. CorrelationIdMiddleware injects X-Correlation-ID
7. CORS middleware validates Origin against AllowedCorsOrigins
8. TranslationController validates request
9. AzureTextTranslationProvider calls Azure Translator API
10. Translation returned → EB → browser
11. Browser updates UI with translated text
12. User clicks Play → POST to /api/translate/tts → Azure Speech TTS → audio streamed back
```

---

## Environment Variables — Quick Reference

All EB environment variables use `__` as the hierarchy separator (maps to `:` in ASP.NET Core config).

| Environment Variable | Maps To | Required |
|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | — | Yes (`Production`) |
| `ASPNETCORE_URLS` | — | Yes (`http://+:5000`) |
| `Translation__Provider` | `Translation:Provider` | Yes (`Azure`) |
| `AzureTranslator__Key` | `AzureTranslator:Key` | Yes (secret) |
| `AzureTranslator__Region` | `AzureTranslator:Region` | Yes |
| `AzureSpeech__Key` | `AzureSpeech:Key` | Yes (secret) |
| `AzureSpeech__Region` | `AzureSpeech:Region` | Yes |
| `AzureSpeech__Endpoint` | `AzureSpeech:Endpoint` | Yes |
| `AllowedCorsOrigins__0` | `AllowedCorsOrigins[0]` | Yes (CloudFront domain) |

`Translation:Provider = "Azure"` is also set in `appsettings.Production.json` — the EB env var is belt-and-suspenders.

---

## Security Boundaries

| Boundary | Enforcement |
|---|---|
| Azure credentials never in browser | Backend proxy pattern — all provider calls server-side |
| Azure credentials not in source control | EB environment variables only |
| HTTPS for all traffic | CloudFront enforces HTTPS on frontend; EB load balancer should enforce HTTPS for backend |
| CORS restricts browser access | `AllowedCorsOrigins` checked for every cross-origin request |
| S3 not publicly accessible | CloudFront OAC — S3 bucket has no public access |
| Health endpoint never exposes internals | `GET /health` returns static `{ "status": "healthy" }` only |

---

## Future Upgrade Path

| Concern | Current (MVP) | Future |
|---|---|---|
| Secrets | EB environment variables | AWS SSM Parameter Store or Secrets Manager |
| Backend hosting | Elastic Beanstalk | ECS/Fargate with Docker |
| HTTPS backend | HTTP within VPC | ACM certificate + HTTPS load balancer |
| CI/CD | Manual deploy | GitHub Actions or AWS CodePipeline |
| Custom domain | Generated AWS/CF domains | Route 53 with ACM certificate |
| Monitoring | `/health` endpoint | CloudWatch, AWS X-Ray |
| Database | None (session-only) | Amazon RDS if user history added |
