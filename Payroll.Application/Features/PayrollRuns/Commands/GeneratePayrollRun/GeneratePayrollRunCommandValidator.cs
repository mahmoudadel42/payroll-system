using FluentValidation;
using Payroll.Application.Features.PayrollRuns.Model;

namespace Payroll.Application.Features.PayrollRuns.Commands.GeneratePayrollRun
{
    public sealed class GeneratePayrollRunCommandValidator : AbstractValidator<GeneratePayrollRunCommand>
    {
        public GeneratePayrollRunCommandValidator()
        {
            Include(new PayrollPeriodValidator());
        }
    }
}
