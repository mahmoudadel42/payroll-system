using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;

namespace Payroll.Application.Features.ServiceYearsRules.Commands.DeleteServiceYearsRule
{
    public sealed class DeleteServiceYearsRuleCommandHandler(IApplicationDbContext db)
        : IRequestHandler<DeleteServiceYearsRuleCommand, Result>
    {
        public async Task<Result> Handle(DeleteServiceYearsRuleCommand request, CancellationToken ct)
        {
            var rule = await db.ServiceYearsIncentiveRules.FirstOrDefaultAsync(r => r.Id == request.Id, ct);
            if (rule is null)
                return Error.NotFound("rule", request.Id);

            db.ServiceYearsIncentiveRules.Remove(rule);
            await db.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
