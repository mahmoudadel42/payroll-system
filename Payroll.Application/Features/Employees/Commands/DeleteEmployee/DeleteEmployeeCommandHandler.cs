using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;

namespace Payroll.Application.Features.Employees.Commands.DeleteEmployee
{
    public sealed class DeleteEmployeeCommandHandler(IApplicationDbContext db)
        : IRequestHandler<DeleteEmployeeCommand, Result>
    {
        public async Task<Result> Handle(DeleteEmployeeCommand request, CancellationToken ct)
        {
            var employee = await db.Employees.FirstOrDefaultAsync(e => e.Id == request.Id, ct);
            if (employee is null)
                return Error.NotFound("employee", request.Id);

            db.Employees.Remove(employee);
            await db.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
