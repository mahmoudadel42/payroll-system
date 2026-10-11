using MediatR;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.Reports.Model;

namespace Payroll.Application.Features.Reports.Queries.GetSalariesReport
{
    public sealed record GetSalariesReportQuery : IRequest<Result<SalariesReportDto>>, IMonthlyReportFilter
    {
        public int Year { get; init; }
        public int Month { get; init; }
        public int? DepartmentId { get; init; }
        public int? EmployeeId { get; init; }
    }
}
