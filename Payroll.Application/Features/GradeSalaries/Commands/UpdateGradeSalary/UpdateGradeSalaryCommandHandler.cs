using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;

namespace Payroll.Application.Features.GradeSalaries.Commands.UpdateGradeSalary
{
    public sealed class UpdateGradeSalaryCommandHandler(IApplicationDbContext db)
        : IRequestHandler<UpdateGradeSalaryCommand, Result>
    {
        public async Task<Result> Handle(UpdateGradeSalaryCommand request, CancellationToken ct)
        {
            var gradeSalary = await db.GradeSalaries.FirstOrDefaultAsync(g => g.Grade == request.Grade, ct);
            if (gradeSalary is null)
                return Error.NotFound("gradeSalary", request.Grade);

            gradeSalary.BaseSalary = request.BaseSalary;
            await db.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
