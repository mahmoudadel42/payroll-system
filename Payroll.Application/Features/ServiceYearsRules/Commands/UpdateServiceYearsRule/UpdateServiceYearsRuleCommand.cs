using MediatR;
using Payroll.Application.Common.Models;

namespace Payroll.Application.Features.ServiceYearsRules.Commands.UpdateServiceYearsRule
{
    public sealed record UpdateServiceYearsRuleCommand(int Id, int MinYears, decimal Percentage) : IRequest<Result>;
}
