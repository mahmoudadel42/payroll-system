using Payroll.Domain.Common;
using Payroll.Domain.Enums;

namespace Payroll.Domain.Entities
{
    public class Payslip : BaseEntity
    {
        public int PayrollRunId { get; set; }
        public PayrollRun PayrollRun { get; set; } = null!;
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        // Snapshot of the employee data at calculation time
        public string EmployeeName { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string DepartmentNameAr { get; set; } = string.Empty;
        public string DepartmentNameEn { get; set; } = string.Empty;
        public JobGrade Grade { get; set; }
        public int ServiceYears { get; set; }
        public int AbsenceDays { get; set; }

        public decimal BaseSalary { get; set; }
        public decimal DepartmentIncentivePercentage { get; set; }
        public decimal DepartmentIncentive { get; set; }
        public decimal ServiceYearsIncentivePercentage { get; set; }
        public decimal ServiceYearsIncentive { get; set; }
        public decimal AbsenceBonusPercentage { get; set; }
        public decimal AbsenceBonus { get; set; }
        public decimal AbsenceDeductionPercentage { get; set; }
        public decimal AbsenceDeduction { get; set; }
        public decimal NetSalary { get; set; }

        public void CopyCalculationFrom(Payslip source)
        {
            EmployeeName = source.EmployeeName;
            DepartmentId = source.DepartmentId;
            DepartmentNameAr = source.DepartmentNameAr;
            DepartmentNameEn = source.DepartmentNameEn;
            Grade = source.Grade;
            ServiceYears = source.ServiceYears;
            AbsenceDays = source.AbsenceDays;
            BaseSalary = source.BaseSalary;
            DepartmentIncentivePercentage = source.DepartmentIncentivePercentage;
            DepartmentIncentive = source.DepartmentIncentive;
            ServiceYearsIncentivePercentage = source.ServiceYearsIncentivePercentage;
            ServiceYearsIncentive = source.ServiceYearsIncentive;
            AbsenceBonusPercentage = source.AbsenceBonusPercentage;
            AbsenceBonus = source.AbsenceBonus;
            AbsenceDeductionPercentage = source.AbsenceDeductionPercentage;
            AbsenceDeduction = source.AbsenceDeduction;
            NetSalary = source.NetSalary;
        }
    }
}
