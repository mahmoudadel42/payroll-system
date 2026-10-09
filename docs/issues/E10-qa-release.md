<!-- EPIC -->
title: [Epic] E10 - QA, Hardening & Release
labels: type:epic, area:qa, module:core, priority:medium
milestone: M5 - QA & Release
---
## Goal
Ensure quality, security and deployability before the first release (v1.0.0).

<!-- TASK -->
title: [QA] API integration tests with Testcontainers (SQL Server)
labels: type:task, area:qa, area:backend, module:core, priority:high
---
## Description
End-to-end API tests against a real SQL Server.

## Technical Details
- `Payroll.Api.IntegrationTests` with `WebApplicationFactory<Program>` + Testcontainers MsSql; Respawn to reset DB between tests.
- Test auth helper to obtain Admin/HR tokens.
- Cover every controller: happy path, validation (400), auth (401/403), not found (404), conflict (409).
- Scenario test: create departments/employees → set rates → enter attendance → generate & approve payroll → verify reports totals.
- Run in CI (Docker available on ubuntu runners).

## Acceptance Criteria
- [ ] All endpoints covered; scenario test green in CI.

**Estimate:** 5 SP

<!-- TASK -->
title: [QA] Frontend unit tests & Playwright E2E tests
labels: type:task, area:qa, area:frontend, module:core, priority:medium
---
## Description
Automated UI testing.

## Technical Details
- Unit tests for services, guards, interceptors, stores and key components (≥ 70% coverage).
- Playwright E2E (against docker-compose stack): login, employee CRUD, attendance entry, payroll generate/approve, report export; run in Arabic and English; screenshot on failure.
- CI job `e2e` on PRs to `main`.

## Acceptance Criteria
- [ ] E2E suite green for both languages.
- [ ] Visual check of every screen in RTL (Arabic) and LTR (English): no misaligned or overflowing elements.

**Estimate:** 5 SP

<!-- TASK -->
title: [Security] Security hardening & performance review
labels: type:task, area:backend, area:frontend, module:core, priority:medium
---
## Description
Review and harden the application.

## Technical Details
- OWASP checklist: security headers (CSP, HSTS, X-Content-Type-Options), rate limiting on `/auth/login` (`AddRateLimiter`), no secrets in repo, CORS whitelist, input length limits.
- Dependency scanning (`dotnet list package --vulnerable`, `npm audit`), GitHub CodeQL workflow.
- Performance: EF query review (`AsNoTracking`, projections, indexes), seed 5,000 employees and measure list/payroll/report endpoints.
- Angular: production build budgets, lazy loading verified, `OnPush`/signals everywhere.

## Acceptance Criteria
- [ ] No high/critical vulnerabilities; CodeQL clean.
- [ ] Payroll generation for 5,000 employees < 10 s.

**Estimate:** 3 SP

<!-- TASK -->
title: [Docs] README, user guide and API documentation
labels: type:task, area:docs, module:core, priority:medium
---
## Description
Project and user documentation.

## Technical Details
- `README.md`: overview, architecture diagram, prerequisites (.NET 10 SDK, Node LTS, Docker), run locally, run tests, environment variables.
- `docs/USER-GUIDE.md` (Arabic + English): each module with screenshots, payroll calculation explanation.
- OpenAPI exported to `docs/openapi.json`; Postman/Bruno collection.
- `CHANGELOG.md`.

## Acceptance Criteria
- [ ] A new developer can run the project using only the README.

**Estimate:** 2 SP

<!-- TASK -->
title: [DevOps] Release pipeline: Docker images to GHCR & deployment
labels: type:task, area:devops, module:core, priority:medium
---
## Description
Automate building and publishing releases.

## Technical Details
- `.github/workflows/release.yml` on tag `v*`: build & push `payroll-api` and `payroll-web` images to `ghcr.io/mahmoudadel42/*`, create GitHub Release with changelog.
- Production config: `ASPNETCORE_ENVIRONMENT=Production`, migrations applied via a migration bundle (`dotnet ef migrations bundle`) step.
- Deployment target (Azure App Service for Containers or a VPS with docker compose) documented; health check after deploy.

## Acceptance Criteria
- [ ] Tagging `v1.0.0` publishes images and a GitHub Release.

**Estimate:** 3 SP
