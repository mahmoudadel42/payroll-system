using MediatR;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.AbsenceRules.Model;
using Payroll.Domain.Entities;

namespace Payroll.Application.Features.AbsenceRules.Commands.CreateAbsenceRule
{
    public sealed class CreateAbsenceRuleCommandHandler(IApplicationDbContext db)
        : IRequestHandler<CreateAbsenceRuleCommand, Result<int>>
    {
        public async Task<Result<int>> Handle(CreateAbsenceRuleCommand request, CancellationToken ct)
        {
            if (await AbsenceRuleChecks.CheckConflictsAsync(db, null, request.Type, request.ThresholdDays, ct) is { } error)
                return error;

            var rule = new AbsenceRule
            {
                Type = request.Type,
                ThresholdDays = request.ThresholdDays,
                Percentage = request.Percentage
            };
            db.AbsenceRules.Add(rule);
            await db.SaveChangesAsync(ct);
            return rule.Id;
        }
    }
}
