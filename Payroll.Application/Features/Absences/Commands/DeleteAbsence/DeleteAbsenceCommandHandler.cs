using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.Absences.Model;

namespace Payroll.Application.Features.Absences.Commands.DeleteAbsence
{
    public sealed class DeleteAbsenceCommandHandler(IApplicationDbContext db)
        : IRequestHandler<DeleteAbsenceCommand, Result>
    {
        public async Task<Result> Handle(DeleteAbsenceCommand request, CancellationToken ct)
        {
            var absence = await db.AbsenceRecords.FirstOrDefaultAsync(a => a.Id == request.Id, ct);
            if (absence is null)
                return Error.NotFound("absence", request.Id);

            if (await AbsenceChecks.IsMonthLockedAsync(db, absence.Date, ct))
                return AbsenceChecks.MonthLocked;

            db.AbsenceRecords.Remove(absence);
            await db.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
