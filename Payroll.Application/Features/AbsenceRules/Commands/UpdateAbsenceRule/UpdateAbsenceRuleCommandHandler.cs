using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.AbsenceRules.Model;

namespace Payroll.Application.Features.AbsenceRules.Commands.UpdateAbsenceRule
{
    public sealed class UpdateAbsenceRuleCommandHandler(IApplicationDbContext db)
        : IRequestHandler<UpdateAbsenceRuleCommand, Result>
    {
        public async Task<Result> Handle(UpdateAbsenceRuleCommand request, CancellationToken ct)
        {
            var rule = await db.AbsenceRules.FirstOrDefaultAsync(r => r.Id == request.Id, ct);
            if (rule is null)
                return Error.NotFound("rule", request.Id);

            if (await AbsenceRuleChecks.CheckConflictsAsync(db, request.Id, request.Type, request.ThresholdDays, ct) is { } error)
                return error;

            rule.Type = request.Type;
            rule.ThresholdDays = request.ThresholdDays;
            rule.Percentage = request.Percentage;
            await db.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
