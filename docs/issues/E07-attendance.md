<!-- EPIC -->
title: [Epic] E07 - Attendance Management (إدارة الحضور والانصراف)
labels: type:epic, module:attendance, priority:high
milestone: M2 - Incentives & Attendance
---
## Goal
Record employees' **absence days** and apply configurable deduction/bonus percentages (ATT-01..ATT-05).

## Absence rules (manager can add/edit % at any time)
| Absence days in month | Effect |
|-----------------------|--------|
| 0 or 1 day | **Bonus** % |
| 2 days | none (see Analysis open question 3) |
| more than 2 days | Deduction % (tier 1) |
| more than 5 days | Deduction % (tier 2) |
| more than 10 days | Deduction % (tier 3) |
| more than 15 days | Deduction % (tier 4) |

Matching: highest deduction threshold strictly exceeded; bonus rule applies when days ≤ bonus max days (1).

<!-- TASK -->
title: [Backend] Absence rules (deduction & bonus tiers) API
labels: type:task, area:backend, module:attendance, priority:high
---
## Description
Store absence rules as configurable data.

## Technical Details
- Enum `AbsenceRuleType { Deduction = 1, Bonus = 2 }`.
- Entity `AbsenceRule : AuditableEntity { AbsenceRuleType Type, int Threshold, decimal Percentage, bool IsActive }`
  - Deduction: `Threshold` = days that must be **exceeded** (2, 5, 10, 15).
  - Bonus: `Threshold` = **maximum** days to qualify (1). Only one active bonus rule.
- Seed the 4 deduction tiers + 1 bonus rule.
- Domain: `AbsencePolicy.Evaluate(int absenceDays, rules)` → `{ type, percentage } | none`.
- Endpoints `/api/v1/attendance/rules`: `GET`, `POST`, `PUT /{id}`, `DELETE /{id}` (Admin for writes).
- Validators: unique threshold per type, percentage 0–100, threshold ≥ 0.

## Acceptance Criteria
- [ ] Unit tests: 0→bonus, 1→bonus, 2→none, 3→>2, 5→>2, 6→>5, 11→>10, 16→>15, 31→>15.
- [ ] Admin can add/edit/delete rules; HR read-only.

**Estimate:** 3 SP

<!-- TASK -->
title: [Backend] Monthly absence records API (bulk upsert)
labels: type:task, area:backend, module:attendance, priority:high
---
## Description
Record absence days per employee per month.

## Technical Details
- Entity `AbsenceRecord : AuditableEntity { int EmployeeId, int Year, int Month, int AbsenceDays, string? Notes }`; unique `(EmployeeId, Year, Month)`.
- `GET /api/v1/attendance/records?year=&month=&departmentId=` → all active employees for the month with their days (0 if no record), plus the evaluated rule (bonus/deduction %) for preview.
- `PUT /api/v1/attendance/records` `{ year, month, items: [{ employeeId, absenceDays, notes }] }` → bulk upsert in one transaction (HrOrAdmin).
- `GET /api/v1/attendance/records/employee/{employeeId}?year=` → history for an employee.
- Validators: 0 ≤ days ≤ days in month; employee hired on/before month end; month not locked by an **approved** payroll run (409).

## Acceptance Criteria
- [ ] Bulk save creates/updates records atomically.
- [ ] Editing a month with an approved payroll returns 409.
- [ ] Integration tests for get/save.

**Estimate:** 3 SP

<!-- TASK -->
title: [Frontend] Absence rules settings screen
labels: type:task, area:frontend, module:attendance, priority:medium
---
## Description
UI for the manager to set deduction and bonus percentages.

## Technical Details
- Route `/attendance/rules` (Admin edit, HR read-only).
- Bonus card: "0 – N days absence → bonus X%" (editable).
- Deduction table: "More than N days → X%" with add/edit dialog and delete confirm; sorted by threshold.

## Acceptance Criteria
- [ ] Admin can add/edit all percentages at any time; validation messages translated.

**Estimate:** 2 SP

<!-- TASK -->
title: [Frontend] Monthly attendance entry grid
labels: type:task, area:frontend, module:attendance, priority:high
---
## Description
Fast entry of absence days for all employees in a month.

## Technical Details
- Route `/attendance`: month/year picker + department filter.
- Editable grid (FormArray): employee code, name, department, absence days (number input), notes, computed effect chip (Bonus x% green / Deduction x% red / none) updated live from the rules.
- Save all (bulk PUT), dirty-state indicator, unsaved-changes guard; read-only with a banner if month is locked by an approved payroll.
- Employee details page tab: attendance history.

## Acceptance Criteria
- [ ] HR can enter/edit absence days for a whole department and save in one action.
- [ ] Effect chip reflects the configured rules.

**Estimate:** 5 SP
