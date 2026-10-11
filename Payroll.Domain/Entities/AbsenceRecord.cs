using Payroll.Domain.Common;

namespace Payroll.Domain.Entities
{
    public class AbsenceRecord : BaseEntity
    {
        public DateOnly Date { get;  set; }
        public string? Reason { get;  set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;
    }
}
