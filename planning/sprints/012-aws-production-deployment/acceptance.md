# Sprint 012 — Acceptance Criteria

**Sprint:** 012 — AWS Production Deployment
**Status:** Complete — All criteria met
**Completed:** 2026-05-31

---

## Acceptance Criteria

| Criterion | Status | Evidence |
|---|---|---|
| Backend deployed | Done | EB environment `my-translation-api-prod` — Status: Ready, Health: Green |
| Frontend deployed | Done | S3 bucket `my-translation-app-frontend` — 3 files uploaded |
| CloudFront configured | Done | Distribution `EOSQIHDJHIZ82` — `d2ftspeokj49uq.cloudfront.net` |
| Health endpoint verified | Done | `{"status":"healthy"}` via CloudFront |
| Translation verified | Done | en→es ("Buenos días"), en→zh-Hans ("谢谢你"), provider: azure |
| STT verified | Done | "Good morning." transcribed → "Buenos días." translated |
| TTS verified | Done | 20,160 bytes audio/mpeg returned |
| 37-language catalog verified | Done | `GET /api/languages` → 37 entries |
| Production report created | Done | `docs/PRODUCTION_DEPLOYMENT_REPORT.md` |
| No secrets committed | Done | All Azure keys in EB env vars only; placeholders in source |
| Planning files updated | Done | STATE.md, DECISIONS.md, RISKS.md, QUESTIONS.md, FILE_INVENTORY.md |

---

## Production Validation Results

Tested through CloudFront HTTPS (`https://d2ftspeokj49uq.cloudfront.net`):

```
GET  /                          → HTTP 200  text/html (React app)
GET  /health                    → {"status":"healthy"}
GET  /api/languages             → 37 languages
POST /api/translate/text        → "Buenos días" (en→es, provider: azure)
POST /api/translate/text        → "谢谢你" (en→zh-Hans, provider: azure)
POST /api/translate/tts         → 20,160 bytes audio/mpeg
POST /api/translate/audio (STT) → "Good morning." → "Buenos días." (provider: azure)
```

---

## Known Deviations from Sprint 011 Plan

| Deviation | Reason | Impact |
|---|---|---|
| Same-origin pattern used instead of separate-origin | EB SingleInstance is HTTP-only; separate-origin would cause mixed-content browser blocks | Positive — simpler CORS, no browser preflight issues |
| IAM role created manually | Not documented as a prerequisite; fresh account had no EB IAM role | One-time setup; documented for future deployments |
| `VITE_API_BASE_URL=""` instead of EB URL | Same-origin proxy makes the URL relative (CloudFront routes /api/*) | Positive — frontend doesn't need to change when EB URL changes |
