using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;
using Payroll.Domain.Entities;

namespace Payroll.Application.Features.ServiceYearsRules.Commands.CreateServiceYearsRule
{
    public sealed class CreateServiceYearsRuleCommandHandler(IApplicationDbContext db)
        : IRequestHandler<CreateServiceYearsRuleCommand, Result<int>>
    {
        public async Task<Result<int>> Handle(CreateServiceYearsRuleCommand request, CancellationToken ct)
        {
            if (await db.ServiceYearsIncentiveRules.AnyAsync(r => r.MinYears == request.MinYears, ct))
                return Error.Conflict("rule.duplicateThreshold", "A rule with the same threshold already exists.");

            var rule = new ServiceYearsIncentiveRule { MinYears = request.MinYears, Percentage = request.Percentage };
            db.ServiceYearsIncentiveRules.Add(rule);
            await db.SaveChangesAsync(ct);
            return rule.Id;
        }
    }
}
