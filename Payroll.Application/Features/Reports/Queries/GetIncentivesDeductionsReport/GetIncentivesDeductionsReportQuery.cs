using MediatR;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.Reports.Model;

namespace Payroll.Application.Features.Reports.Queries.GetIncentivesDeductionsReport
{
    public sealed record GetIncentivesDeductionsReportQuery : IRequest<Result<IncentivesDeductionsReportDto>>, IMonthlyReportFilter
    {
        public int Year { get; init; }
        public int Month { get; init; }
        public int? DepartmentId { get; init; }
        public int? EmployeeId { get; init; }
    }
}
