using FluentValidation;
using Payroll.Application.Features.Reports.Model;

namespace Payroll.Application.Features.Reports.Queries.GetIncentivesDeductionsReport
{
    public sealed class GetIncentivesDeductionsReportQueryValidator : AbstractValidator<GetIncentivesDeductionsReportQuery>
    {
        public GetIncentivesDeductionsReportQueryValidator()
        {
            Include(new MonthlyReportFilterValidator());
        }
    }
}
