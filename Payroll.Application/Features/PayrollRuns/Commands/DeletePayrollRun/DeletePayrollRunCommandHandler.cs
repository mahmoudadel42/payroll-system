using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.PayrollRuns.Model;
using Payroll.Domain.Enums;

namespace Payroll.Application.Features.PayrollRuns.Commands.DeletePayrollRun
{
    public sealed class DeletePayrollRunCommandHandler(IApplicationDbContext db)
        : IRequestHandler<DeletePayrollRunCommand, Result>
    {
        public async Task<Result> Handle(DeletePayrollRunCommand request, CancellationToken ct)
        {
            var run = await db.PayrollRuns.Include(r => r.Payslips).FirstOrDefaultAsync(r => r.Id == request.Id, ct);
            if (run is null)
                return PayrollRunErrors.NotFound(request.Id);

            if (run.Status != PayrollRunStatus.Draft)
                return PayrollRunErrors.Locked;

            db.Payslips.RemoveRange(run.Payslips);
            db.PayrollRuns.Remove(run);
            await db.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
