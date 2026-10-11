using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payroll.Domain.Entities;

namespace Payroll.Infrastructure.Persistence.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> b)
        {
            b.Property(x => x.NameAr).HasMaxLength(100).IsRequired();
            b.Property(x => x.NameEn).HasMaxLength(100).IsRequired();
            b.Property(x => x.IncentivePercentage).HasPrecision(5, 2);
            b.HasIndex(x => x.NameAr).IsUnique().HasFilter("[IsDeleted] = 0");
            b.HasIndex(x => x.NameEn).IsUnique().HasFilter("[IsDeleted] = 0");
            b.ToTable(t => t.HasCheckConstraint("CK_Department_Incentive", "[IncentivePercentage] BETWEEN 0 AND 100"));
        }
    }
}