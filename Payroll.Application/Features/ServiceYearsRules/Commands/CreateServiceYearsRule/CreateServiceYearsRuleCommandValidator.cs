using FluentValidation;
using Payroll.Application.Resources;

namespace Payroll.Application.Features.ServiceYearsRules.Commands.CreateServiceYearsRule
{
    public sealed class CreateServiceYearsRuleCommandValidator : AbstractValidator<CreateServiceYearsRuleCommand>
    {
        public CreateServiceYearsRuleCommandValidator()
        {
            RuleFor(x => x.MinYears).GreaterThanOrEqualTo(0)
                .WithMessage(_ => AppMessages.Get("minYears.negative"));
            RuleFor(x => x.Percentage).InclusiveBetween(0, 100)
                .WithMessage(_ => AppMessages.Get("percentage.outOfRange"));
        }
    }
}
