using FluentValidation;
using Payroll.Application.Resources;

namespace Payroll.Application.Features.GradeSalaries.Commands.UpdateGradeSalary
{
    public sealed class UpdateGradeSalaryCommandValidator : AbstractValidator<UpdateGradeSalaryCommand>
    {
        public UpdateGradeSalaryCommandValidator()
        {
            RuleFor(x => x.Grade).IsInEnum();
            RuleFor(x => x.BaseSalary).GreaterThan(0)
                .WithMessage(_ => AppMessages.Get("baseSalary.positive"));
        }
    }
}
