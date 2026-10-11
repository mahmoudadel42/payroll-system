using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;

namespace Payroll.Application.Features.Employees.Commands.UpdateEmployee
{
    public sealed class UpdateEmployeeCommandHandler(IApplicationDbContext db)
        : IRequestHandler<UpdateEmployeeCommand, Result>
    {
        public async Task<Result> Handle(UpdateEmployeeCommand request, CancellationToken ct)
        {
            var employee = await db.Employees.FirstOrDefaultAsync(e => e.Id == request.Id, ct);
            if (employee is null)
                return Error.NotFound("employee", request.Id);

            if (!await db.Departments.AnyAsync(d => d.Id == request.DepartmentId, ct))
                return Error.NotFound("department", request.DepartmentId);

            var email = request.Email.Trim().ToLowerInvariant();
            if (await db.Employees.AnyAsync(e => e.Id != request.Id && e.Email == email, ct))
                return Error.Conflict("employee.emailExists", "Another employee already uses this email.");

            employee.FullName = request.FullName.Trim();
            employee.BirthDate = request.BirthDate;
            employee.Address = request.Address.Trim();
            employee.Phone = request.Phone.Trim();
            employee.Email = email;
            employee.Grade = request.Grade;
            employee.DepartmentId = request.DepartmentId;
            employee.HireDate = request.HireDate;
            await db.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
