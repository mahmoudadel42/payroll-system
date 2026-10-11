using FluentValidation;
using Payroll.Application.Features.AbsenceRules.Model;

namespace Payroll.Application.Features.AbsenceRules.Commands.UpdateAbsenceRule
{
    public sealed class UpdateAbsenceRuleCommandValidator : AbstractValidator<UpdateAbsenceRuleCommand>
    {
        public UpdateAbsenceRuleCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            Include(new AbsenceRuleFieldsValidator());
        }
    }
}
