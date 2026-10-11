using System.Linq.Expressions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.Departments.Model;

namespace Payroll.Application.Features.Departments.Queries.GetDepartments
{
    public sealed class GetDepartmentsQueryHandler(IApplicationDbContext db)
        : IRequestHandler<GetDepartmentsQuery, PagedResult<DepartmentDto>>
    {
        public Task<PagedResult<DepartmentDto>> Handle(GetDepartmentsQuery request, CancellationToken ct)
        {
            var query = db.Departments.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();
                query = query.Where(d => d.NameAr.Contains(search) || d.NameEn.Contains(search));
            }

            return query
                .Select(DepartmentProjection.ToDto)
                .ToPagedResultAsync(request.Page, request.PageSize, ct);
        }
    }
}
