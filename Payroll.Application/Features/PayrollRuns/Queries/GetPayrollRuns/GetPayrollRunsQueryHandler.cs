using System.Linq.Expressions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.PayrollRuns.Model;

namespace Payroll.Application.Features.PayrollRuns.Queries.GetPayrollRuns
{
    public sealed class GetPayrollRunsQueryHandler(IApplicationDbContext db)
        : IRequestHandler<GetPayrollRunsQuery, PagedResult<PayrollRunDto>>
    {
        public Task<PagedResult<PayrollRunDto>> Handle(GetPayrollRunsQuery request, CancellationToken ct)
        {
            var query = db.PayrollRuns.AsNoTracking();

            if (request.Year is { } year)
                query = query.Where(r => r.Year == year);
            if (request.Status is { } status)
                query = query.Where(r => r.Status == status);

            var sortBy = request.SortBy ?? "period";
            var descending = request.SortBy is null || request.Descending;

            query = (sortBy.ToLowerInvariant(), descending) switch
            {
                ("totalnet", false) => query.OrderBy(r => r.TotalNet),
                ("totalnet", true) => query.OrderByDescending(r => r.TotalNet),
                (_, false) => query.OrderBy(r => r.Year).ThenBy(r => r.Month),
                (_, true) => query.OrderByDescending(r => r.Year).ThenByDescending(r => r.Month)
            };

            return query
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
                .ToPagedResultAsync(request.Page, request.PageSize, ct);
        }
    }
}
