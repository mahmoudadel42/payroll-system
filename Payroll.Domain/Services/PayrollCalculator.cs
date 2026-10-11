using Payroll.Domain.Entities;
using Payroll.Domain.Enums;

namespace Payroll.Domain.Services
{
    public sealed record PayslipInput(
        decimal BaseSalary,
        decimal DepartmentPercentage,
        DateOnly HireDate,
        int AbsenceDays,
        DateOnly PeriodEnd,
        IReadOnlyList<ServiceYearsIncentiveRule> ServiceRules,
        IReadOnlyList<AbsenceRule> AbsenceRules);

    public sealed record PayslipBreakdown(
        int ServiceYears,
        decimal DepartmentPercentage,
        decimal DepartmentIncentive,
        decimal ServicePercentage,
        decimal ServiceIncentive,
        decimal BonusPercentage,
        decimal Bonus,
        decimal DeductionPercentage,
        decimal Deduction,
        decimal Net);

    public static class PayrollCalculator
    {
        public static PayslipBreakdown Calculate(PayslipInput input)
        {
            var baseSalary = input.BaseSalary;

            var departmentIncentive = Percent(baseSalary, input.DepartmentPercentage);

            var serviceRule = input.ServiceRules
                .Where(r => input.HireDate.AddYears(r.MinYears) < input.PeriodEnd)
                .MaxBy(r => r.MinYears);
            var servicePercentage = serviceRule?.Percentage ?? 0;
            var serviceIncentive = Percent(baseSalary, servicePercentage);

            var bonusRule = input.AbsenceRules
                .FirstOrDefault(r => r.Type == AbsenceRuleType.Bonus && input.AbsenceDays <= r.ThresholdDays);
            var bonusPercentage = bonusRule?.Percentage ?? 0;
            var bonus = Percent(baseSalary, bonusPercentage);

            var deductionRule = input.AbsenceRules
                .Where(r => r.Type == AbsenceRuleType.Deduction && input.AbsenceDays > r.ThresholdDays)
                .MaxBy(r => r.ThresholdDays);
            var deductionPercentage = deductionRule?.Percentage ?? 0;
            var deduction = Percent(baseSalary, deductionPercentage);

            var net = Round(baseSalary + departmentIncentive + serviceIncentive + bonus - deduction);

            return new PayslipBreakdown(
                CompletedYears(input.HireDate, input.PeriodEnd),
                input.DepartmentPercentage, departmentIncentive,
                servicePercentage, serviceIncentive,
                bonusPercentage, bonus,
                deductionPercentage, deduction,
                net);
        }

        public static int CompletedYears(DateOnly from, DateOnly to)
        {
            var years = to.Year - from.Year;
            if (from.AddYears(years) > to) years--;
            return Math.Max(0, years);
        }

        private static decimal Percent(decimal amount, decimal percentage) => Round(amount * percentage / 100m);

        private static decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}
