using Payroll.Domain.Enums;

namespace Payroll.Application.Features.PayrollRuns.Model
{
    public sealed record PayslipDto
    {
        public int Id { get; init; }
        public int PayrollRunId { get; init; }
        public int EmployeeId { get; init; }
        public string EmployeeName { get; init; } = string.Empty;
        public int DepartmentId { get; init; }
        public string DepartmentName { get; init; } = string.Empty;
        public JobGrade Grade { get; init; }
        public int ServiceYears { get; init; }
        public int AbsenceDays { get; init; }
        public decimal BaseSalary { get; init; }
        public decimal DepartmentIncentivePercentage { get; init; }
        public decimal DepartmentIncentive { get; init; }
        public decimal ServiceYearsIncentivePercentage { get; init; }
        public decimal ServiceYearsIncentive { get; init; }
        public decimal AbsenceBonusPercentage { get; init; }
        public decimal AbsenceBonus { get; init; }
        public decimal AbsenceDeductionPercentage { get; init; }
        public decimal AbsenceDeduction { get; init; }
        public decimal NetSalary { get; init; }
    }
}
