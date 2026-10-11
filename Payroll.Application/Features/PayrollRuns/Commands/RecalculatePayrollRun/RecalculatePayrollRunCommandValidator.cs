using FluentValidation;

namespace Payroll.Application.Features.PayrollRuns.Commands.RecalculatePayrollRun
{
    public sealed class RecalculatePayrollRunCommandValidator : AbstractValidator<RecalculatePayrollRunCommand>
    {
        public RecalculatePayrollRunCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}
