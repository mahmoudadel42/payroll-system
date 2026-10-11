using FluentValidation;

namespace Payroll.Application.Features.Reports.Queries.GetEmployeesReport
{
    public sealed class GetEmployeesReportQueryValidator : AbstractValidator<GetEmployeesReportQuery>
    {
        public GetEmployeesReportQueryValidator()
        {
            RuleFor(x => x.DepartmentId).GreaterThan(0).When(x => x.DepartmentId.HasValue);
            RuleFor(x => x.Grade).IsInEnum().When(x => x.Grade.HasValue);
        }
    }
}
