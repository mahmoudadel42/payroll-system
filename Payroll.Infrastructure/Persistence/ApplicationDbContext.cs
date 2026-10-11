using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Payroll.Application.Common.Interfaces;
using Payroll.Domain.Common;
using Payroll.Domain.Entities;
using Payroll.Infrastructure.Identity.Model;
using System.Linq.Expressions;

namespace Payroll.Infrastructure.Persistence
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options), IApplicationDbContext
    {
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<GradeSalary> GradeSalaries { get; set; }
        public DbSet<ServiceYearsIncentiveRule> ServiceYearsIncentiveRules { get; set; }
        public DbSet<AbsenceRule> AbsenceRules { get; set; }
        public DbSet<AbsenceRecord> AbsenceRecords { get; set; }
        public DbSet<PayrollRun> PayrollRuns { get; set; }
        public DbSet<Payslip> Payslips { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            foreach (var entityType in builder.Model.GetEntityTypes()
                         .Where(t => typeof(BaseEntity).IsAssignableFrom(t.ClrType)))
            {
                var p = Expression.Parameter(entityType.ClrType, "e");
                var filter = Expression.Lambda(
                    Expression.Not(Expression.Property(p, nameof(BaseEntity.IsDeleted))), p);
                builder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }

            // Overrides the generic soft-delete filter: an unnamed filter replaces the previous one.
            builder.Entity<AbsenceRecord>().HasQueryFilter(x => !x.IsDeleted && !x.Employee.IsDeleted);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
       // Payslips are snapshots that must stay visible after their employee is soft-deleted.
       optionsBuilder.ConfigureWarnings(w =>
           w.Ignore(CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning));
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
            configurationBuilder.Properties<string>().HaveMaxLength(256);
        }
    }
}
