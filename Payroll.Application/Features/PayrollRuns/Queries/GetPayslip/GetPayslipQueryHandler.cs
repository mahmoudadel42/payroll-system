using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Localization;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.PayrollRuns.Model;

namespace Payroll.Application.Features.PayrollRuns.Queries.GetPayslip
{
    public sealed class GetPayslipQueryHandler(IApplicationDbContext db)
        : IRequestHandler<GetPayslipQuery, Result<PayslipDto>>
    {
        public async Task<Result<PayslipDto>> Handle(GetPayslipQuery request, CancellationToken ct)
        {
            var payslip = await db.Payslips.AsNoTracking()
                .Where(p => p.Id == request.Id)
                .Select(PayslipProjection.ToDto(CurrentCulture.IsArabic))
                .FirstOrDefaultAsync(ct);

            return payslip is null ? Error.NotFound("payslip", request.Id) : payslip;
        }
    }
}
