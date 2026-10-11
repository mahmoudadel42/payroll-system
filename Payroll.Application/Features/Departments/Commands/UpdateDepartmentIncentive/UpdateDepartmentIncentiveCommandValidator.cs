using FluentValidation;
using Payroll.Application.Resources;

namespace Payroll.Application.Features.Departments.Commands.UpdateDepartmentIncentive
{
    public sealed class UpdateDepartmentIncentiveCommandValidator : AbstractValidator<UpdateDepartmentIncentiveCommand>
    {
        public UpdateDepartmentIncentiveCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.IncentivePercentage).InclusiveBetween(0, 100)
                .WithMessage(_ => AppMessages.Get("percentage.outOfRange"));
        }
    }
}
