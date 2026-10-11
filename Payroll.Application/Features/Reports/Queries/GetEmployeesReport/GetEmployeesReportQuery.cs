using MediatR;
using Payroll.Application.Features.Reports.Model;
using Payroll.Domain.Enums;

namespace Payroll.Application.Features.Reports.Queries.GetEmployeesReport
{
    public sealed record GetEmployeesReportQuery : IRequest<EmployeesReportDto>
    {
        public int? DepartmentId { get; init; }
        public JobGrade? Grade { get; init; }
        /// <summary>Date used to calculate service years; defaults to today.</summary>
        public DateOnly? AsOf { get; init; }
    }
}
