using Payroll.Domain.Common;
using Payroll.Domain.Enums;

namespace Payroll.Domain.Entities
{
    public class Employee : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public DateOnly BirthDate { get; set; }
        public string Address { get;  set; } = string.Empty;
        public string Phone { get;  set; } = string.Empty;
        public string Email { get;  set; } = string.Empty;
        public JobGrade Grade { get;  set; }
        public DateOnly HireDate { get; set; }
        public int DepartmentId { get;  set; }
        public Department Department { get;  set; } = null!;
        public ICollection<AbsenceRecord> Absences { get; private set; } = [];
    }
}
