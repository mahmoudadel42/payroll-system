using MediatR;
using Payroll.Application.Common.Models;

namespace Payroll.Application.Features.Employees.Commands.DeleteEmployee
{
    public sealed record DeleteEmployeeCommand(int Id) : IRequest<Result>;
}
