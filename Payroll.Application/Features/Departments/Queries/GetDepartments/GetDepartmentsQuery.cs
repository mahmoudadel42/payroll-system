using MediatR;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.Departments.Model;

namespace Payroll.Application.Features.Departments.Queries.GetDepartments
{
    public sealed record GetDepartmentsQuery : PagedRequest, IRequest<PagedResult<DepartmentDto>>;
}
