using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;
using Payroll.Domain.Enums;

namespace Payroll.Application.Features.AbsenceRules.Model
{
    internal static class AbsenceRuleChecks
    {
        public static async Task<Error?> CheckConflictsAsync(
            IApplicationDbContext db, int? excludeId, AbsenceRuleType type, int thresholdDays, CancellationToken ct)
        {
            var others = db.AbsenceRules.Where(r => excludeId == null || r.Id != excludeId);

            if (type == AbsenceRuleType.Bonus && await others.AnyAsync(r => r.Type == AbsenceRuleType.Bonus, ct))
                return Error.Conflict("absenceRule.bonusExists", "A bonus rule already exists. Edit it instead.");

            if (await others.AnyAsync(r => r.Type == type && r.ThresholdDays == thresholdDays, ct))
                return Error.Conflict("rule.duplicateThreshold", "A rule with the same threshold already exists.");

            return null;
        }
    }
}
