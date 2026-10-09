<!-- EPIC -->
title: [Epic] E05 - Employee Management (إدارة الموظفين)
labels: type:epic, module:employees, priority:high
milestone: M1 - Master Data
---
## Goal
**Add, Edit, View, Delete** employees (EMP-01..EMP-04).

Employee fields: **Name (Arabic required, English optional), Date of Birth, Address, Phone, Email, Job Grade, Department, Hire Date**.

## Business Rules
- Name, grade, department, hire date, email are required; email unique.
- Hire date not in the future; age at hire ≥ 18.
- Phone: valid format (e.g. Egyptian mobile `^01[0125][0-9]{8}$`, configurable).
- Delete = soft delete (keeps payroll history).

<!-- TASK -->
title: [Backend] Employee entity & CRUD API with search, filter and paging
labels: type:task, area:backend, module:employees, priority:high
---
## Description
Implement the Employee aggregate and endpoints.

## Technical Details
- Entity `Employee : AuditableEntity, ISoftDelete { EmployeeCode (auto, e.g. EMP-0001), LocalizedText FullName (FullNameAr, FullNameEn), SearchKey (normalised Arabic + English), DateOnly DateOfBirth, Address, Phone, Email, JobGrade Grade, int DepartmentId, Department, DateOnly HireDate, RowVersion }`.
- Domain method `GetYearsOfService(DateOnly asOf)` (full years) — unit tested incl. leap years.
- CQRS: Create / Update / Delete commands; `GetEmployeesQuery` (search by Arabic or English name via normalised `SearchKey`, code, email, phone; sorting by name uses the active language; filter `departmentId`, `grade`, sort, paging); `GetEmployeeByIdQuery` (includes department name, current basic salary, years of service).
- Endpoints: `GET /api/v1/employees`, `GET /{id}`, `POST`, `PUT /{id}`, `DELETE /{id}` (HrOrAdmin).
- FluentValidation rules as in the epic; department must exist; unique email (409/400).
- Indexes: `Email` (unique filtered), `DepartmentId`, `FullName`.

## Acceptance Criteria
- [ ] All CRUD operations work; deleted employees disappear from lists but remain in payroll history.
- [ ] Validation rules enforced with field-level errors.
- [ ] Unit + integration tests.

**Estimate:** 5 SP

<!-- TASK -->
title: [Frontend] Employees list with search, filters and paging
labels: type:task, area:frontend, module:employees, priority:high
---
## Description
Employee listing screen.

## Technical Details
- Route `/employees`: DataTable columns (code, name, department, grade, phone, email, hire date, actions).
- Toolbar: debounced search (300 ms), department dropdown (lookup API), grade dropdown, clear filters; filters synced with URL query params.
- Server-side paging/sorting; actions: view, edit, delete (confirm).
- Signal store `EmployeesStore` (`rxResource` with params signal).

## Acceptance Criteria
- [ ] Search/filter/paging work against the API and persist on refresh (URL).
- [ ] Delete removes the row after confirmation.

**Estimate:** 3 SP

<!-- TASK -->
title: [Frontend] Employee add/edit form and details page
labels: type:task, area:frontend, module:employees, priority:high
---
## Description
Forms for creating/updating an employee and a read-only details page.

## Technical Details
- Routes `/employees/new`, `/employees/:id/edit`, `/employees/:id`.
- Typed reactive form: fullNameAr (required, `dir="rtl"`), fullNameEn (optional, `dir="ltr"`), dateOfBirth, address, phone, email, grade (select), department (select), hireDate; client validators mirroring the server; server errors mapped to controls.
- Unsaved-changes guard (`canDeactivate`).
- Details page: personal info, job info (department, grade, current basic salary, years of service), and tabs placeholders for attendance history and payslips (filled in E07/E08).

## Acceptance Criteria
- [ ] Employee can be created and edited with full validation in AR/EN.
- [ ] Details page shows computed years of service and current basic salary.

**Estimate:** 3 SP
