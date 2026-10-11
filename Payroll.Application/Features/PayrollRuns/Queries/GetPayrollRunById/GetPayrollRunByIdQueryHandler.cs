using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Localization;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.PayrollRuns.Model;

namespace Payroll.Application.Features.PayrollRuns.Queries.GetPayrollRunById
{
    public sealed class GetPayrollRunByIdQueryHandler(IApplicationDbContext db)
        : IRequestHandler<GetPayrollRunByIdQuery, Result<PayrollRunDetailsDto>>
    {
        public async Task<Result<PayrollRunDetailsDto>> Handle(GetPayrollRunByIdQuery request, CancellationToken ct)
        {
            var run = await db.PayrollRuns.AsNoTracking()
                .Where(r => r.Id == request.Id)
                .Select(r => new PayrollRunDto
                {
                    Id = r.Id,
                    Year = r.Year,
                    Month = r.Month,
                    Status = r.Status,
                    TotalNet = r.TotalNet,
                    EmployeesCount = r.Payslips.Count,
                    CreatedAt = r.CreatedAt,
                    ApprovedAt = r.ApprovedAt
                })
                .FirstOrDefaultAsync(ct);
            if (run is null)
                return PayrollRunErrors.NotFound(request.Id);

            var payslips = await db.Payslips.AsNoTracking()
                .Where(p => p.PayrollRunId == request.Id)
                .OrderBy(p => p.EmployeeName)
                .Select(PayslipProjection.ToDto(CurrentCulture.IsArabic))
                .ToListAsync(ct);

            return new PayrollRunDetailsDto(run, payslips);
        }
    }
}
