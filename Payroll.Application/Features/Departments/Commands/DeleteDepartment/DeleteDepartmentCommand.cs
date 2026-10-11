using MediatR;
using Payroll.Application.Common.Models;

namespace Payroll.Application.Features.Departments.Commands.DeleteDepartment
{
    public sealed record DeleteDepartmentCommand(int Id) : IRequest<Result>;
}
