using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;

namespace Payroll.Application.Features.Departments.Commands.DeleteDepartment
{
    public sealed class DeleteDepartmentCommandHandler(IApplicationDbContext db)
        : IRequestHandler<DeleteDepartmentCommand, Result>
    {
        public async Task<Result> Handle(DeleteDepartmentCommand request, CancellationToken ct)
        {
            var department = await db.Departments.FirstOrDefaultAsync(d => d.Id == request.Id, ct);
            if (department is null)
                return Error.NotFound("department", request.Id);

            if (await db.Employees.AnyAsync(e => e.DepartmentId == request.Id, ct))
                return Error.Rule("department.hasEmployees", "Cannot delete a department that still has employees.");

            db.Departments.Remove(department);
            await db.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
