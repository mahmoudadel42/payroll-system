using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.Departments.Model;

namespace Payroll.Application.Features.Departments.Queries.GetDepartmentById
{
    public sealed class GetDepartmentByIdQueryHandler(IApplicationDbContext db)
        : IRequestHandler<GetDepartmentByIdQuery, Result<DepartmentDto>>
    {
        public async Task<Result<DepartmentDto>> Handle(GetDepartmentByIdQuery request, CancellationToken ct)
        {
            var department = await db.Departments.AsNoTracking()
                .Where(d => d.Id == request.Id)
                .Select(DepartmentProjection.ToDto)
                .FirstOrDefaultAsync(ct);

            return department is null ? Error.NotFound("department", request.Id) : department;
        }
    }
}
