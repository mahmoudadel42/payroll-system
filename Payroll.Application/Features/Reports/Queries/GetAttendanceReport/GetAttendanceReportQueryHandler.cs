using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Localization;
using Payroll.Application.Features.Reports.Model;
using Payroll.Domain.Services;

namespace Payroll.Application.Features.Reports.Queries.GetAttendanceReport
{
    public sealed class GetAttendanceReportQueryHandler(IApplicationDbContext db)
        : IRequestHandler<GetAttendanceReportQuery, AttendanceReportDto>
    {
        public async Task<AttendanceReportDto> Handle(GetAttendanceReportQuery request, CancellationToken ct)
        {
            var periodStart = new DateOnly(request.Year, request.Month, 1);
            var periodEnd = periodStart.AddMonths(1).AddDays(-1);
            var arabic = CurrentCulture.IsArabic;

            var employeesQuery = db.Employees.AsNoTracking().Where(e => e.HireDate <= periodEnd);
            if (request.DepartmentId is { } departmentId)
                employeesQuery = employeesQuery.Where(e => e.DepartmentId == departmentId);
            if (request.EmployeeId is { } employeeId)
                employeesQuery = employeesQuery.Where(e => e.Id == employeeId);

            var employees = await employeesQuery
                .OrderBy(e => e.FullName)
                .Select(e => new
                {
                    e.Id,
                    e.FullName,
                    e.HireDate,
                    DepartmentName = arabic ? e.Department.NameAr : e.Department.NameEn
                })
                .ToListAsync(ct);

            var employeeIds = employees.Select(e => e.Id).ToList();
            var absences = await db.AbsenceRecords.AsNoTracking()
                .Where(a => a.Date >= periodStart && a.Date <= periodEnd && employeeIds.Contains(a.EmployeeId))
                .Select(a => new { a.EmployeeId, a.Date })
                .ToListAsync(ct);
            var datesByEmployee = absences
                .GroupBy(a => a.EmployeeId)
                .ToDictionary(g => g.Key, g => g.Select(a => a.Date).OrderBy(d => d).ToList());

            var absenceRules = await db.AbsenceRules.AsNoTracking().ToListAsync(ct);

            var rows = employees.Select(e =>
            {
                var dates = datesByEmployee.GetValueOrDefault(e.Id) ?? [];
                var breakdown = PayrollCalculator.Calculate(new PayslipInput(
                    0, 0, e.HireDate, dates.Count, periodEnd, [], absenceRules));

                var (effect, percentage) = breakdown.DeductionPercentage > 0
                    ? (AbsenceEffect.Deduction, breakdown.DeductionPercentage)
                    : breakdown.BonusPercentage > 0
                        ? (AbsenceEffect.Bonus, breakdown.BonusPercentage)
                        : (AbsenceEffect.None, 0m);

                return new AttendanceReportRow(
                    e.Id, e.FullName, e.DepartmentName, dates.Count,
                    string.Join(", ", dates.Select(d => d.ToString("yyyy-MM-dd"))),
                    effect, percentage);
            }).ToList();

            return new AttendanceReportDto(
                request.Year,
                request.Month,
                rows,
                rows.Sum(r => r.AbsenceDays),
                rows.Count(r => r.Effect == AbsenceEffect.Bonus),
                rows.Count(r => r.Effect == AbsenceEffect.Deduction));
        }
    }
}
