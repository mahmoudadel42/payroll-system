using FluentValidation;
using Payroll.Application.Resources;

namespace Payroll.Application.Features.GradeSalaries.Commands.CreateGradeSalary
{
    public sealed class CreateGradeSalaryCommandValidator : AbstractValidator<CreateGradeSalaryCommand>
    {
        public CreateGradeSalaryCommandValidator()
        {
            RuleFor(x => x.Grade).IsInEnum();
            RuleFor(x => x.BaseSalary).GreaterThan(0)
                .WithMessage(_ => AppMessages.Get("baseSalary.positive"));
        }
    }
}
