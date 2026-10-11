using System.Linq.Expressions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Localization;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.Absences.Model;

namespace Payroll.Application.Features.Absences.Queries.GetAbsences
{
    public sealed class GetAbsencesQueryHandler(IApplicationDbContext db)
        : IRequestHandler<GetAbsencesQuery, PagedResult<AbsenceDto>>
    {
        public Task<PagedResult<AbsenceDto>> Handle(GetAbsencesQuery request, CancellationToken ct)
        {
            var query = db.AbsenceRecords.AsNoTracking();

            if (request.EmployeeId is { } employeeId)
                query = query.Where(a => a.EmployeeId == employeeId);
            if (request.DepartmentId is { } departmentId)
                query = query.Where(a => a.Employee.DepartmentId == departmentId);
            if (request.From is { } from)
                query = query.Where(a => a.Date >= from);
            if (request.To is { } to)
                query = query.Where(a => a.Date <= to);
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();
                query = query.Where(a => a.Employee.FullName.Contains(search) || (a.Reason != null && a.Reason.Contains(search)));
            }

            var arabic = CurrentCulture.IsArabic;
            return query
                .Select(a => new AbsenceDto
                {
                    Id = a.Id,
                    EmployeeId = a.EmployeeId,
                    EmployeeName = a.Employee.FullName,
                    DepartmentName = arabic ? a.Employee.Department.NameAr : a.Employee.Department.NameEn,
                    Date = a.Date,
                    Reason = a.Reason
                })
                .ToPagedResultAsync(request.Page, request.PageSize, ct);
        }
    }
}
