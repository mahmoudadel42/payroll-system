using MediatR;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.Employees.Model;
using Payroll.Domain.Enums;

namespace Payroll.Application.Features.Employees.Queries.GetEmployees
{
    public sealed record GetEmployeesQuery : PagedRequest, IRequest<PagedResult<EmployeeDto>>
    {
        public int? DepartmentId { get; init; }
        public JobGrade? Grade { get; init; }
    }
}
