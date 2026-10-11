using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payroll.Domain.Entities;

namespace Payroll.Infrastructure.Persistence.Configurations
{
    public class PayrollRunConfiguration : IEntityTypeConfiguration<PayrollRun>
    {
        public void Configure(EntityTypeBuilder<PayrollRun> b)
        {
            b.Property(x => x.Status).HasConversion<int>();
            b.Property(x => x.RowVersion).IsRowVersion();
            b.HasIndex(x => new { x.Year, x.Month }).IsUnique().HasFilter("[IsDeleted] = 0");
            b.ToTable(t => t.HasCheckConstraint("CK_PayrollRun_Month", "[Month] BETWEEN 1 AND 12"));
            b.Ignore(x => x.PeriodStart);
            b.Ignore(x => x.PeriodEnd);
        }
    }

}
