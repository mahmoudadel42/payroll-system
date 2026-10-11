using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Features.AbsenceRules.Model;

namespace Payroll.Application.Features.AbsenceRules.Queries.GetAbsenceRules
{
    public sealed class GetAbsenceRulesQueryHandler(IApplicationDbContext db)
        : IRequestHandler<GetAbsenceRulesQuery, IReadOnlyList<AbsenceRuleDto>>
    {
        public async Task<IReadOnlyList<AbsenceRuleDto>> Handle(GetAbsenceRulesQuery request, CancellationToken ct) =>
            await db.AbsenceRules.AsNoTracking()
                .OrderByDescending(r => r.Type)
                .ThenBy(r => r.ThresholdDays)
                .Select(r => new AbsenceRuleDto(r.Id, r.Type, r.ThresholdDays, r.Percentage))
                .ToListAsync(ct);
    }
}
