<!-- EPIC -->
title: [Epic] E09 - Reports & Dashboard (إدارة التقارير)
labels: type:epic, module:reports, priority:high
milestone: M4 - Reports & Dashboard
---
## Goal
Provide the four required reports (REP-01..REP-04) with on-screen view, filtering, and export to **PDF** and **Excel**, plus a summary dashboard.

1. Attendance report — تقرير بالحضور والانصراف
2. Incentives & deductions report — تقرير بالحوافز والخصومات
3. Company employees report — تقرير بالعاملين بالشركة
4. Employees' salaries report — تقرير مرتبات العاملين

## Common
- Filters: year, month, department (optional), grade (where relevant).
- `format=json|pdf|xlsx` query parameter; **every report available in Arabic and English** (language from `Accept-Language` / `?culture=`): translated titles, headers, enum values, localized names, locale number/date formats; RTL layout + Arabic-capable font for Arabic; company header, generated date/user, page numbers, totals row.

<!-- TASK -->
title: [Backend] Reporting infrastructure (PDF with QuestPDF, Excel with ClosedXML)
labels: type:task, area:backend, module:reports, priority:high
---
## Description
Reusable export engine for all reports.

## Technical Details
- `IReportExporter` with `PdfReportExporter` (QuestPDF, Community license, embedded Cairo/Noto Naskh Arabic font, RTL when `lang=ar`) and `ExcelReportExporter` (ClosedXML; header styling, number formats, auto-fit, RTL sheet when Arabic).
- Generic `ReportDefinition<T> { Title, Columns (header key, selector, format), Rows, Totals, Filters }` so each report only defines data + columns.
- Controller helper returning `FileContentResult` with proper content type and file name `{report}-{yyyy-MM}.{ext}`.
- Localised column headers via `IStringLocalizer` (`Accept-Language`).

## Acceptance Criteria
- [ ] Sample report renders correctly in Arabic and English for PDF and Excel (snapshot/smoke tests).

**Estimate:** 5 SP

<!-- TASK -->
title: [Backend] Attendance report endpoint
labels: type:task, area:backend, module:reports, priority:high
---
## Description
Absence days per employee for a month.

## Technical Details
- `GET /api/v1/reports/attendance?year=&month=&departmentId=&format=`
- Columns: code, name, department, absence days, effect (bonus/deduction/none), %.
- Summary: total employees, total absence days, employees with bonus, employees with deductions per tier.

## Acceptance Criteria
- [ ] JSON, PDF and Excel outputs contain identical data.

**Estimate:** 2 SP

<!-- TASK -->
title: [Backend] Incentives & deductions report endpoint
labels: type:task, area:backend, module:reports, priority:high
---
## Description
Breakdown of incentives and deductions per employee for a payroll month.

## Technical Details
- `GET /api/v1/reports/incentives-deductions?year=&month=&departmentId=&format=` — source: payslips of the month's run (Approved preferred; Draft flagged "DRAFT" watermark).
- Columns: code, name, department, dept incentive % / amount, service incentive % / amount, attendance bonus, absence deduction, net effect.
- Totals per column and per department subtotal.

## Acceptance Criteria
- [ ] Totals match payroll run totals.
- [ ] 404 ProblemDetails if no payroll run exists for the month.

**Estimate:** 2 SP

<!-- TASK -->
title: [Backend] Company employees report endpoint
labels: type:task, area:backend, module:reports, priority:medium
---
## Description
List of company employees.

## Technical Details
- `GET /api/v1/reports/employees?departmentId=&grade=&format=`
- Columns: code, name, department, grade, hire date, years of service, phone, email, address.
- Summary: count per department and per grade.

## Acceptance Criteria
- [ ] Filters applied; deleted employees excluded.

**Estimate:** 2 SP

<!-- TASK -->
title: [Backend] Employees salaries report endpoint
labels: type:task, area:backend, module:reports, priority:high
---
## Description
Monthly salaries per employee.

## Technical Details
- `GET /api/v1/reports/salaries?year=&month=&departmentId=&grade=&format=` — source: payslips.
- Columns: code, name, department, grade, basic, total incentives, bonus, deductions, net.
- Totals and department subtotals.

## Acceptance Criteria
- [ ] Net total equals payroll run `TotalNet`.

**Estimate:** 2 SP

<!-- TASK -->
title: [Frontend] Reports module (filters, preview, export, print)
labels: type:task, area:frontend, module:reports, priority:high
---
## Description
One reports area with a consistent UX for all four reports.

## Technical Details
- `/reports` landing with 4 report cards; `/reports/:type` page.
- Filter panel (month/year, department, grade as applicable) → preview table (MatTable with totals footer).
- Buttons: Export PDF, Export Excel (download blob via `HttpClient` `responseType: 'blob'`, file name from `Content-Disposition`), Print.
- Report language follows the UI language (`Accept-Language`).

## Acceptance Criteria
- [ ] All four reports can be previewed and exported in both formats and both languages.

**Estimate:** 5 SP

<!-- TASK -->
title: [Full-stack] Dashboard summary
labels: type:task, area:backend, area:frontend, module:reports, priority:low
---
## Description
Home dashboard with key figures.

## Technical Details
- `GET /api/v1/dashboard/summary` → employees count, departments count, last payroll total net & status, current month absence stats, employees per department, net salary trend (last 6 runs).
- Angular `/dashboard`: KPI cards + charts (ng2-charts: bar per department, line trend).

## Acceptance Criteria
- [ ] Dashboard loads in < 1 s with seeded data and reflects latest payroll.

**Estimate:** 3 SP
