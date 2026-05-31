# Sprint 012 — Blueprint

**Sprint:** 012 — AWS Production Deployment
**Status:** Complete
**Completed:** 2026-05-31

---

## Architecture Pattern Selected

Same-origin proxy via CloudFront (deviation from Sprint 011 separate-origin documentation).

**Reason:** EB SingleInstance does not support HTTPS without a custom domain. CloudFront proxying both frontend and API avoids mixed-content browser blocks and eliminates CORS for API calls.

---

## Deployment Steps Executed

### Step 1 — Pre-Deployment Validation
- Local Azure mode validated: health, languages, translation (en→es/cs/zh), TTS, STT round-trip
- phase0-test.html removed from `src/frontend/public/`
- 133/133 tests confirmed passing

### Step 2 — Backend Publish
```bash
dotnet publish src/backend/MyTranslationApp.Api/MyTranslationApp.Api.csproj \
  --configuration Release --runtime linux-x64 --self-contained false --output ./publish
```
Output: 4.18 MB zip

### Step 3 — IAM Setup
Created `aws-elasticbeanstalk-ec2-role` IAM role + instance profile with policies:
- AWSElasticBeanstalkWebTier
- AWSElasticBeanstalkMulticontainerDocker
- AWSElasticBeanstalkWorkerTier

### Step 4 — EB Deployment
- EB Application: `my-translation-app`
- EB Environment: `my-translation-api-prod` (SingleInstance, .NET 8 on AL2023)
- Zip uploaded to: `s3://elasticbeanstalk-ap-southeast-2-185512089178/my-translation-app/my-translation-api-v1.zip`
- 13 environment variables configured (9 app + 2 runtime + 1 CORS + 1 health)

### Step 5 — S3 Setup
- Bucket: `my-translation-app-frontend` (ap-southeast-2)
- Public access: blocked
- Frontend built with `VITE_API_BASE_URL=""` (same-origin)
- 3 files synced: `index.html`, `assets/index-*.css`, `assets/index-*.js`

### Step 6 — CloudFront Distribution
- OAC: `EB1W02JTGY0HR`
- Distribution ID: `EOSQIHDJHIZ82`
- Domain: `d2ftspeokj49uq.cloudfront.net`
- Default behavior: `/*` → S3 (CachingOptimized)
- Behavior `/api/*` → EB (CachingDisabled, AllViewerExceptHostHeader, all methods)
- Behavior `/health` → EB (CachingDisabled, GET/HEAD only)
- Error pages: 403 + 404 → `/index.html` HTTP 200

### Step 7 — CORS Update
Updated EB `AllowedCorsOrigins__0` to `https://d2ftspeokj49uq.cloudfront.net`

### Step 8 — End-to-End Validation
All checks passed via CloudFront HTTPS.
