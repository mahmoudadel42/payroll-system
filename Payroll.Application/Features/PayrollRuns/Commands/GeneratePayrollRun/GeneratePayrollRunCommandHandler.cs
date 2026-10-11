using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;
using Payroll.Domain.Entities;
using Payroll.Domain.Enums;

namespace Payroll.Application.Features.PayrollRuns.Commands.GeneratePayrollRun
{
    public sealed class GeneratePayrollRunCommandHandler(
        IApplicationDbContext db, IPayrollDataService payrollData, TimeProvider time)
        : IRequestHandler<GeneratePayrollRunCommand, Result<int>>
    {
        public async Task<Result<int>> Handle(GeneratePayrollRunCommand request, CancellationToken ct)
        {
            var today = DateOnly.FromDateTime(time.GetLocalNow().DateTime);
            if (new DateOnly(request.Year, request.Month, 1) > today)
                return Error.Rule("payroll.futureMonth", "Payroll cannot be generated for a future month.");

            if (await db.PayrollRuns.AnyAsync(r => r.Year == request.Year && r.Month == request.Month, ct))
                return Error.Conflict("payroll.exists", "A payroll run already exists for this month.");

            var payslips = await payrollData.BuildPayslipsAsync(request.Year, request.Month, null, null, ct);
            if (!payslips.IsSuccess)
                return payslips.Error!;

            var run = new PayrollRun
            {
                Year = request.Year,
                Month = request.Month,
                Status = PayrollRunStatus.Draft,
                TotalNet = payslips.Value!.Sum(p => p.NetSalary)
            };
            foreach (var payslip in payslips.Value!)
                run.Payslips.Add(payslip);

            db.PayrollRuns.Add(run);
            await db.SaveChangesAsync(ct);
            return run.Id;
        }
    }
}
