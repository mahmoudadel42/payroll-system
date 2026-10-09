<!-- EPIC -->
title: [Epic] E04 - Salary Management (إدارة المرتبات)
labels: type:epic, module:salaries, priority:high
milestone: M1 - Master Data
---
## Goal
**Add and edit salaries** (SAL-01, SAL-02). Salaries differ by **job grade: First, Second, Third** (SAL-03).

## Business Rules
- One basic salary amount per grade, **effective-dated** (`EffectiveFrom`) so history is preserved.
- The salary used for a payroll month is the latest record with `EffectiveFrom <= last day of the month`.
- Amount > 0.

<!-- TASK -->
title: [Backend] Grade salary API (effective-dated)
labels: type:task, area:backend, module:salaries, priority:high
---
## Description
Store and manage basic salary per job grade.

## Technical Details
- Enum `JobGrade { First = 1, Second = 2, Third = 3 }` (Domain).
- Entity `GradeSalary : AuditableEntity { JobGrade Grade, decimal BasicSalary (18,2), DateOnly EffectiveFrom }`; unique index `(Grade, EffectiveFrom)`.
- Domain service / query `IGradeSalaryProvider.GetBasicSalary(grade, DateOnly asOf)`.
- Endpoints:
  - `GET /api/v1/grade-salaries` → current salary per grade `[{ grade, basicSalary, effectiveFrom }]`
  - `GET /api/v1/grade-salaries/history?grade=` → all records
  - `POST /api/v1/grade-salaries` `{ grade, basicSalary, effectiveFrom }` (Admin) — add
  - `PUT /api/v1/grade-salaries/{id}` (Admin) — edit; blocked (409) if used by an approved payroll run
- Seed initial salaries for the 3 grades.

## Acceptance Criteria
- [ ] Current salary per grade is returned correctly for any date (unit tests with multiple effective dates).
- [ ] Editing a salary used by an approved payroll returns 409.

**Estimate:** 3 SP

<!-- TASK -->
title: [Frontend] Grade salaries screen with history
labels: type:task, area:frontend, module:salaries, priority:high
---
## Description
UI to add/edit salaries per grade.

## Technical Details
- Route `/salaries`: 3 cards (First/Second/Third) showing current basic salary and effective date.
- "Add new salary" dialog (grade, amount, effective from — datepicker), "Edit" on history rows.
- Expandable history table per grade.
- Grade names translated (أولى / ثانية / ثالثة).

## Acceptance Criteria
- [ ] Admin can add and edit grade salaries; HR has read-only view.
- [ ] History shows all changes ordered by effective date.

**Estimate:** 2 SP
