using FluentValidation;
using Payroll.Application.Features.Employees.Model;

namespace Payroll.Application.Features.Employees.Commands.CreateEmployee
{
    public sealed class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
    {
        public CreateEmployeeCommandValidator(TimeProvider time)
        {
            Include(new EmployeeFieldsValidator(time));
        }
    }
}
