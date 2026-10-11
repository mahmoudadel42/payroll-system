using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Localization;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.Employees.Model;

namespace Payroll.Application.Features.Employees.Queries.GetEmployees
{
    public sealed class GetEmployeesQueryHandler(IApplicationDbContext db)
        : IRequestHandler<GetEmployeesQuery, PagedResult<EmployeeDto>>
    {
        public Task<PagedResult<EmployeeDto>> Handle(GetEmployeesQuery request, CancellationToken ct)
        {
            var query = db.Employees.AsNoTracking();

            if (request.DepartmentId is { } departmentId)
                query = query.Where(e => e.DepartmentId == departmentId);

            if (request.Grade is { } grade)
                query = query.Where(e => e.Grade == grade);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();
                query = query.Where(e => e.FullName.Contains(search) || e.Email.Contains(search) || e.Phone.Contains(search));
            }

            return query
                .Select(EmployeeProjection.ToDto(CurrentCulture.IsArabic))
                .ToPagedResultAsync(request.Page, request.PageSize, ct);
        }
    }
}
