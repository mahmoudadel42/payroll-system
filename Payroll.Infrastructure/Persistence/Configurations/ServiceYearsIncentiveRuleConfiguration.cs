using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payroll.Domain.Entities;

namespace Payroll.Infrastructure.Persistence.Configurations
{
    public class ServiceYearsIncentiveRuleConfiguration : IEntityTypeConfiguration<ServiceYearsIncentiveRule>
    {
        public void Configure(EntityTypeBuilder<ServiceYearsIncentiveRule> b)
        {
            b.Property(x => x.Percentage).HasPrecision(5, 2);
            b.HasIndex(x => x.MinYears).IsUnique().HasFilter("[IsDeleted] = 0");
        }
    }
}
