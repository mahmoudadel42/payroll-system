using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;

namespace Payroll.Application.Features.ServiceYearsRules.Commands.UpdateServiceYearsRule
{
    public sealed class UpdateServiceYearsRuleCommandHandler(IApplicationDbContext db)
        : IRequestHandler<UpdateServiceYearsRuleCommand, Result>
    {
        public async Task<Result> Handle(UpdateServiceYearsRuleCommand request, CancellationToken ct)
        {
            var rule = await db.ServiceYearsIncentiveRules.FirstOrDefaultAsync(r => r.Id == request.Id, ct);
            if (rule is null)
                return Error.NotFound("rule", request.Id);

            if (await db.ServiceYearsIncentiveRules.AnyAsync(r => r.Id != request.Id && r.MinYears == request.MinYears, ct))
                return Error.Conflict("rule.duplicateThreshold", "A rule with the same threshold already exists.");

            rule.MinYears = request.MinYears;
            rule.Percentage = request.Percentage;
            await db.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
