using FluentValidation;
using Payroll.Application.Resources;

namespace Payroll.Application.Features.Employees.Model
{
    public sealed class EmployeeFieldsValidator : AbstractValidator<IEmployeeFields>
    {
        public EmployeeFieldsValidator(TimeProvider time)
        {
            var today = DateOnly.FromDateTime(time.GetLocalNow().DateTime);

            RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Address).NotEmpty().MaximumLength(250);
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(150);
            RuleFor(x => x.Phone).NotEmpty().MaximumLength(20)
                .Matches(@"^\+?[0-9]{7,19}$").WithMessage(_ => AppMessages.Get("phone.invalid"));
            RuleFor(x => x.Grade).IsInEnum();
            RuleFor(x => x.DepartmentId).GreaterThan(0);
            RuleFor(x => x.BirthDate).LessThan(today);
            RuleFor(x => x.HireDate)
                .LessThanOrEqualTo(today).WithMessage(_ => AppMessages.Get("employee.hireDateFuture"))
                .Must((x, hireDate) => hireDate >= x.BirthDate.AddYears(18))
                .WithMessage(_ => AppMessages.Get("employee.underage"));
        }
    }
}
