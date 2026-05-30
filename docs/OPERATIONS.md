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

### Elastic Beanstalk Health Check

EB's default health check targets `GET /` which returns 404. Configure the health check path to `/health` in the EB environment:

**AWS Console:** EB Environment → Configuration → Load balancer → Processes → Edit default process → **Health check path: `/health`**

The `/health` endpoint returns HTTP 200 with `{ "status": "healthy" }` regardless of Azure provider status. This is intentional — a connectivity check would require Azure credentials in the probe path.

### CORS and CloudFront

If the browser shows `Access-Control-Allow-Origin` errors when the frontend is on CloudFront:

1. Verify `AllowedCorsOrigins__0` on EB matches the exact CloudFront domain (including `https://`).
2. Verify the CloudFront **Origin request policy** is configured to forward the `Origin` header to the backend. Without this, `OPTIONS` preflight requests fail silently.
3. Restart the EB environment after changing `AllowedCorsOrigins`.

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
