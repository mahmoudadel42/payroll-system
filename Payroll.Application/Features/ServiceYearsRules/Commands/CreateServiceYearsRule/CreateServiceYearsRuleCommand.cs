using MediatR;
using Payroll.Application.Common.Models;

namespace Payroll.Application.Features.ServiceYearsRules.Commands.CreateServiceYearsRule
{
    public sealed record CreateServiceYearsRuleCommand(int MinYears, decimal Percentage) : IRequest<Result<int>>;
}
