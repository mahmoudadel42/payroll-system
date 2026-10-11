using Payroll.Domain.Enums;

namespace Payroll.Application.Features.Employees.Model
{
    public sealed record EmployeeDto
    {
        public int Id { get; init; }
        public string FullName { get; init; } = string.Empty;
        public DateOnly BirthDate { get; init; }
        public string Address { get; init; } = string.Empty;
        public string Phone { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public JobGrade Grade { get; init; }
        public int DepartmentId { get; init; }
        public string DepartmentName { get; init; } = string.Empty;
        public DateOnly HireDate { get; init; }
    }
}
