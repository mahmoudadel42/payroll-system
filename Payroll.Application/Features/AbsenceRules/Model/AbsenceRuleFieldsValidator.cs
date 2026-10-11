using FluentValidation;
using Payroll.Application.Resources;
using Payroll.Domain.Enums;

namespace Payroll.Application.Features.AbsenceRules.Model
{
    public interface IAbsenceRuleFields
    {
        AbsenceRuleType Type { get; }
        int ThresholdDays { get; }
        decimal Percentage { get; }
    }

    public sealed class AbsenceRuleFieldsValidator : AbstractValidator<IAbsenceRuleFields>
    {
        public AbsenceRuleFieldsValidator()
        {
            RuleFor(x => x.Type).IsInEnum();
            RuleFor(x => x.ThresholdDays).GreaterThanOrEqualTo(0)
                .WithMessage(_ => AppMessages.Get("minDays.negative"));
            RuleFor(x => x.Percentage).InclusiveBetween(0, 100)
                .WithMessage(_ => AppMessages.Get("percentage.outOfRange"));
        }
    }
}
