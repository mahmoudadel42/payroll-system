using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;
using Payroll.Domain.Enums;

namespace Payroll.Application.Features.Absences.Model
{
    internal static class AbsenceChecks
    {
        public static Error MonthLocked =>
            Error.Rule("absence.payrollLocked", "The payroll for this month is approved; absences can no longer change.");

        public static Error BeforeHireDate =>
            Error.Rule("absence.outsideEmployment", "Absence date is before the employee's hire date.");

        public static Error Duplicate =>
            Error.Conflict("absence.duplicate", "The employee is already recorded absent on this date.");

        public static Task<bool> IsMonthLockedAsync(IApplicationDbContext db, DateOnly date, CancellationToken ct) =>
            db.PayrollRuns.AnyAsync(r => r.Year == date.Year && r.Month == date.Month && r.Status == PayrollRunStatus.Approved, ct);
    }
}
