using FluentValidation;

namespace Payroll.Application.Features.ServiceYearsRules.Commands.DeleteServiceYearsRule
{
    public sealed class DeleteServiceYearsRuleCommandValidator : AbstractValidator<DeleteServiceYearsRuleCommand>
    {
        public DeleteServiceYearsRuleCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}
