using FluentValidation;
using Payroll.Application.Resources;

namespace Payroll.Application.Features.ServiceYearsRules.Commands.UpdateServiceYearsRule
{
    public sealed class UpdateServiceYearsRuleCommandValidator : AbstractValidator<UpdateServiceYearsRuleCommand>
    {
        public UpdateServiceYearsRuleCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.MinYears).GreaterThanOrEqualTo(0)
                .WithMessage(_ => AppMessages.Get("minYears.negative"));
            RuleFor(x => x.Percentage).InclusiveBetween(0, 100)
                .WithMessage(_ => AppMessages.Get("percentage.outOfRange"));
        }
    }
}
