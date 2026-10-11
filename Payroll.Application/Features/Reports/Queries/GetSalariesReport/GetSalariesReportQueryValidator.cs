using FluentValidation;
using Payroll.Application.Features.Reports.Model;

namespace Payroll.Application.Features.Reports.Queries.GetSalariesReport
{
    public sealed class GetSalariesReportQueryValidator : AbstractValidator<GetSalariesReportQuery>
    {
        public GetSalariesReportQueryValidator()
        {
            Include(new MonthlyReportFilterValidator());
        }
    }
}
