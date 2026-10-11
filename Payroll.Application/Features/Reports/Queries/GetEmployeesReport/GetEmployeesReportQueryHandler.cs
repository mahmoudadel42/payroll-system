using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Localization;
using Payroll.Application.Features.Reports.Model;
using Payroll.Domain.Services;

namespace Payroll.Application.Features.Reports.Queries.GetEmployeesReport
{
    public sealed class GetEmployeesReportQueryHandler(IApplicationDbContext db, TimeProvider time)
        : IRequestHandler<GetEmployeesReportQuery, EmployeesReportDto>
    {
        public async Task<EmployeesReportDto> Handle(GetEmployeesReportQuery request, CancellationToken ct)
        {
            var asOf = request.AsOf ?? DateOnly.FromDateTime(time.GetLocalNow().DateTime);
            var arabic = CurrentCulture.IsArabic;

            var query = db.Employees.AsNoTracking();
            if (request.DepartmentId is { } departmentId)
                query = query.Where(e => e.DepartmentId == departmentId);
            if (request.Grade is { } grade)
                query = query.Where(e => e.Grade == grade);

            var employees = await query
                .OrderBy(e => e.FullName)
                .Select(e => new
                {
                    e.Id,
                    e.FullName,
                    e.Email,
                    e.Phone,
                    DepartmentName = arabic ? e.Department.NameAr : e.Department.NameEn,
                    e.Grade,
                    e.BirthDate,
                    e.HireDate
                })
                .ToListAsync(ct);

            var salaries = await db.GradeSalaries.AsNoTracking().ToDictionaryAsync(g => g.Grade, g => g.BaseSalary, ct);

            var rows = employees.Select(e => new EmployeesReportRow(
                e.Id, e.FullName, e.Email, e.Phone, e.DepartmentName, e.Grade, e.BirthDate, e.HireDate,
                PayrollCalculator.CompletedYears(e.HireDate, asOf),
                salaries.TryGetValue(e.Grade, out var s) ? s : null)).ToList();

            var byDepartment = rows.GroupBy(r => r.DepartmentName)
                .Select(g => new CountItem(g.Key, g.Count()))
                .OrderBy(c => c.Name)
                .ToList();
            var byGrade = rows.GroupBy(r => r.Grade)
                .OrderBy(g => g.Key)
                .Select(g => new CountItem(g.Key.ToString(), g.Count()))
                .ToList();

            return new EmployeesReportDto(asOf, rows, rows.Count, byDepartment, byGrade);
        }
    }
}
