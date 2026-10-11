using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;

namespace Payroll.Application.Features.Departments.Commands.UpdateDepartment
{
    public sealed class UpdateDepartmentCommandHandler(IApplicationDbContext db)
        : IRequestHandler<UpdateDepartmentCommand, Result>
    {
        public async Task<Result> Handle(UpdateDepartmentCommand request, CancellationToken ct)
        {
            var department = await db.Departments.FirstOrDefaultAsync(d => d.Id == request.Id, ct);
            if (department is null)
                return Error.NotFound("department", request.Id);

            var nameAr = request.NameAr.Trim();
            var nameEn = request.NameEn.Trim();

            if (await db.Departments.AnyAsync(d => d.Id != request.Id && (d.NameAr == nameAr || d.NameEn == nameEn), ct))
                return Error.Conflict("department.nameExists", "A department with the same name already exists.");

            department.NameAr = nameAr;
            department.NameEn = nameEn;
            department.IncentivePercentage = request.IncentivePercentage;
            await db.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
