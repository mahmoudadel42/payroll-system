using Payroll.Domain.Enums;

namespace Payroll.Application.Features.AbsenceRules.Model
{
    public sealed record AbsenceRuleDto(int Id, AbsenceRuleType Type, int ThresholdDays, decimal Percentage);
}
