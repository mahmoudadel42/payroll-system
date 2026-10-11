using MediatR;
using Payroll.Application.Common.Models;

namespace Payroll.Application.Features.AbsenceRules.Commands.DeleteAbsenceRule
{
    public sealed record DeleteAbsenceRuleCommand(int Id) : IRequest<Result>;
}
