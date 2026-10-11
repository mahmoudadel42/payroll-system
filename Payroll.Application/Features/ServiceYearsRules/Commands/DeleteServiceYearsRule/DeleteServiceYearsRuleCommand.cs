using MediatR;
using Payroll.Application.Common.Models;

namespace Payroll.Application.Features.ServiceYearsRules.Commands.DeleteServiceYearsRule
{
    public sealed record DeleteServiceYearsRuleCommand(int Id) : IRequest<Result>;
}
