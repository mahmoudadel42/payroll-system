using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Features.ServiceYearsRules.Model;

namespace Payroll.Application.Features.ServiceYearsRules.Queries.GetServiceYearsRules
{
    public sealed class GetServiceYearsRulesQueryHandler(IApplicationDbContext db)
        : IRequestHandler<GetServiceYearsRulesQuery, IReadOnlyList<ServiceYearsRuleDto>>
    {
        public async Task<IReadOnlyList<ServiceYearsRuleDto>> Handle(GetServiceYearsRulesQuery request, CancellationToken ct) =>
            await db.ServiceYearsIncentiveRules.AsNoTracking()
                .OrderBy(r => r.MinYears)
                .Select(r => new ServiceYearsRuleDto(r.Id, r.MinYears, r.Percentage))
                .ToListAsync(ct);
    }
}
