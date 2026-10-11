using MediatR;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.Departments.Model;

namespace Payroll.Application.Features.Departments.Queries.GetDepartmentById
{
    public sealed record GetDepartmentByIdQuery(int Id) : IRequest<Result<DepartmentDto>>;
}
