using Payroll.Domain.Enums;

namespace Payroll.Application.Features.Employees.Model
{
    public interface IEmployeeFields
    {
        string FullName { get; }
        DateOnly BirthDate { get; }
        string Address { get; }
        string Phone { get; }
        string Email { get; }
        JobGrade Grade { get; }
        int DepartmentId { get; }
        DateOnly HireDate { get; }
    }
}
