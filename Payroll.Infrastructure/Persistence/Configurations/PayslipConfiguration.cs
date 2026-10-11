using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payroll.Domain.Entities;

namespace Payroll.Infrastructure.Persistence.Configurations
{
    public class PayslipConfiguration : IEntityTypeConfiguration<Payslip>
    {
        public void Configure(EntityTypeBuilder<Payslip> b)
        {
            b.Property(x => x.EmployeeName).HasMaxLength(150).IsRequired();
            b.Property(x => x.DepartmentNameAr).HasMaxLength(100).IsRequired();
            b.Property(x => x.DepartmentNameEn).HasMaxLength(100).IsRequired();
            b.Property(x => x.Grade).HasConversion<int>();
            b.Property(x => x.DepartmentIncentivePercentage).HasPrecision(5, 2);
            b.Property(x => x.ServiceYearsIncentivePercentage).HasPrecision(5, 2);
            b.Property(x => x.AbsenceBonusPercentage).HasPrecision(5, 2);
            b.Property(x => x.AbsenceDeductionPercentage).HasPrecision(5, 2);
            b.HasIndex(x => new { x.PayrollRunId, x.EmployeeId }).IsUnique().HasFilter("[IsDeleted] = 0");
            b.HasIndex(x => new { x.PayrollRunId, x.DepartmentId });
            b.HasOne(x => x.PayrollRun).WithMany(r => r.Payslips).HasForeignKey(x => x.PayrollRunId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
