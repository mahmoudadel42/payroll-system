using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.Absences.Model;
using Payroll.Domain.Entities;

namespace Payroll.Application.Features.Absences.Commands.CreateAbsences
{
    public sealed class CreateAbsencesCommandHandler(IApplicationDbContext db)
        : IRequestHandler<CreateAbsencesCommand, Result<int>>
    {
        public async Task<Result<int>> Handle(CreateAbsencesCommand request, CancellationToken ct)
        {
            var hireDate = await db.Employees
                .Where(e => e.Id == request.EmployeeId)
                .Select(e => (DateOnly?)e.HireDate)
                .FirstOrDefaultAsync(ct);
            if (hireDate is null)
                return Error.NotFound("employee", request.EmployeeId);

            var dates = request.Dates.Distinct().ToList();
            if (dates.Any(d => d < hireDate))
                return AbsenceChecks.BeforeHireDate;

            foreach (var month in dates.Select(d => new DateOnly(d.Year, d.Month, 1)).Distinct())
                if (await AbsenceChecks.IsMonthLockedAsync(db, month, ct))
                    return AbsenceChecks.MonthLocked;

            var existing = await db.AbsenceRecords
                .Where(a => a.EmployeeId == request.EmployeeId && dates.Contains(a.Date))
                .Select(a => a.Date)
                .ToListAsync(ct);

            var reason = string.IsNullOrWhiteSpace(request.Reason) ? null : request.Reason.Trim();
            var toAdd = dates.Except(existing)
                .Select(d => new AbsenceRecord { EmployeeId = request.EmployeeId, Date = d, Reason = reason })
                .ToList();

            db.AbsenceRecords.AddRange(toAdd);
            await db.SaveChangesAsync(ct);
            return toAdd.Count;
        }
    }
}
