using FluentValidation;

namespace Payroll.Application.Features.Employees.Commands.DeleteEmployee
{
    public sealed class DeleteEmployeeCommandValidator : AbstractValidator<DeleteEmployeeCommand>
    {
        public DeleteEmployeeCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}
