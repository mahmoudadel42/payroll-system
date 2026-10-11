using MediatR;
using Payroll.Application.Common.Models;
using Payroll.Domain.Enums;

namespace Payroll.Application.Features.GradeSalaries.Commands.CreateGradeSalary
{
    public sealed record CreateGradeSalaryCommand(JobGrade Grade, decimal BaseSalary) : IRequest<Result<int>>;
}
