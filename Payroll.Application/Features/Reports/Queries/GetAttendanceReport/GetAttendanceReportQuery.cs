using MediatR;
using Payroll.Application.Features.Reports.Model;

namespace Payroll.Application.Features.Reports.Queries.GetAttendanceReport
{
    public sealed record GetAttendanceReportQuery : IRequest<AttendanceReportDto>, IMonthlyReportFilter
    {
        public int Year { get; init; }
        public int Month { get; init; }
        public int? DepartmentId { get; init; }
        public int? EmployeeId { get; init; }
    }
}
