using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.PayrollRuns.Model;
using Payroll.Domain.Enums;

namespace Payroll.Application.Features.PayrollRuns.Commands.ApprovePayrollRun
{
    public sealed class ApprovePayrollRunCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, TimeProvider time)
        : IRequestHandler<ApprovePayrollRunCommand, Result>
    {
        public async Task<Result> Handle(ApprovePayrollRunCommand request, CancellationToken ct)
        {
            var run = await db.PayrollRuns.FirstOrDefaultAsync(r => r.Id == request.Id, ct);
            if (run is null)
                return PayrollRunErrors.NotFound(request.Id);

            if (run.Status != PayrollRunStatus.Draft)
                return PayrollRunErrors.Locked;

            if (!await db.Payslips.AnyAsync(p => p.PayrollRunId == run.Id, ct))
                return Error.Rule("payroll.empty", "Cannot approve a payroll run without payslips.");

            run.Status = PayrollRunStatus.Approved;
            run.ApprovedAt = time.GetUtcNow().UtcDateTime;
            run.ApprovedBy = currentUser.UserId;
            await db.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
