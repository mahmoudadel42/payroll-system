namespace Payroll.Application.Features.Reports.Model
{
    public enum AbsenceEffect
    {
        None = 0,
        Bonus = 1,
        Deduction = 2
    }

    public sealed record AttendanceReportDto(
        int Year,
        int Month,
        IReadOnlyList<AttendanceReportRow> Rows,
        int TotalAbsenceDays,
        int EmployeesWithBonus,
        int EmployeesWithDeduction);

    public sealed record AttendanceReportRow(
        int EmployeeId,
        string EmployeeName,
        string DepartmentName,
        int AbsenceDays,
        string Dates,
        AbsenceEffect Effect,
        decimal EffectPercentage);
}
