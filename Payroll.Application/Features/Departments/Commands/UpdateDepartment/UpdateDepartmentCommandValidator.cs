using FluentValidation;
using Payroll.Application.Resources;

namespace Payroll.Application.Features.Departments.Commands.UpdateDepartment
{
    public sealed class UpdateDepartmentCommandValidator : AbstractValidator<UpdateDepartmentCommand>
    {
        public UpdateDepartmentCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.NameAr).NotEmpty().MaximumLength(100);
            RuleFor(x => x.NameEn).NotEmpty().MaximumLength(100);
            RuleFor(x => x.IncentivePercentage).InclusiveBetween(0, 100)
                .WithMessage(_ => AppMessages.Get("percentage.outOfRange"));
        }
    }
}
