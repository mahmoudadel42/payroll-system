using Payroll.Domain.Common;
using Payroll.Domain.Enums;

namespace Payroll.Domain.Entities
{
    public class AbsenceRule : BaseEntity
    {
        // Deduction: applies when absence days > ThresholdDays. Bonus: applies when absence days <= ThresholdDays.
        public int ThresholdDays { get;  set; }
        public decimal Percentage { get;  set; }
        public AbsenceRuleType Type { get;  set; }
    }
}
