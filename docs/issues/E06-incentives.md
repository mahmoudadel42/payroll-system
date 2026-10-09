<!-- EPIC -->
title: [Epic] E06 - Incentive Management (إدارة الحوافز)
labels: type:epic, module:incentives, priority:high
milestone: M2 - Incentives & Attendance
---
## Goal
Manage the two incentive types (INC-01..INC-03):
1. **Department incentive** — every department has an incentive % applied to the salary (add/edit %).
2. **Years-of-service incentive** — employees who exceeded **5 years** get a %, and those who exceeded **7 years** get a % (add/edit %).

## Business Rules
- Percentages 0–100, two decimals.
- Service tiers are data (`MinYearsExceeded`, `Percentage`), seeded with 5 and 7; Admin may add more tiers.
- An employee matches the tier with the **highest `MinYearsExceeded` strictly less than** their full years of service; only that tier applies (not cumulative — see Analysis open question 2).
- Changes affect only draft/future payroll runs.

<!-- TASK -->
title: [Backend] Department incentive percentage endpoint
labels: type:task, area:backend, module:incentives, priority:high
---
## Description
Allow the Admin to set/edit each department's incentive %.

## Technical Details
- `PUT /api/v1/departments/{id}/incentive` `{ incentivePercentage }` (AdminOnly) → `UpdateDepartmentIncentiveCommand`.
- `GET /api/v1/incentives/departments` → `[{ departmentId, departmentName, incentivePercentage, employeesCount }]`.
- Validator: 0 ≤ value ≤ 100.
- Record change in an `IncentiveChangeLog` (old/new value, user, date) for auditing.

## Acceptance Criteria
- [ ] Admin can update the percentage; HR gets 403.
- [ ] Out-of-range values return 400.

**Estimate:** 2 SP

<!-- TASK -->
title: [Backend] Years-of-service incentive tiers API
labels: type:task, area:backend, module:incentives, priority:high
---
## Description
CRUD for service incentive tiers and the tier-matching logic.

## Technical Details
- Entity `ServiceIncentiveTier : AuditableEntity { int MinYearsExceeded, decimal Percentage, bool IsActive }`; unique `MinYearsExceeded`.
- Seed: `{5, x%}`, `{7, y%}` (values from configuration/stakeholder).
- Domain: `ServiceIncentivePolicy.Match(int yearsOfService, IEnumerable<ServiceIncentiveTier>)` → tier or null.
- Endpoints (`/api/v1/incentives/service-tiers`): `GET`, `POST` (Admin), `PUT /{id}` (Admin), `DELETE /{id}` (Admin).
- Validators: years ≥ 0, unique; percentage 0–100; warn (not block) if a higher years tier has a lower %.

## Acceptance Criteria
- [ ] Unit tests: 4 years → none; 5 years → none (must *exceed* 5); 6 years → 5-year tier; 8 years → 7-year tier.
- [ ] CRUD endpoints work and are documented.

**Estimate:** 3 SP

<!-- TASK -->
title: [Frontend] Incentive settings screen (department % and service tiers)
labels: type:task, area:frontend, module:incentives, priority:high
---
## Description
One screen with two tabs to manage incentive rates.

## Technical Details
- Route `/incentives` (Admin edit, HR read-only).
- Tab 1 "Department incentives": table of departments with inline-editable % (validation 0–100), save per row.
- Tab 2 "Years of service": table of tiers ("More than N years" → %), add/edit dialog, delete confirm.
- Info banner explaining that changes affect future/draft payrolls only.

## Acceptance Criteria
- [ ] Admin can add/edit department incentive % and service tiers.
- [ ] Values displayed with % formatting in AR/EN.

**Estimate:** 3 SP
