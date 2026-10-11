using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;
using Payroll.Domain.Entities;
using Payroll.Domain.Services;

namespace Payroll.Application.Features.PayrollRuns.Services
{
    public sealed class PayrollDataService(IApplicationDbContext db) : IPayrollDataService
    {
        public async Task<Result<IReadOnlyList<Payslip>>> BuildPayslipsAsync(
            int year, int month, int? departmentId, int? employeeId, CancellationToken ct)
        {
            var periodStart = new DateOnly(year, month, 1);
            var periodEnd = periodStart.AddMonths(1).AddDays(-1);

            var employeesQuery = db.Employees.AsNoTracking().Where(e => e.HireDate <= periodEnd);
            if (departmentId is { } deptId)
                employeesQuery = employeesQuery.Where(e => e.DepartmentId == deptId);
            if (employeeId is { } empId)
                employeesQuery = employeesQuery.Where(e => e.Id == empId);

            var employees = await employeesQuery
                .Select(e => new
                {
                    e.Id,
                    e.FullName,
                    e.Grade,
                    e.HireDate,
                    e.DepartmentId,
                    e.Department.NameAr,
                    e.Department.NameEn,
                    e.Department.IncentivePercentage
                })
                .OrderBy(e => e.FullName)
                .ToListAsync(ct);

            var gradeSalaries = await db.GradeSalaries.AsNoTracking().ToDictionaryAsync(g => g.Grade, g => g.BaseSalary, ct);
            var serviceRules = await db.ServiceYearsIncentiveRules.AsNoTracking().ToListAsync(ct);
            var absenceRules = await db.AbsenceRules.AsNoTracking().ToListAsync(ct);

            var employeeIds = employees.Select(e => e.Id).ToList();
            var absenceCounts = await db.AbsenceRecords.AsNoTracking()
                .Where(a => a.Date >= periodStart && a.Date <= periodEnd && employeeIds.Contains(a.EmployeeId))
                .GroupBy(a => a.EmployeeId)
                .Select(g => new { EmployeeId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.EmployeeId, x => x.Count, ct);

            var payslips = new List<Payslip>(employees.Count);
            foreach (var e in employees)
            {
                if (!gradeSalaries.TryGetValue(e.Grade, out var baseSalary))
                    return Error.Rule("payroll.missingGradeSalary", "Define salaries for all job grades before running payroll.");

                var absenceDays = absenceCounts.GetValueOrDefault(e.Id);
                var breakdown = PayrollCalculator.Calculate(new PayslipInput(
                    baseSalary, e.IncentivePercentage, e.HireDate, absenceDays, periodEnd, serviceRules, absenceRules));

                payslips.Add(new Payslip
                {
                    EmployeeId = e.Id,
                    EmployeeName = e.FullName,
                    DepartmentId = e.DepartmentId,
                    DepartmentNameAr = e.NameAr,
                    DepartmentNameEn = e.NameEn,
                    Grade = e.Grade,
                    ServiceYears = breakdown.ServiceYears,
                    AbsenceDays = absenceDays,
                    BaseSalary = baseSalary,
                    DepartmentIncentivePercentage = breakdown.DepartmentPercentage,
                    DepartmentIncentive = breakdown.DepartmentIncentive,
                    ServiceYearsIncentivePercentage = breakdown.ServicePercentage,
                    ServiceYearsIncentive = breakdown.ServiceIncentive,
                    AbsenceBonusPercentage = breakdown.BonusPercentage,
                    AbsenceBonus = breakdown.Bonus,
                    AbsenceDeductionPercentage = breakdown.DeductionPercentage,
                    AbsenceDeduction = breakdown.Deduction,
                    NetSalary = breakdown.Net
                });
            }

            return payslips;
        }
    }
}
