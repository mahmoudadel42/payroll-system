using FluentValidation;
using Payroll.Application.Features.PayrollRuns.Model;

namespace Payroll.Application.Features.Reports.Model
{
    public interface IMonthlyReportFilter : IPayrollPeriod
    {
        int? DepartmentId { get; }
        int? EmployeeId { get; }
    }

    public sealed class MonthlyReportFilterValidator : AbstractValidator<IMonthlyReportFilter>
    {
        public MonthlyReportFilterValidator()
        {
            Include(new PayrollPeriodValidator());
            RuleFor(x => x.DepartmentId).GreaterThan(0).When(x => x.DepartmentId.HasValue);
            RuleFor(x => x.EmployeeId).GreaterThan(0).When(x => x.EmployeeId.HasValue);
        }
    }
}
