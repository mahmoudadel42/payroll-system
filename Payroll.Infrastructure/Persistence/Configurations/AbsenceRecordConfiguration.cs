using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payroll.Domain.Entities;

namespace Payroll.Infrastructure.Persistence.Configurations
{
    public class AbsenceRecordConfiguration : IEntityTypeConfiguration<AbsenceRecord>
    {
        public void Configure(EntityTypeBuilder<AbsenceRecord> b)
        {
            b.Property(x => x.Reason).HasMaxLength(250);
            b.HasIndex(x => new { x.EmployeeId, x.Date }).IsUnique().HasFilter("[IsDeleted] = 0");
            b.HasIndex(x => x.Date);
            b.HasOne(x => x.Employee).WithMany(e => e.Absences).HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
