using MediatR;
using Payroll.Application.Common.Models;

namespace Payroll.Application.Features.Absences.Commands.DeleteAbsence
{
    public sealed record DeleteAbsenceCommand(int Id) : IRequest<Result>;
}
