using Payroll.Domain.Enums;

namespace Payroll.Application.Features.PayrollRuns.Model
{
    public sealed record PayrollRunDto
    {
        public int Id { get; init; }
        public int Year { get; init; }
        public int Month { get; init; }
        public PayrollRunStatus Status { get; init; }
        public decimal TotalNet { get; init; }
        public int EmployeesCount { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? ApprovedAt { get; init; }
    }

    public sealed record PayrollRunDetailsDto(PayrollRunDto Run, IReadOnlyList<PayslipDto> Payslips);
}
