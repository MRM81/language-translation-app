# Sprint 011 Acceptance — AWS Deployment Preparation

## Code Changes

- [x] `src/frontend/src/api/translationApi.ts` — `API_BASE` reads from `import.meta.env.VITE_API_BASE_URL ?? ''`
- [x] `src/frontend/.env.production.example` — documents `VITE_API_BASE_URL` for production builds
- [x] `.gitignore` — allows `.env.production.example` to be committed

## Documentation

- [x] `docs/AWS_DEPLOYMENT.md` — prerequisites, backend publish, EB setup, S3+CloudFront setup, CORS config, health check, environment variables, verification, rollback, cost notes
- [x] `docs/AWS_ARCHITECTURE.md` — architecture diagram, component responsibilities, cross-cloud rationale, environment variable reference, security boundaries, upgrade path
- [x] `docs/DEPLOYMENT.md` — AWS section and links added
- [x] `docs/ENVIRONMENTS.md` — EB environment variables section added
- [x] `docs/OPERATIONS.md` — EB health check config, CORS debugging, startup log location added

## Build and Tests

- [x] Backend build — 0 warnings, 0 errors
- [x] Backend tests — 133/133 pass
- [x] Frontend TypeScript — clean (`npx tsc --noEmit`)
- [x] Frontend build — clean (`npm run build`)

## Planning

- [x] `planning/STATE.md` updated
- [x] `planning/DECISIONS.md` — D-082 to D-085 added
- [x] `planning/RISKS.md` — R-048 to R-051 added
- [x] `planning/QUESTIONS.md` — Q-041 resolved; Q-045 to Q-047 added

## Security

- [x] No AWS credentials in source control
- [x] No Azure credentials in source control
- [x] `.env.production.example` contains only placeholder values

## Hosting Readiness

- [x] AWS S3 + CloudFront frontend hosting documented
- [x] AWS Elastic Beanstalk backend hosting documented
- [x] CORS CloudFront Origin-forwarding requirement documented (critical for cross-origin setup)
- [x] EB health check path `/health` documented
- [x] `ASPNETCORE_URLS=http://+:5000` (EB port) documented
- [x] Secrets management via EB env vars (MVP) and SSM (recommended) documented
- [x] CloudFront SPA routing (error pages → index.html) documented
- [x] Rollback steps documented for both frontend and backend

## Not Done (deferred)

- [ ] Actual AWS deployment (deferred to Sprint 012)
- [ ] CI/CD pipeline (Q-047 open)
- [ ] Custom domain / Route 53 (Q-046 open)
- [ ] Docker / ECS/Fargate (out of scope for Sprint 011)
