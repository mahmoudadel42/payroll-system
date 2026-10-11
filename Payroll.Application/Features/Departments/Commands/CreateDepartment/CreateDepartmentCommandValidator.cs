using FluentValidation;
using Payroll.Application.Resources;

namespace Payroll.Application.Features.Departments.Commands.CreateDepartment
{
    public sealed class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
    {
        public CreateDepartmentCommandValidator()
        {
            RuleFor(x => x.NameAr).NotEmpty().MaximumLength(100);
            RuleFor(x => x.NameEn).NotEmpty().MaximumLength(100);
            RuleFor(x => x.IncentivePercentage).InclusiveBetween(0, 100)
                .WithMessage(_ => AppMessages.Get("percentage.outOfRange"));
        }
    }
}
