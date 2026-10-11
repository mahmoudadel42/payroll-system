using System.Linq.Expressions;
using Payroll.Domain.Entities;

namespace Payroll.Application.Features.PayrollRuns.Model
{
    public static class PayslipProjection
    {
        public static Expression<Func<Payslip, PayslipDto>> ToDto(bool arabic) => p => new PayslipDto
        {
            Id = p.Id,
            PayrollRunId = p.PayrollRunId,
            EmployeeId = p.EmployeeId,
            EmployeeName = p.EmployeeName,
            DepartmentId = p.DepartmentId,
            DepartmentName = arabic ? p.DepartmentNameAr : p.DepartmentNameEn,
            Grade = p.Grade,
            ServiceYears = p.ServiceYears,
            AbsenceDays = p.AbsenceDays,
            BaseSalary = p.BaseSalary,
            DepartmentIncentivePercentage = p.DepartmentIncentivePercentage,
            DepartmentIncentive = p.DepartmentIncentive,
            ServiceYearsIncentivePercentage = p.ServiceYearsIncentivePercentage,
            ServiceYearsIncentive = p.ServiceYearsIncentive,
            AbsenceBonusPercentage = p.AbsenceBonusPercentage,
            AbsenceBonus = p.AbsenceBonus,
            AbsenceDeductionPercentage = p.AbsenceDeductionPercentage,
            AbsenceDeduction = p.AbsenceDeduction,
            NetSalary = p.NetSalary
        };
    }
}
