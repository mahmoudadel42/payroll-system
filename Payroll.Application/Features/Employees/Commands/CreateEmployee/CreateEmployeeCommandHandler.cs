using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;
using Payroll.Domain.Entities;

namespace Payroll.Application.Features.Employees.Commands.CreateEmployee
{
    public sealed class CreateEmployeeCommandHandler(IApplicationDbContext db)
        : IRequestHandler<CreateEmployeeCommand, Result<int>>
    {
        public async Task<Result<int>> Handle(CreateEmployeeCommand request, CancellationToken ct)
        {
            if (!await db.Departments.AnyAsync(d => d.Id == request.DepartmentId, ct))
                return Error.NotFound("department", request.DepartmentId);

            var email = request.Email.Trim().ToLowerInvariant();
            if (await db.Employees.AnyAsync(e => e.Email == email, ct))
                return Error.Conflict("employee.emailExists", "Another employee already uses this email.");

            var employee = new Employee
            {
                FullName = request.FullName.Trim(),
                BirthDate = request.BirthDate,
                Address = request.Address.Trim(),
                Phone = request.Phone.Trim(),
                Email = email,
                Grade = request.Grade,
                DepartmentId = request.DepartmentId,
                HireDate = request.HireDate
            };
            db.Employees.Add(employee);
            await db.SaveChangesAsync(ct);
            return employee.Id;
        }
    }
}
