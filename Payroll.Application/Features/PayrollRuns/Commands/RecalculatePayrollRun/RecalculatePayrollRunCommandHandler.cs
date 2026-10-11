using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.PayrollRuns.Model;
using Payroll.Domain.Enums;

namespace Payroll.Application.Features.PayrollRuns.Commands.RecalculatePayrollRun
{
    public sealed class RecalculatePayrollRunCommandHandler(IApplicationDbContext db, IPayrollDataService payrollData)
        : IRequestHandler<RecalculatePayrollRunCommand, Result>
    {
        public async Task<Result> Handle(RecalculatePayrollRunCommand request, CancellationToken ct)
        {
            var run = await db.PayrollRuns.Include(r => r.Payslips).FirstOrDefaultAsync(r => r.Id == request.Id, ct);
            if (run is null)
                return PayrollRunErrors.NotFound(request.Id);

            if (run.Status != PayrollRunStatus.Draft)
                return PayrollRunErrors.Locked;

            var payslips = await payrollData.BuildPayslipsAsync(run.Year, run.Month, null, null, ct);
            if (!payslips.IsSuccess)
                return payslips.Error!;

            var existing = run.Payslips.ToDictionary(p => p.EmployeeId);
            var calculated = payslips.Value!;

            foreach (var payslip in calculated)
            {
                if (existing.Remove(payslip.EmployeeId, out var current))
                    current.CopyCalculationFrom(payslip);
                else
                    run.Payslips.Add(payslip);
            }

            db.Payslips.RemoveRange(existing.Values);
            run.TotalNet = calculated.Sum(p => p.NetSalary);

            await db.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
