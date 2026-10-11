using MediatR;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.Absences.Model;

namespace Payroll.Application.Features.Absences.Queries.GetAbsences
{
    public sealed record GetAbsencesQuery : PagedRequest, IRequest<PagedResult<AbsenceDto>>
    {
        public int? EmployeeId { get; init; }
        public int? DepartmentId { get; init; }
        public DateOnly? From { get; init; }
        public DateOnly? To { get; init; }
    }
}
