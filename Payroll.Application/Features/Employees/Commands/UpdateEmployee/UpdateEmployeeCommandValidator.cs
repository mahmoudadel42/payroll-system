using FluentValidation;
using Payroll.Application.Features.Employees.Model;

namespace Payroll.Application.Features.Employees.Commands.UpdateEmployee
{
    public sealed class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
    {
        public UpdateEmployeeCommandValidator(TimeProvider time)
        {
            RuleFor(x => x.Id).GreaterThan(0);
            Include(new EmployeeFieldsValidator(time));
        }
    }
}
