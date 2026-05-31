# Operations Guide

**Project:** My Translation App
**Sprint:** 010 — Deployment Preparation
**Date:** 2026-05-31

---

## Health Endpoint

```
GET /health
```

Returns HTTP 200 with:

```json
{ "status": "healthy" }
```

The health endpoint:
- Does not check Azure provider connectivity
- Does not require authentication
- Works in Mock and Azure provider modes
- Is safe for uptime monitoring, load balancer probes, and deployment verification

Use this endpoint to confirm the application process is running and the ASP.NET Core pipeline is functioning correctly.

**Example:**
```bash
curl https://yourdomain.com/health
# { "status": "healthy" }
```

If the application returns anything other than HTTP 200, the process has failed and should be restarted.

---

## Startup Validation

The application validates configuration on startup when `Translation:Provider = "Azure"`.

**What is validated:**

| Setting | Required |
|---|---|
| `AzureTranslator:Key` | Yes — non-empty |
| `AzureTranslator:Region` | Yes — non-empty |
| `AzureSpeech:Key` | Yes — non-empty |
| `AzureSpeech:Region` | Yes — non-empty |
| `AzureSpeech:Endpoint` | Yes — non-empty |

**On failure:**

The application throws `InvalidOperationException` at startup with a message identifying the missing key. It does not start and does not accept any requests. Example:

```
InvalidOperationException: Azure Speech is not configured.
Set AzureSpeech:Key, AzureSpeech:Region, and AzureSpeech:Endpoint via User Secrets or environment variables.
```

This is the correct behavior — fail fast before serving users with a broken configuration.

**Active provider log:**

On every startup, the application logs:

```
info: Provider mode: Azure
```
or
```
info: Provider mode: Mock
```

If this log does not appear, the application has not started.

---

## Switching Provider Mode

**To Mock mode (no Azure credentials):**

Set environment variable:
```
Translation__Provider = Mock
```

Or in `appsettings.json` (non-production):
```json
"Translation": { "Provider": "Mock" }
```

**To Azure mode:**

Set environment variable:
```
Translation__Provider = Azure
```

All five Azure credential environment variables must also be set.

---

## Troubleshooting

### Application fails to start

**Symptom:** Process exits immediately; health endpoint not reachable.

**Check:**
1. Is `ASPNETCORE_ENVIRONMENT` set? If set to `Production`, `appsettings.Production.json` activates `Translation:Provider = "Azure"`.
2. Are all 5 Azure environment variables set? Check the startup error message — it names the missing key.
3. Is the .NET 8 runtime installed on the host?

**Resolution:** Verify environment variables are set correctly on the hosting platform. Restart after correcting.

---

### Translation returns 502

**Symptom:** `POST /api/translate/text` or `POST /api/translate/audio` returns HTTP 502 with error code `PROVIDER_ERROR`.

**Meaning:** Azure Translator or Azure Speech returned an error or is unavailable.

**Check:**
1. Are Azure credentials valid and not expired?
2. Is the Azure resource in the correct region?
3. Has the quota been exhausted?
4. Is the Azure service experiencing an outage? Check Azure status page.

**Log to inspect:** The application logs `Provider exception. CorrelationId: ..., ErrorCode: PROVIDER_ERROR`. The correlation ID in the response and log allows matching the request.

---

### TTS returns 502

**Symptom:** `POST /api/translate/tts` returns HTTP 502.

**Meaning:** Azure Speech TTS failed.

**Check:**
1. Is `AzureSpeech:Key` and `AzureSpeech:Region` correct?
2. Is the requested language (`language` field) supported? Check if the Neural voice for that language is available in your Azure resource's region and SKU.

---

### CORS error in browser

**Symptom:** Browser console shows `Access-Control-Allow-Origin` error when frontend calls the backend.

**Meaning:** The frontend origin is not in `AllowedCorsOrigins`.

**Fix:** Add the frontend origin to the `AllowedCorsOrigins` configuration:

```
AllowedCorsOrigins__0 = https://your-frontend-domain.com
```

Restart the backend after changing. For same-origin deployments, CORS errors should not occur — if they do, verify the hosting proxy is routing correctly.

---

### Request size error (413)

**Symptom:** Audio upload returns HTTP 413.

**Meaning:** Audio file exceeds the 10 MB limit set in `Translation:MaxAudioBytes`.

**Resolution:** The limit is intentional. Request the user to use a shorter recording (max 60 seconds push-to-talk). The frontend enforces a 10 MB client-side guard before upload.

---

## Logging

### What is logged

| Event | Level | Content |
|---|---|---|
| Provider mode at startup | Information | Provider name (`Mock` or `Azure`) |
| Request received (each endpoint) | Information | Endpoint, correlation ID, language codes |
| Validation failure | Information | Correlation ID, error code |
| Request completed | Information | Correlation ID |
| Provider exception | Warning | Correlation ID, error code, exception type only |
| Unhandled exception | Error | Correlation ID, exception type only |

### What is never logged

- Source text (the text submitted for translation)
- Translated text
- Audio content
- Transcribed text
- Azure API keys or credentials
- Provider error messages or stack traces

This is enforced by design (D-009, D-024). See `docs/VALIDATION.md` security rules.

### Production log levels

In production (`appsettings.Production.json`):
- Framework logs (`Microsoft.*`) — Warning only
- Application logs (`MyTranslationApp.*`) — Information

This means standard request/response logs from `Microsoft.AspNetCore.Hosting` are suppressed. Only application-level events are emitted at Information.

---

## Correlation IDs

Every request receives a correlation ID injected by `CorrelationIdMiddleware`:

- If the request includes an `X-Correlation-ID` header, that value is reused.
- Otherwise, a new GUID is generated.

The correlation ID appears in:
- All application log entries for that request
- All API response bodies (`correlationId` field)
- The `X-Correlation-ID` response header (TTS endpoint)

When investigating a reported issue, ask the user for the `correlationId` from the error panel. Use it to find all related log entries.

---

## AWS-Specific Operations Notes

### Production Environment (Sprint 012)

| Resource | Value |
|---|---|
| EB Application | `my-translation-app` |
| EB Environment | `my-translation-api-prod` (e-wkmimrxppx) |
| EB URL | http://my-translation-api-prod.eba-pahyptkw.ap-southeast-2.elasticbeanstalk.com |
| CloudFront Distribution | `EOSQIHDJHIZ82` |
| CloudFront URL | https://d2ftspeokj49uq.cloudfront.net |
| S3 Bucket | `my-translation-app-frontend` |
| Region | ap-southeast-2 (Sydney) |

### Elastic Beanstalk Health Check

The EB environment is configured with health check path `/health` via the `Application Healthcheck URL` option setting. The `/health` endpoint returns HTTP 200 with `{ "status": "healthy" }` regardless of Azure provider status — this is intentional (D-078).

To verify EB is healthy:
```bash
curl http://my-translation-api-prod.eba-pahyptkw.ap-southeast-2.elasticbeanstalk.com/health
# Expected: { "status": "healthy" }
```

Or through CloudFront (recommended):
```bash
curl https://d2ftspeokj49uq.cloudfront.net/health
# Expected: { "status": "healthy" }
```

### CORS and CloudFront

The Sprint 012 deployment uses **same-origin proxy pattern** — CloudFront routes `/api/*` to EB. Browser API calls are same-origin (CloudFront domain) so CORS preflight does not occur.

`AllowedCorsOrigins__0` is set to `https://d2ftspeokj49uq.cloudfront.net` on EB as a belt-and-suspenders configuration.

If a CORS error appears in the browser (unexpected in same-origin pattern), check:
1. That the request URL uses the CloudFront domain, not the EB domain directly.
2. That the CloudFront `/api/*` behavior is routing to EB (check CloudFront distribution behaviors).

### Redeploying the Backend

```bash
# 1. Publish
dotnet publish src/backend/MyTranslationApp.Api/MyTranslationApp.Api.csproj \
  --configuration Release --runtime linux-x64 --self-contained false --output ./publish

# 2. Package (PowerShell)
Compress-Archive -Path ./publish/* -DestinationPath ./my-translation-api-v2.zip -Force

# 3. Upload to S3
aws s3 cp my-translation-api-v2.zip \
  s3://elasticbeanstalk-ap-southeast-2-185512089178/my-translation-app/my-translation-api-v2.zip

# 4. Create new EB application version
aws elasticbeanstalk create-application-version \
  --application-name my-translation-app \
  --version-label v2.0.0 \
  --source-bundle S3Bucket=elasticbeanstalk-ap-southeast-2-185512089178,S3Key=my-translation-app/my-translation-api-v2.zip \
  --region ap-southeast-2

# 5. Deploy to environment
aws elasticbeanstalk update-environment \
  --environment-name my-translation-api-prod \
  --version-label v2.0.0 \
  --region ap-southeast-2
```

### Redeploying the Frontend

```bash
# Build with same-origin config (VITE_API_BASE_URL empty = CloudFront proxy)
cd src/frontend
npm run build

# Sync to S3
aws s3 sync dist/ s3://my-translation-app-frontend --delete --region ap-southeast-2

# Invalidate CloudFront cache
aws cloudfront create-invalidation \
  --distribution-id EOSQIHDJHIZ82 \
  --paths "/*"
```

### Stopping the Environment (Cost Control)

```bash
aws elasticbeanstalk terminate-environment \
  --environment-name my-translation-api-prod \
  --region ap-southeast-2
```

S3 and CloudFront have negligible cost at rest. The EB EC2 instance (~$8–10/month) is the main ongoing charge.

### EB Startup Log

In EB console → **Logs** → **Request last 100 lines**, look for:

```
Provider mode: Azure
```

If this line is absent, the application has not started or started in Mock mode. Check that `Translation__Provider=Azure` and all Azure credential env vars are set.

---

## Deployment Verification Checklist

After each deployment:

- [ ] `GET /health` → HTTP 200, `{ "status": "healthy" }`
- [ ] `GET /api/languages` → 37 languages with capability flags
- [ ] `POST /api/translate/text` with English → Spanish → real translation, `"provider": "azure"`
- [ ] Startup log shows `Provider mode: Azure`
- [ ] No secrets appear in any log output
- [ ] Browser: frontend loads, language selectors populate, translate button works
