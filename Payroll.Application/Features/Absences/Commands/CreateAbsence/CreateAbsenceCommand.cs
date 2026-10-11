using MediatR;
using Payroll.Application.Common.Models;

namespace Payroll.Application.Features.Absences.Commands.CreateAbsence
{
    public sealed record CreateAbsenceCommand(int EmployeeId, DateOnly Date, string? Reason) : IRequest<Result<int>>;
}
