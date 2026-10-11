using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.Absences.Model;

namespace Payroll.Application.Features.Absences.Commands.UpdateAbsence
{
    public sealed class UpdateAbsenceCommandHandler(IApplicationDbContext db)
        : IRequestHandler<UpdateAbsenceCommand, Result>
    {
        public async Task<Result> Handle(UpdateAbsenceCommand request, CancellationToken ct)
        {
            var absence = await db.AbsenceRecords.Include(a => a.Employee).FirstOrDefaultAsync(a => a.Id == request.Id, ct);
            if (absence is null)
                return Error.NotFound("absence", request.Id);

            if (request.Date < absence.Employee.HireDate)
                return AbsenceChecks.BeforeHireDate;

            if (await AbsenceChecks.IsMonthLockedAsync(db, absence.Date, ct) ||
                await AbsenceChecks.IsMonthLockedAsync(db, request.Date, ct))
                return AbsenceChecks.MonthLocked;

            if (await db.AbsenceRecords.AnyAsync(a => a.Id != request.Id && a.EmployeeId == absence.EmployeeId && a.Date == request.Date, ct))
                return AbsenceChecks.Duplicate;

            absence.Date = request.Date;
            absence.Reason = string.IsNullOrWhiteSpace(request.Reason) ? null : request.Reason.Trim();
            await db.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
