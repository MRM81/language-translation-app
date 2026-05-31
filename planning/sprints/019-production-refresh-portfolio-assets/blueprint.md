# Sprint 019 — Blueprint

**Sprint:** 019 — Production Refresh & Portfolio Assets
**Status:** Completed
**Completed:** 2026-05-31

---

## Deployment

Frontend only — no backend changes since Sprint 012.

User executes:
```
cd src/frontend
npm run build

aws s3 sync dist/ s3://my-translation-app-frontend --delete
aws cloudfront create-invalidation --distribution-id EOSQIHDJHIZ82 --paths "/*"
```

---

## Portfolio Updates

| File | Action |
|---|---|
| portfolio/case-study.md | Updated — Sprints 009–019, full v1 feature set |
| portfolio/architecture-overview.md | Updated — AWS diagram, Conversation Mode, storage schema |
| portfolio/demo-script.md | Updated — 7–8 min, all features |
| portfolio/lessons-learned.md | Updated — Sprints 009–018 |
| portfolio/release-notes-v1.md | Created |

---

## Screenshots

Captured from local v1.0 build (localhost:5175) using Playwright.

Script: `design/screenshots/take-v1.mjs`

8 screenshots in `design/screenshots/v1/`:
- landing-page.png
- translation-mode.png
- audio-translation.png
- conversation-mode.png
- push-to-talk.png
- conversation-manager.png
- conversation-search.png
- mobile-view.png
