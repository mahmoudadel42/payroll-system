using MediatR;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.Reports.Model;
using Payroll.Application.Features.Reports.Services;

namespace Payroll.Application.Features.Reports.Queries.GetSalariesReport
{
    public sealed class GetSalariesReportQueryHandler(IApplicationDbContext db, IPayrollDataService payrollData)
        : IRequestHandler<GetSalariesReportQuery, Result<SalariesReportDto>>
    {
        public async Task<Result<SalariesReportDto>> Handle(GetSalariesReportQuery request, CancellationToken ct)
        {
            var loaded = await MonthlyPayslipsLoader.LoadAsync(db, payrollData, request, ct);
            if (!loaded.IsSuccess)
                return loaded.Error!;

            var (isPreview, status, rows) = loaded.Value!;
            var totals = new SalariesTotals(
                rows.Count,
                rows.Sum(r => r.BaseSalary),
                rows.Sum(r => r.DepartmentIncentive),
                rows.Sum(r => r.ServiceYearsIncentive),
                rows.Sum(r => r.AbsenceBonus),
                rows.Sum(r => r.AbsenceDeduction),
                rows.Sum(r => r.NetSalary));

            return new SalariesReportDto(request.Year, request.Month, isPreview, status, rows, totals);
        }
    }
}
