using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;

namespace Payroll.Application.Features.AbsenceRules.Commands.DeleteAbsenceRule
{
    public sealed class DeleteAbsenceRuleCommandHandler(IApplicationDbContext db)
        : IRequestHandler<DeleteAbsenceRuleCommand, Result>
    {
        public async Task<Result> Handle(DeleteAbsenceRuleCommand request, CancellationToken ct)
        {
            var rule = await db.AbsenceRules.FirstOrDefaultAsync(r => r.Id == request.Id, ct);
            if (rule is null)
                return Error.NotFound("rule", request.Id);

            db.AbsenceRules.Remove(rule);
            await db.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
