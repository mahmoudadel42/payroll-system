using Payroll.Domain.Common;
using Payroll.Domain.Enums;

namespace Payroll.Domain.Entities
{
    public class PayrollRun : BaseEntity
    {
        public int Year { get;  set; }
        public int Month { get;  set; }
        public PayrollRunStatus Status { get;  set; }
        public DateTime? ApprovedAt { get;  set; }
        public Guid? ApprovedBy { get;  set; }
        public decimal TotalNet { get;  set; }
        public byte[] RowVersion { get; set; } = [];
        public ICollection<Payslip> Payslips { get; private set; } = [];
        public DateOnly PeriodStart => new(Year, Month, 1);
        public DateOnly PeriodEnd => PeriodStart.AddMonths(1).AddDays(-1);
    }
}
