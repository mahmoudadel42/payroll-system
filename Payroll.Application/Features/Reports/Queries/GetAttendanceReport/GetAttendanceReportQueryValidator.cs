using FluentValidation;
using Payroll.Application.Features.Reports.Model;

namespace Payroll.Application.Features.Reports.Queries.GetAttendanceReport
{
    public sealed class GetAttendanceReportQueryValidator : AbstractValidator<GetAttendanceReportQuery>
    {
        public GetAttendanceReportQueryValidator()
        {
            Include(new MonthlyReportFilterValidator());
        }
    }
}
