using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;

namespace Payroll.Application.Features.Departments.Commands.UpdateDepartmentIncentive
{
    public sealed class UpdateDepartmentIncentiveCommandHandler(IApplicationDbContext db)
        : IRequestHandler<UpdateDepartmentIncentiveCommand, Result>
    {
        public async Task<Result> Handle(UpdateDepartmentIncentiveCommand request, CancellationToken ct)
        {
            var department = await db.Departments.FirstOrDefaultAsync(d => d.Id == request.Id, ct);
            if (department is null)
                return Error.NotFound("department", request.Id);

            department.IncentivePercentage = request.IncentivePercentage;
            await db.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
