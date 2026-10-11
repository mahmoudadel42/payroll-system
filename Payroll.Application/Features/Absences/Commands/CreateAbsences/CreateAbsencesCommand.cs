using MediatR;
using Payroll.Application.Common.Models;

namespace Payroll.Application.Features.Absences.Commands.CreateAbsences
{
    /// <summary>Records several absence days for one employee; dates already recorded are skipped.</summary>
    public sealed record CreateAbsencesCommand(int EmployeeId, IReadOnlyList<DateOnly> Dates, string? Reason)
        : IRequest<Result<int>>;
}
