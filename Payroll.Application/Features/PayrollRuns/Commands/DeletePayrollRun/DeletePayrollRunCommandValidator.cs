using FluentValidation;

namespace Payroll.Application.Features.PayrollRuns.Commands.DeletePayrollRun
{
    public sealed class DeletePayrollRunCommandValidator : AbstractValidator<DeletePayrollRunCommand>
    {
        public DeletePayrollRunCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}
