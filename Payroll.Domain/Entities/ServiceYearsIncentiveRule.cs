using Payroll.Domain.Common;

namespace Payroll.Domain.Entities
{
    public class ServiceYearsIncentiveRule : BaseEntity
    {
        public int MinYears { get;  set; }
        public decimal Percentage { get;  set; }
    }
}
