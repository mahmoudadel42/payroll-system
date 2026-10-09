<!-- EPIC -->
title: [Epic] E01 - Project Setup & Infrastructure
labels: type:epic, area:devops, module:core, priority:high
milestone: M0 - Project Setup
---
## Goal
Create the foundations of the solution so feature work can start: repository conventions, .NET 10 backend skeleton (Clean Architecture), Angular app shell, database, Docker and CI.

## Outcome
- `docker compose up` starts SQL Server, the API and the Angular app.
- CI builds, lints and tests both backend and frontend on every PR.

## References
- docs/02-ARCHITECTURE.md
- docs/03-IMPLEMENTATION-PLAN.md

<!-- TASK -->
title: [Setup] Initialize repository, branching strategy & conventions
labels: type:task, area:devops, module:core, priority:high
---
## Description
Prepare the repository for team development.

## Technical Details
- Create `develop` branch; protect `main` and `develop` (require PR + passing CI).
- Add `.gitignore` (VisualStudio + Node + Angular), `.editorconfig`, `.gitattributes` (LF normalisation).
- Add `README.md` skeleton, `CONTRIBUTING.md` (branch naming `feature/<issue>-<slug>`, Conventional Commits).
- Add `.github/pull_request_template.md` (summary, linked issue, test evidence, checklist) and issue templates (bug, task).
- Folder layout: `backend/`, `frontend/`, `docs/`, `.github/`.

## Acceptance Criteria
- [ ] Branch protection active on `main` and `develop`.
- [ ] PR and issue templates appear when creating PRs/issues.
- [ ] Repository structure matches the architecture document.

**Estimate:** 2 SP

<!-- TASK -->
title: [Backend] Scaffold .NET 10 Clean Architecture solution
labels: type:task, area:backend, module:core, priority:high
---
## Description
Create the backend solution with Clean Architecture layers and test projects.

## Technical Details
- `dotnet new sln -n Payroll` under `backend/`.
- Projects (target `net10.0`, nullable + implicit usings enabled, `TreatWarningsAsErrors`):
  - `Payroll.Domain` (class library, no dependencies)
  - `Payroll.Application` (refs Domain; MediatR, FluentValidation, Mapster)
  - `Payroll.Infrastructure` (refs Application; EF Core SqlServer, Identity, QuestPDF, ClosedXML)
  - `Payroll.Api` (ASP.NET Core Web API, controllers; refs Application + Infrastructure)
  - Tests: `Payroll.Domain.Tests`, `Payroll.Application.Tests`, `Payroll.Api.IntegrationTests` (xUnit, FluentAssertions, NSubstitute)
- `Directory.Build.props` (common settings, analyzers) and `Directory.Packages.props` (central package management).
- `DependencyInjection.cs` extension in each layer: `AddApplication()`, `AddInfrastructure(config)`.
- Health check endpoint `GET /health`.

## Acceptance Criteria
- [ ] `dotnet build` and `dotnet test` succeed from `backend/`.
- [ ] Project references respect the dependency rule (Domain has no references).
- [ ] `GET /health` returns `200 Healthy`.

**Estimate:** 3 SP

<!-- TASK -->
title: [Backend] Configure EF Core, SQL Server & base entities (audit, soft delete)
labels: type:task, area:backend, module:core, priority:high
---
## Description
Set up persistence with EF Core 10 and common entity behaviour.

## Technical Details
- `BaseEntity { int Id }`, `AuditableEntity : BaseEntity { CreatedAt, CreatedBy, UpdatedAt, UpdatedBy }`, `ISoftDelete { IsDeleted, DeletedAt }`.
- `PayrollDbContext : IdentityDbContext<AppUser, AppRole, int>` implementing `IApplicationDbContext`.
- `SaveChangesInterceptor` filling audit fields from `ICurrentUserService`; soft delete converts `Deleted` state to `Modified` + `IsDeleted = true`.
- Global query filter for `ISoftDelete` entities.
- `IEntityTypeConfiguration<T>` per entity; all money columns `decimal(18,2)`, percentages `decimal(5,2)`.
- `rowversion` concurrency token on aggregate roots.
- Connection string from configuration / user-secrets; `Initial` migration; apply migrations + seed on startup in Development.

## Acceptance Criteria
- [ ] `dotnet ef database update` creates the database.
- [ ] Audit fields are filled automatically (covered by test).
- [ ] Soft-deleted rows are excluded from queries (covered by test).

**Estimate:** 3 SP

<!-- TASK -->
title: [Backend] Cross-cutting: ProblemDetails, validation pipeline, Serilog, OpenAPI
labels: type:task, area:backend, module:core, priority:high
---
## Description
Add shared infrastructure used by every feature.

## Technical Details
- Global exception handler (`IExceptionHandler`) → RFC 7807 `ProblemDetails`; map `ValidationException` → 400 (with `errors` dictionary), `NotFoundException` → 404, `ConflictException` → 409, `DbUpdateConcurrencyException` → 409.
- MediatR pipeline behaviours: `ValidationBehavior` (FluentValidation), `LoggingBehavior`, `PerformanceBehavior` (warn > 500 ms).
- Serilog: console + rolling file, request logging, correlation id header `X-Correlation-Id`.
- OpenAPI document (`AddOpenApi`) + Scalar UI at `/scalar` in Development; JWT security scheme.
- API versioning (`/api/v1`), CORS policy for the Angular origin, response compression.
- Shared `PagedResult<T>` and `PagedQuery` (page, pageSize ≤ 100, sort, search).

## Acceptance Criteria
- [ ] Invalid request returns 400 `ProblemDetails` with field errors.
- [ ] Unknown id returns 404 `ProblemDetails`.
- [ ] OpenAPI UI available in Development.

**Estimate:** 3 SP

<!-- TASK -->
title: [Backend] Localization infrastructure (Arabic / English)
labels: type:task, area:backend, module:core, priority:high, i18n
---
## Description
Make the API fully bilingual: messages in the requested language and support for bilingual data fields.

## Technical Details
- `AddLocalization()` + `UseRequestLocalization` with supported cultures `ar` (default) and `en`, provider `AcceptLanguageHeaderRequestCultureProvider` (+ `?culture=` query for downloads).
- `SharedResource.ar.resx` / `SharedResource.en.resx` for: validation messages, `ProblemDetails` titles/details, enum display names (`Enum.JobGrade.First` = أولى / First, etc.), report titles and column headers.
- FluentValidation: `ValidatorOptions.Global.LanguageManager` culture from request; custom messages through `IStringLocalizer<SharedResource>`.
- Owned value object `LocalizedText { string Ar; string? En }` + EF configuration (`NameAr`, `NameEn` columns) and `ToLocalized(culture)` extension (English falls back to Arabic).
- Arabic search normalisation helper `ArabicNormalizer.Normalize(string)` (remove tashkeel/tatweel, أ/إ/آ→ا, ة→ه, ى→ي) used to fill `SearchKey` columns.
- `AppUser.PreferredLanguage` (`ar`/`en`) + `PUT /api/v1/auth/me/language`.
- Unit test that both `.resx` files contain the same keys.

## Acceptance Criteria
- [ ] Same invalid request returns Arabic messages with `Accept-Language: ar` and English with `en`.
- [ ] Missing English value falls back to Arabic.
- [ ] Arabic search finds "احمد" when stored as "أحمد".

**Estimate:** 3 SP

<!-- TASK -->
title: [Frontend] Scaffold Angular app (standalone, Material, ESLint, Prettier)
labels: type:task, area:frontend, module:core, priority:high
---
## Description
Create the Angular workspace using the latest stable Angular CLI.

## Technical Details
- `npx @angular/cli@latest new payroll-web --style=scss --routing --ssr=false` under `frontend/`.
- Standalone components, zoneless change detection (`provideZonelessChangeDetection()`), strict TypeScript.
- `ng add @angular/material` with a custom theme (primary/accent), typography supporting Arabic (Cairo font).
- `ng add @angular-eslint/schematics`, Prettier + `eslint-config-prettier`, Husky + lint-staged pre-commit.
- Environment files (`apiBaseUrl`), proxy config `proxy.conf.json` to the API for local dev.
- Path aliases: `@core/*`, `@shared/*`, `@features/*`, `@env/*`.

## Acceptance Criteria
- [ ] `ng serve`, `ng build`, `ng lint`, `ng test` succeed.
- [ ] Material theme applied; Arabic font renders correctly.

**Estimate:** 2 SP

<!-- TASK -->
title: [Frontend] Core shell: layout, routing, i18n (Arabic RTL / English), HTTP interceptors
labels: type:task, area:frontend, module:core, priority:high, i18n
---
## Description
Build the application shell and shared plumbing used by all features.

## Technical Details
- `MainLayoutComponent`: Material sidenav (menu: Dashboard, Employees, Departments, Salaries, Incentives, Attendance, Payroll, Reports, Users) + toolbar (language switch, user menu, logout).
- Lazy-loaded feature routes (`loadChildren` / `loadComponent`), `NotFoundComponent`.
- i18n with `@ngx-translate/core` + HTTP loader (`assets/i18n/ar.json`, `en.json`); default Arabic; `LanguageService` (signal) persists choice in `localStorage` and user profile; sets `document.dir`, `lang`, CDK `Directionality`, `MAT_DATE_LOCALE` and `DateAdapter` locale; `registerLocaleData(localeAr)`.
- `acceptLanguageInterceptor` sends `Accept-Language` on every request.
- RTL rules: only CSS logical properties (`margin-inline-start`, etc.), mirror directional icons under `[dir=rtl]`, sidenav position switches to the right in Arabic.
- Shared `BilingualInputComponent` (Arabic field `dir="rtl"` + English field `dir="ltr"`) for `*Ar`/`*En` form fields.
- Script `npm run i18n:check` (run in CI) failing when `ar.json` and `en.json` keys differ.
- Interceptors (functional): `apiUrlInterceptor`, `authInterceptor` (Bearer), `errorInterceptor` (ProblemDetails → snackbar, field errors returned to forms), `loadingInterceptor` (global progress bar).
- Shared components: `PageHeaderComponent`, `DataTableComponent<T>` (MatTable + sort + paginator, server-side paging), `ConfirmDialogComponent`, `EmptyStateComponent`.
- Shared pipes: `currencyEg`/money pipe, translated enum pipe (`grade`).

## Acceptance Criteria
- [ ] Switching language flips layout direction, texts, dates and numbers without reload.
- [ ] `i18n:check` passes; no hard-coded UI text.
- [ ] Unknown route shows 404 page.
- [ ] API errors show a translated snackbar message.

**Estimate:** 5 SP

<!-- TASK -->
title: [DevOps] Docker & docker-compose for local development
labels: type:task, area:devops, module:core, priority:medium
---
## Description
Containerise the full stack for local development and later deployment.

## Technical Details
- `backend/Dockerfile`: multi-stage (`mcr.microsoft.com/dotnet/sdk:10.0` → `aspnet:10.0`), non-root user.
- `frontend/Dockerfile`: multi-stage (Node LTS build → `nginx:alpine`), `nginx.conf` with SPA fallback and `/api` reverse proxy.
- `docker-compose.yml`: `sqlserver` (mcr.microsoft.com/mssql/server:2022-latest, volume, healthcheck), `api` (depends on healthy DB), `web`.
- `.env.example` for passwords/ports; `.dockerignore` files.

## Acceptance Criteria
- [ ] `docker compose up --build` serves the app at `http://localhost:4200` and API at `http://localhost:5000`.
- [ ] Database data persists across restarts.

**Estimate:** 3 SP

<!-- TASK -->
title: [DevOps] GitHub Actions CI pipeline (build, lint, test)
labels: type:task, area:devops, module:core, priority:high
---
## Description
Automate quality checks on every push and pull request.

## Technical Details
- `.github/workflows/ci.yml` with path filters and two jobs:
  - **backend**: setup-dotnet 10.x, restore (cache NuGet), `dotnet format --verify-no-changes`, build Release, test with coverage (Coverlet) → upload artifact / summary.
  - **frontend**: setup-node LTS (cache npm), `npm ci`, `ng lint`, `ng test --watch=false`, `ng build --configuration production`.
- Required status checks on `develop` and `main`.
- Dependabot config for NuGet, npm, GitHub Actions.

## Acceptance Criteria
- [ ] CI runs on PRs and blocks merge on failure.
- [ ] Coverage report visible in the workflow summary.

**Estimate:** 2 SP
