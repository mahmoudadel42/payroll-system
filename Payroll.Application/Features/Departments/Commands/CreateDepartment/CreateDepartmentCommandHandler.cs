using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;
using Payroll.Domain.Entities;

namespace Payroll.Application.Features.Departments.Commands.CreateDepartment
{
    public sealed class CreateDepartmentCommandHandler(IApplicationDbContext db)
        : IRequestHandler<CreateDepartmentCommand, Result<int>>
    {
        public async Task<Result<int>> Handle(CreateDepartmentCommand request, CancellationToken ct)
        {
            var nameAr = request.NameAr.Trim();
            var nameEn = request.NameEn.Trim();

            if (await db.Departments.AnyAsync(d => d.NameAr == nameAr || d.NameEn == nameEn, ct))
                return Error.Conflict("department.nameExists", "A department with the same name already exists.");

            var department = new Department
            {
                NameAr = nameAr,
                NameEn = nameEn,
                IncentivePercentage = request.IncentivePercentage
            };
            db.Departments.Add(department);
            await db.SaveChangesAsync(ct);
            return department.Id;
        }
    }
}
