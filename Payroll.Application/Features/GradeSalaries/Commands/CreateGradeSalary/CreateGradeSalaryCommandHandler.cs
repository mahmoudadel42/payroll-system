using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;
using Payroll.Domain.Entities;

namespace Payroll.Application.Features.GradeSalaries.Commands.CreateGradeSalary
{
    public sealed class CreateGradeSalaryCommandHandler(IApplicationDbContext db)
        : IRequestHandler<CreateGradeSalaryCommand, Result<int>>
    {
        public async Task<Result<int>> Handle(CreateGradeSalaryCommand request, CancellationToken ct)
        {
            if (await db.GradeSalaries.AnyAsync(g => g.Grade == request.Grade, ct))
                return Error.Conflict("gradeSalary.exists", "A salary is already defined for this grade. Edit it instead.");

            var gradeSalary = new GradeSalary { Grade = request.Grade, BaseSalary = request.BaseSalary };
            db.GradeSalaries.Add(gradeSalary);
            await db.SaveChangesAsync(ct);
            return gradeSalary.Id;
        }
    }
}
