using Payroll.Domain.Enums;

namespace Payroll.Application.Features.Reports.Model
{
    public sealed record EmployeesReportDto(
        DateOnly AsOf,
        IReadOnlyList<EmployeesReportRow> Rows,
        int TotalEmployees,
        IReadOnlyList<CountItem> ByDepartment,
        IReadOnlyList<CountItem> ByGrade);

    public sealed record EmployeesReportRow(
        int EmployeeId,
        string FullName,
        string Email,
        string Phone,
        string DepartmentName,
        JobGrade Grade,
        DateOnly BirthDate,
        DateOnly HireDate,
        int ServiceYears,
        decimal? BaseSalary);

    public sealed record CountItem(string Name, int Count);
}
