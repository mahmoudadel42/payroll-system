using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payroll.Domain.Entities;

namespace Payroll.Infrastructure.Persistence.Configurations
{
    public class AbsenceRuleConfiguration : IEntityTypeConfiguration<AbsenceRule>
    {
        public void Configure(EntityTypeBuilder<AbsenceRule> b)
        {
            b.Property(x => x.Percentage).HasPrecision(5, 2);
            b.Property(x => x.Type).HasConversion<int>();
            b.HasIndex(x => new { x.Type, x.ThresholdDays }).IsUnique().HasFilter("[IsDeleted] = 0");
        }
    }
}
