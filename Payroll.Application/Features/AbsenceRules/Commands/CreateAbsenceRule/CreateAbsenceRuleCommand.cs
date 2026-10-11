using MediatR;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.AbsenceRules.Model;
using Payroll.Domain.Enums;

namespace Payroll.Application.Features.AbsenceRules.Commands.CreateAbsenceRule
{
    public sealed record CreateAbsenceRuleCommand(AbsenceRuleType Type, int ThresholdDays, decimal Percentage)
        : IRequest<Result<int>>, IAbsenceRuleFields;
}
