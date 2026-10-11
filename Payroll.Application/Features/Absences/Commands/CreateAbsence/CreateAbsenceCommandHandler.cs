using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.Absences.Model;
using Payroll.Domain.Entities;

namespace Payroll.Application.Features.Absences.Commands.CreateAbsence
{
    public sealed class CreateAbsenceCommandHandler(IApplicationDbContext db)
        : IRequestHandler<CreateAbsenceCommand, Result<int>>
    {
        public async Task<Result<int>> Handle(CreateAbsenceCommand request, CancellationToken ct)
        {
            var hireDate = await db.Employees
                .Where(e => e.Id == request.EmployeeId)
                .Select(e => (DateOnly?)e.HireDate)
                .FirstOrDefaultAsync(ct);
            if (hireDate is null)
                return Error.NotFound("employee", request.EmployeeId);

            if (request.Date < hireDate)
                return AbsenceChecks.BeforeHireDate;

            if (await AbsenceChecks.IsMonthLockedAsync(db, request.Date, ct))
                return AbsenceChecks.MonthLocked;

            if (await db.AbsenceRecords.AnyAsync(a => a.EmployeeId == request.EmployeeId && a.Date == request.Date, ct))
                return AbsenceChecks.Duplicate;

            var absence = new AbsenceRecord
            {
                EmployeeId = request.EmployeeId,
                Date = request.Date,
                Reason = string.IsNullOrWhiteSpace(request.Reason) ? null : request.Reason.Trim()
            };
            db.AbsenceRecords.Add(absence);
            await db.SaveChangesAsync(ct);
            return absence.Id;
        }
    }
}
