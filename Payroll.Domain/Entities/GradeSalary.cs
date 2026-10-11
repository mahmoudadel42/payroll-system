using Payroll.Domain.Common;
using Payroll.Domain.Enums;

namespace Payroll.Domain.Entities
{
    public class GradeSalary : BaseEntity
    {
        public JobGrade Grade { get;  set; }
        public decimal BaseSalary { get;  set; }
    }
}
