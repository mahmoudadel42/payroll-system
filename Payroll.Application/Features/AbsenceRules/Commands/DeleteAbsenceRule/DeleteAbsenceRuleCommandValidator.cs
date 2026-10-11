using FluentValidation;

namespace Payroll.Application.Features.AbsenceRules.Commands.DeleteAbsenceRule
{
    public sealed class DeleteAbsenceRuleCommandValidator : AbstractValidator<DeleteAbsenceRuleCommand>
    {
        public DeleteAbsenceRuleCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}
