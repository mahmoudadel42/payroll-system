using Payroll.Domain.Common;

namespace Payroll.Domain.Entities
{
    public class Department : BaseEntity
    {
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        //(each department has a salary-based incentive at a specific rate)
        public decimal IncentivePercentage { get; set; }
        public ICollection<Employee> Employees { get; private set; } = [];
    }
}
