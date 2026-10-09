<!-- EPIC -->
title: [Epic] E03 - Department Management (إدارة الأقسام)
labels: type:epic, module:departments, priority:high
milestone: M1 - Master Data
---
## Goal
Manage company departments: **Add, Edit, Delete, View** (requirements DEP-01..DEP-04).
Each department also holds its **incentive percentage** (managed in E06).

## Business Rules
- Department name in **Arabic is required**, English optional (falls back to Arabic); each unique (max 100 chars).
- A department with assigned (non-deleted) employees cannot be deleted.

<!-- TASK -->
title: [Backend] Department entity & CRUD API
labels: type:task, area:backend, module:departments, priority:high
---
## Description
Implement the Department aggregate and its REST endpoints.

## Technical Details
- Entity `Department : AuditableEntity, ISoftDelete { LocalizedText Name (NameAr, NameEn), LocalizedText? Description, IncentivePercentage (decimal 5,2, default 0), RowVersion, ICollection<Employee> Employees }`; unique indexes on `NameAr` and `NameEn` (filtered `IsDeleted = 0`).
- CQRS: `CreateDepartmentCommand`, `UpdateDepartmentCommand`, `DeleteDepartmentCommand`, `GetDepartmentsQuery` (paged/search/sort), `GetDepartmentByIdQuery`, `GetDepartmentLookupQuery` (id + name for dropdowns).
- `DepartmentDto { id, name (localized by Accept-Language), nameAr, nameEn, descriptionAr, descriptionEn, incentivePercentage, employeesCount, rowVersion }`; lookup returns localized name.
- Endpoints: `GET /api/v1/departments`, `GET /lookup`, `GET /{id}`, `POST` (Admin), `PUT /{id}` (Admin), `DELETE /{id}` (Admin).
- Validators: name required/unique/max length; delete returns 409 if employees exist.

## Acceptance Criteria
- [ ] CRUD works and is documented in OpenAPI.
- [ ] Duplicate name → 400; delete with employees → 409.
- [ ] Unit tests for handlers/validators; integration test for the endpoints.

**Estimate:** 3 SP

<!-- TASK -->
title: [Frontend] Departments list & add/edit form
labels: type:task, area:frontend, module:departments, priority:high
---
## Description
UI for department management.

## Technical Details
- Route `/departments`: DataTable (name, description, incentive %, employees count, actions) with search and server-side paging.
- Add/Edit in a Material dialog using `BilingualInputComponent` for name and description (Arabic required, English optional); server validation errors mapped to fields.
- Table shows the name in the active language.
- Delete with `ConfirmDialog`; show translated message on 409.
- `DepartmentsService` (HTTP) + signal store; Admin-only buttons via `*appHasRole`.

## Acceptance Criteria
- [ ] User can add, edit, delete, and view departments.
- [ ] Screens work in Arabic (RTL) and English.

**Estimate:** 3 SP
