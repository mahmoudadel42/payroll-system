using MediatR;
using Payroll.Application.Common.Models;
using Payroll.Domain.Enums;

namespace Payroll.Application.Features.GradeSalaries.Commands.UpdateGradeSalary
{
    public sealed record UpdateGradeSalaryCommand(JobGrade Grade, decimal BaseSalary) : IRequest<Result>;
}
