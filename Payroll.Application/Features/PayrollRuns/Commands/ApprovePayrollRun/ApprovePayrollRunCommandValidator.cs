using FluentValidation;

namespace Payroll.Application.Features.PayrollRuns.Commands.ApprovePayrollRun
{
    public sealed class ApprovePayrollRunCommandValidator : AbstractValidator<ApprovePayrollRunCommand>
    {
        public ApprovePayrollRunCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}
