namespace Payroll.Application.Features.Employees.Model
{
    public sealed record EmployeeDetailsDto(
        EmployeeDto Employee,
        decimal? BaseSalary,
        decimal DepartmentIncentivePercentage,
        int ServiceYears);
}
