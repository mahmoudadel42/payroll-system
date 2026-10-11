using FluentValidation;
using Payroll.Application.Resources;

namespace Payroll.Application.Features.PayrollRuns.Model
{
    public interface IPayrollPeriod
    {
        int Year { get; }
        int Month { get; }
    }

    public sealed class PayrollPeriodValidator : AbstractValidator<IPayrollPeriod>
    {
        public PayrollPeriodValidator()
        {
            RuleFor(x => x.Year).InclusiveBetween(2000, 2100)
                .WithMessage(_ => AppMessages.Get("payroll.invalidYear"));
            RuleFor(x => x.Month).InclusiveBetween(1, 12)
                .WithMessage(_ => AppMessages.Get("payroll.invalidMonth"));
        }
    }
}
