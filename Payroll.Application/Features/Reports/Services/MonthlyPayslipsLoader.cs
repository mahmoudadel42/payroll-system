using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Localization;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.PayrollRuns.Model;
using Payroll.Application.Features.Reports.Model;
using Payroll.Domain.Enums;

namespace Payroll.Application.Features.Reports.Services
{
    internal sealed record MonthlyPayslips(bool IsPreview, PayrollRunStatus? RunStatus, IReadOnlyList<PayslipDto> Payslips);

    internal static class MonthlyPayslipsLoader
    {
        /// <summary>Stored payslips when the month has a payroll run, otherwise a live calculation (preview).</summary>
        public static async Task<Result<MonthlyPayslips>> LoadAsync(
            IApplicationDbContext db, IPayrollDataService payrollData, IMonthlyReportFilter filter, CancellationToken ct)
        {
            var arabic = CurrentCulture.IsArabic;

            var run = await db.PayrollRuns.AsNoTracking()
                .Where(r => r.Year == filter.Year && r.Month == filter.Month)
                .Select(r => new { r.Id, r.Status })
                .FirstOrDefaultAsync(ct);

            if (run is not null)
            {
                var query = db.Payslips.AsNoTracking().Where(p => p.PayrollRunId == run.Id);
                if (filter.DepartmentId is { } departmentId)
                    query = query.Where(p => p.DepartmentId == departmentId);
                if (filter.EmployeeId is { } employeeId)
                    query = query.Where(p => p.EmployeeId == employeeId);

                var stored = await query
                    .OrderBy(p => p.EmployeeName)
                    .Select(PayslipProjection.ToDto(arabic))
                    .ToListAsync(ct);
                return new MonthlyPayslips(false, run.Status, stored);
            }

            var built = await payrollData.BuildPayslipsAsync(filter.Year, filter.Month, filter.DepartmentId, filter.EmployeeId, ct);
            if (!built.IsSuccess)
                return built.Error!;

            var toDto = PayslipProjection.ToDto(arabic).Compile();
            return new MonthlyPayslips(true, null, built.Value!.Select(toDto).ToList());
        }
    }
}
