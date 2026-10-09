<!-- EPIC -->
title: [Epic] E02 - Authentication & Authorization
labels: type:epic, module:auth, priority:high
milestone: M0 - Project Setup
---
## Goal
Secure the system with login, JWT tokens and two roles:
- **Admin** (manager): full access, manages rates/percentages, approves payroll, manages users.
- **HR**: manages employees and attendance, views reports, generates draft payroll.

## Outcome
Only authenticated users can access the API/UI; screens and endpoints respect roles.

<!-- TASK -->
title: [Backend] Identity, JWT login/refresh, roles & seeding
labels: type:task, area:backend, module:auth, priority:high
---
## Description
Implement authentication with ASP.NET Core Identity and JWT bearer tokens.

## Technical Details
- `AppUser : IdentityUser<int>` (+ `FullName`, `IsActive`), `AppRole : IdentityRole<int>`; `RefreshToken` entity (token hash, expires, revoked, replacedBy).
- Endpoints (`AuthController`):
  - `POST /api/v1/auth/login` `{ userName, password }` → `{ accessToken, expiresAt, refreshToken, user: { id, fullName, roles } }`
  - `POST /api/v1/auth/refresh` `{ refreshToken }` → new pair (rotation; reuse detection revokes chain)
  - `POST /api/v1/auth/logout` → revoke refresh token
  - `GET /api/v1/auth/me`
- JWT: HMAC-SHA256, 15 min access token, 7 day refresh token; settings in `JwtOptions` (validated on startup).
- Authorization policies: `AdminOnly`, `HrOrAdmin`; default fallback policy = authenticated.
- Lockout: 5 failed attempts → 15 minutes. Password policy: min 8, upper, lower, digit.
- Seed roles `Admin`, `HR` and default admin user from configuration.
- `ICurrentUserService` reading claims (used by audit interceptor).

## Acceptance Criteria
- [ ] Valid credentials return tokens; invalid return 401 `ProblemDetails`.
- [ ] Refresh rotates tokens; reused refresh token is rejected.
- [ ] Protected endpoint returns 401 without token and 403 with insufficient role.
- [ ] Integration tests cover login, refresh, logout.

**Estimate:** 5 SP

<!-- TASK -->
title: [Backend] User management API (Admin only)
labels: type:task, area:backend, module:auth, priority:medium
---
## Description
Allow the Admin to manage system users.

## Technical Details
- `GET /api/v1/users` (paged, search), `GET /{id}`, `POST` (fullName, userName, email, password, role), `PUT /{id}` (fullName, email, role, isActive), `POST /{id}/reset-password`, `DELETE /{id}` (deactivate).
- Policy `AdminOnly`; prevent deactivating/deleting the last active Admin or yourself.
- Validators: unique userName/email, valid role.

## Acceptance Criteria
- [ ] Admin can create, edit, deactivate users and reset passwords.
- [ ] HR user receives 403 on all user endpoints.
- [ ] Last Admin cannot be removed (409).

**Estimate:** 3 SP

<!-- TASK -->
title: [Frontend] Login page, auth service, route guards
labels: type:task, area:frontend, module:auth, priority:high
---
## Description
Implement the login experience and client-side auth handling.

## Technical Details
- `LoginComponent` (standalone, reactive form, show/hide password, error message, language switch).
- `AuthService` with signals: `currentUser`, `isAuthenticated`, `roles`; store tokens (access token in memory, refresh token in `localStorage`); auto-refresh on 401 in `authInterceptor` with request queueing.
- `authGuard` (redirect to `/login?returnUrl=`), `roleGuard(['Admin'])`, `*appHasRole` structural directive to hide menu items/buttons.
- Logout clears state and calls API.

## Acceptance Criteria
- [ ] Unauthenticated user is redirected to login and returned to the original page after login.
- [ ] HR user does not see Admin-only menu items and cannot open them via URL.
- [ ] Expired access token is refreshed transparently.

**Estimate:** 3 SP

<!-- TASK -->
title: [Frontend] User management screens
labels: type:task, area:frontend, module:auth, priority:low
---
## Description
Admin UI for managing users.

## Technical Details
- `/users` list (DataTable: name, username, email, role, status, actions) with search.
- Create/Edit dialog (role select, active toggle), Reset password dialog, Deactivate confirm.
- Route protected by `roleGuard(['Admin'])`.

## Acceptance Criteria
- [ ] All user API operations are available from the UI with validation messages in AR/EN.

**Estimate:** 2 SP
