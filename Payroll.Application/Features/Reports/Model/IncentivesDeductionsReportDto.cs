using Payroll.Domain.Enums;

namespace Payroll.Application.Features.Reports.Model
{
    public sealed record IncentivesDeductionsReportDto(
        int Year,
        int Month,
        bool IsPreview,
        PayrollRunStatus? RunStatus,
        IReadOnlyList<IncentivesDeductionsRow> Rows,
        IncentivesDeductionsTotals Totals);

    public sealed record IncentivesDeductionsRow(
        int EmployeeId,
        string EmployeeName,
        string DepartmentName,
        int ServiceYears,
        int AbsenceDays,
        decimal DepartmentIncentivePercentage,
        decimal DepartmentIncentive,
        decimal ServiceYearsIncentivePercentage,
        decimal ServiceYearsIncentive,
        decimal AbsenceBonusPercentage,
        decimal AbsenceBonus,
        decimal AbsenceDeductionPercentage,
        decimal AbsenceDeduction,
        decimal TotalIncentives,
        decimal NetEffect);

    public sealed record IncentivesDeductionsTotals(
        decimal DepartmentIncentive,
        decimal ServiceYearsIncentive,
        decimal AbsenceBonus,
        decimal AbsenceDeduction,
        decimal TotalIncentives,
        decimal NetEffect);
}
