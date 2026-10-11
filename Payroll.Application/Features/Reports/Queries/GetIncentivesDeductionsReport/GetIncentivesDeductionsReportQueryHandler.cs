using MediatR;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.Reports.Model;
using Payroll.Application.Features.Reports.Services;

namespace Payroll.Application.Features.Reports.Queries.GetIncentivesDeductionsReport
{
    public sealed class GetIncentivesDeductionsReportQueryHandler(IApplicationDbContext db, IPayrollDataService payrollData)
        : IRequestHandler<GetIncentivesDeductionsReportQuery, Result<IncentivesDeductionsReportDto>>
    {
        public async Task<Result<IncentivesDeductionsReportDto>> Handle(GetIncentivesDeductionsReportQuery request, CancellationToken ct)
        {
            var loaded = await MonthlyPayslipsLoader.LoadAsync(db, payrollData, request, ct);
            if (!loaded.IsSuccess)
                return loaded.Error!;

            var (isPreview, status, payslips) = loaded.Value!;
            var rows = payslips.Select(p =>
            {
                var incentives = p.DepartmentIncentive + p.ServiceYearsIncentive + p.AbsenceBonus;
                return new IncentivesDeductionsRow(
                    p.EmployeeId, p.EmployeeName, p.DepartmentName, p.ServiceYears, p.AbsenceDays,
                    p.DepartmentIncentivePercentage, p.DepartmentIncentive,
                    p.ServiceYearsIncentivePercentage, p.ServiceYearsIncentive,
                    p.AbsenceBonusPercentage, p.AbsenceBonus,
                    p.AbsenceDeductionPercentage, p.AbsenceDeduction,
                    incentives, incentives - p.AbsenceDeduction);
            }).ToList();

            var totals = new IncentivesDeductionsTotals(
                rows.Sum(r => r.DepartmentIncentive),
                rows.Sum(r => r.ServiceYearsIncentive),
                rows.Sum(r => r.AbsenceBonus),
                rows.Sum(r => r.AbsenceDeduction),
                rows.Sum(r => r.TotalIncentives),
                rows.Sum(r => r.NetEffect));

            return new IncentivesDeductionsReportDto(request.Year, request.Month, isPreview, status, rows, totals);
        }
    }
}
