using MediatR;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.AbsenceRules.Model;
using Payroll.Domain.Enums;

namespace Payroll.Application.Features.AbsenceRules.Commands.UpdateAbsenceRule
{
    public sealed record UpdateAbsenceRuleCommand(int Id, AbsenceRuleType Type, int ThresholdDays, decimal Percentage)
        : IRequest<Result>, IAbsenceRuleFields;
}
