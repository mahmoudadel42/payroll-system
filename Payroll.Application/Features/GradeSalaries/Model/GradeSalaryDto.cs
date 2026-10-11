using Payroll.Domain.Enums;

namespace Payroll.Application.Features.GradeSalaries.Model
{
    public sealed record GradeSalaryDto(JobGrade Grade, decimal? BaseSalary, DateTime? UpdatedAt);
}
