using MediatR;
using Payroll.Application.Common.Models;

namespace Payroll.Application.Features.Absences.Commands.UpdateAbsence
{
    public sealed record UpdateAbsenceCommand(int Id, DateOnly Date, string? Reason) : IRequest<Result>;
}
