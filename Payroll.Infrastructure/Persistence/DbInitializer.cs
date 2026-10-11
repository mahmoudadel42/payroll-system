using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Payroll.Application.Common.Security;
using Payroll.Domain.Entities;
using Payroll.Domain.Enums;
using Payroll.Infrastructure.Identity.Model;

namespace Payroll.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider services, CancellationToken ct = default)
        {
            using var scope = services.CreateScope();
            var sp = scope.ServiceProvider;
            var db = sp.GetRequiredService<ApplicationDbContext>();
            var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(DbInitializer));
            var seed = sp.GetRequiredService<IOptions<SeedOptions>>().Value;

            await db.Database.MigrateAsync(ct);

            await SeedIdentityAsync(sp, seed, logger);
            await SeedReferenceDataAsync(db, ct);
            if (seed.SampleData) await SeedSampleDataAsync(db, ct);

            logger.LogInformation("Database initialized");
        }

        private static async Task SeedIdentityAsync(IServiceProvider sp, SeedOptions seed, ILogger logger)
        {
            var roles = sp.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (var role in new[] { Roles.Admin, Roles.HR })
                if (!await roles.RoleExistsAsync(role))
                    await roles.CreateAsync(new IdentityRole(role));

            var users = sp.GetRequiredService<UserManager<ApplicationUser>>();
            await EnsureUserAsync(users, seed.AdminUserName, seed.AdminPassword, "System Administrator", Roles.Admin, logger);
            await EnsureUserAsync(users, seed.HrUserName, seed.HrPassword, "HR Officer", Roles.HR, logger);
        }

        private static async Task EnsureUserAsync(
            UserManager<ApplicationUser> users, string userName, string password, string displayName, string role, ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password)) return;
            if (await users.FindByNameAsync(userName) is not null) return;

            var user = new ApplicationUser { UserName = userName, DisplayName = displayName, EmailConfirmed = true };
            var result = await users.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                logger.LogError("Failed to seed user {User}: {Errors}", userName,
                    string.Join("; ", result.Errors.Select(e => e.Description)));
                return;
            }

            await users.AddToRoleAsync(user, role);
        }

        private static async Task SeedReferenceDataAsync(ApplicationDbContext db, CancellationToken ct)
        {
            if (!await db.GradeSalaries.AnyAsync(ct))
            {
                db.GradeSalaries.AddRange(
                    new GradeSalary { Grade = JobGrade.First, BaseSalary = 15000 },
                    new GradeSalary { Grade = JobGrade.Second, BaseSalary = 10000 },
                    new GradeSalary { Grade = JobGrade.Third, BaseSalary = 7000 });
            }

            if (!await db.ServiceYearsIncentiveRules.AnyAsync(ct))
            {
                db.ServiceYearsIncentiveRules.AddRange(
                    new ServiceYearsIncentiveRule { MinYears = 5, Percentage = 5 },
                    new ServiceYearsIncentiveRule { MinYears = 7, Percentage = 10 });
            }

            if (!await db.AbsenceRules.AnyAsync(ct))
            {
                db.AbsenceRules.AddRange(
                    new AbsenceRule { ThresholdDays = 1, Percentage = 5, Type = AbsenceRuleType.Bonus },
                    new AbsenceRule { ThresholdDays = 2, Percentage = 5, Type = AbsenceRuleType.Deduction },
                    new AbsenceRule { ThresholdDays = 5, Percentage = 10, Type = AbsenceRuleType.Deduction },
                    new AbsenceRule { ThresholdDays = 10, Percentage = 20, Type = AbsenceRuleType.Deduction },
                    new AbsenceRule { ThresholdDays = 15, Percentage = 30, Type = AbsenceRuleType.Deduction });
            }

            await db.SaveChangesAsync(ct);
        }

        private static async Task SeedSampleDataAsync(ApplicationDbContext db, CancellationToken ct)
        {
            if (await db.Departments.IgnoreQueryFilters().AnyAsync(ct)) return;

            var it = new Department { NameAr = "تكنولوجيا المعلومات", NameEn = "Information Technology", IncentivePercentage = 10 };
            var hr = new Department { NameAr = "الموارد البشرية", NameEn = "Human Resources", IncentivePercentage = 5 };
            var finance = new Department { NameAr = "المالية", NameEn = "Finance", IncentivePercentage = 7.5m };
            db.Departments.AddRange(it, hr, finance);
            await db.SaveChangesAsync(ct);

            db.Employees.AddRange(
                new Employee
                {
                    FullName = "أحمد محمد علي",
                    BirthDate = new DateOnly(1985, 3, 12),
                    Address = "القاهرة - مدينة نصر",
                    Phone = "01001234567",
                    Email = "ahmed.ali@payroll.local",
                    Grade = JobGrade.First,
                    DepartmentId = it.Id,
                    HireDate = new DateOnly(2015, 1, 1)
                },
                new Employee
                {
                    FullName = "Sara Hassan",
                    BirthDate = new DateOnly(1992, 7, 5),
                    Address = "Giza - Dokki",
                    Phone = "01112345678",
                    Email = "sara.hassan@payroll.local",
                    Grade = JobGrade.Second,
                    DepartmentId = hr.Id,
                    HireDate = new DateOnly(2020, 6, 15)
                },
                new Employee
                {
                    FullName = "محمود إبراهيم",
                    BirthDate = new DateOnly(1995, 11, 20),
                    Address = "الإسكندرية - سموحة",
                    Phone = "01223456789",
                    Email = "mahmoud.ibrahim@payroll.local",
                    Grade = JobGrade.Third,
                    DepartmentId = finance.Id,
                    HireDate = new DateOnly(2018, 9, 1)
                },
                new Employee
                {
                    FullName = "Mona Adel",
                    BirthDate = new DateOnly(1990, 1, 30),
                    Address = "Cairo - Maadi",
                    Phone = "01534567890",
                    Email = "mona.adel@payroll.local",
                    Grade = JobGrade.Second,
                    DepartmentId = it.Id,
                    HireDate = new DateOnly(2012, 4, 10)
                }
 );
            await db.SaveChangesAsync(ct);
        }
    }
}
