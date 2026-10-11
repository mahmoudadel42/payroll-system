using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payroll.Domain.Entities;

namespace Payroll.Infrastructure.Persistence.Configurations
{
    public class GradeSalaryConfiguration : IEntityTypeConfiguration<GradeSalary>
    {
        public void Configure(EntityTypeBuilder<GradeSalary> b)
        {
            b.Property(x => x.Grade).HasConversion<int>();
            b.HasIndex(x => x.Grade).IsUnique().HasFilter("[IsDeleted] = 0");
            b.ToTable(t => t.HasCheckConstraint("CK_GradeSalary_NonNegative", "[BaseSalary] >= 0"));
        }
    }
}
