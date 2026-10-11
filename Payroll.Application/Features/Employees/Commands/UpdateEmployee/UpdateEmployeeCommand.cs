using MediatR;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.Employees.Model;
using Payroll.Domain.Enums;

namespace Payroll.Application.Features.Employees.Commands.UpdateEmployee
{
    public sealed record UpdateEmployeeCommand(
        int Id,
        string FullName,
        DateOnly BirthDate,
        string Address,
        string Phone,
        string Email,
        JobGrade Grade,
        int DepartmentId,
        DateOnly HireDate) : IRequest<Result>, IEmployeeFields;
}
