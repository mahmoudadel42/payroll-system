using Microsoft.EntityFrameworkCore;
using Payroll.Domain.Entities;

namespace Payroll.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Department> Departments { get; set; }
        DbSet<Employee> Employees { get; set; }
        DbSet<GradeSalary> GradeSalaries { get; set; }
        DbSet<ServiceYearsIncentiveRule> ServiceYearsIncentiveRules { get; set; }
        DbSet<AbsenceRule> AbsenceRules { get; set; }
        DbSet<AbsenceRecord> AbsenceRecords { get; set; }
        DbSet<PayrollRun> PayrollRuns { get; set; }
        DbSet<Payslip> Payslips { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
