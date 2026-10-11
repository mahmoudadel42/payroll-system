using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Features.GradeSalaries.Model;
using Payroll.Domain.Enums;

namespace Payroll.Application.Features.GradeSalaries.Queries.GetGradeSalaries
{
    public sealed class GetGradeSalariesQueryHandler(IApplicationDbContext db)
        : IRequestHandler<GetGradeSalariesQuery, IReadOnlyList<GradeSalaryDto>>
    {
        public async Task<IReadOnlyList<GradeSalaryDto>> Handle(GetGradeSalariesQuery request, CancellationToken ct)
        {
            var salaries = await db.GradeSalaries.AsNoTracking().ToDictionaryAsync(g => g.Grade, ct);

            return Enum.GetValues<JobGrade>()
                .Select(grade => salaries.TryGetValue(grade, out var s)
                    ? new GradeSalaryDto(grade, s.BaseSalary, s.UpdatedAt ?? s.CreatedAt)
                    : new GradeSalaryDto(grade, null, null))
                .ToList();
        }
    }
}
