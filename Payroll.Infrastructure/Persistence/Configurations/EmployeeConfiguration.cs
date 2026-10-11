using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payroll.Domain.Entities;

namespace Payroll.Infrastructure.Persistence.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> b)
        {
            b.Property(x => x.FullName).HasMaxLength(150).IsRequired();
            b.Property(x => x.Address).HasMaxLength(250).IsRequired();
            b.Property(x => x.Phone).HasMaxLength(20).IsRequired();
            b.Property(x => x.Email).HasMaxLength(150).IsRequired();
            b.Property(x => x.Grade).HasConversion<int>();
            b.HasIndex(x => x.Email).IsUnique().HasFilter("[IsDeleted] = 0");
            b.HasIndex(x => x.FullName);
            b.HasOne(x => x.Department).WithMany(d => d.Employees).HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
