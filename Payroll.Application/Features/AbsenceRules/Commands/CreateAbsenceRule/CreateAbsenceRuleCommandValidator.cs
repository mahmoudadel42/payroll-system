using FluentValidation;
using Payroll.Application.Features.AbsenceRules.Model;

namespace Payroll.Application.Features.AbsenceRules.Commands.CreateAbsenceRule
{
    public sealed class CreateAbsenceRuleCommandValidator : AbstractValidator<CreateAbsenceRuleCommand>
    {
        public CreateAbsenceRuleCommandValidator()
        {
            Include(new AbsenceRuleFieldsValidator());
        }
    }
}
