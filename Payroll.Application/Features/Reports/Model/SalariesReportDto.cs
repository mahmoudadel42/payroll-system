using Payroll.Application.Features.PayrollRuns.Model;
using Payroll.Domain.Enums;

namespace Payroll.Application.Features.Reports.Model
{
    public sealed record SalariesReportDto(
        int Year,
        int Month,
        bool IsPreview,
        PayrollRunStatus? RunStatus,
        IReadOnlyList<PayslipDto> Rows,
        SalariesTotals Totals);

    public sealed record SalariesTotals(
        int EmployeesCount,
        decimal BaseSalary,
        decimal DepartmentIncentive,
        decimal ServiceYearsIncentive,
        decimal AbsenceBonus,
        decimal AbsenceDeduction,
        decimal NetSalary);
}
