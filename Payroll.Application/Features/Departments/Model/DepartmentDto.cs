namespace Payroll.Application.Features.Departments.Model
{
    public sealed record DepartmentDto
    {
        public int Id { get; init; }
        public string NameAr { get; init; } = string.Empty;
        public string NameEn { get; init; } = string.Empty;
        public decimal IncentivePercentage { get; init; }
        public int EmployeesCount { get; init; }
    }
}
