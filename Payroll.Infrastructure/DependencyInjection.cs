using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payroll.Application.Common.Interfaces;
using Payroll.Infrastructure.Identity.Model;
using Payroll.Infrastructure.Identity.Service;
using Payroll.Infrastructure.Persistence;
using Payroll.Infrastructure.Persistence.Interceptors;
using Payroll.Infrastructure.Reports;

namespace Payroll.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");

            services.AddSingleton(TimeProvider.System);
            services.AddScoped<AuditableEntityInterceptor>();
            services.AddDbContext<ApplicationDbContext>((sp, options) => options
                .UseSqlServer(connectionString, sql =>
                {
                    sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    sql.EnableRetryOnFailure(3);
                })
                .AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>()));
            services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

            services.AddOptions<JwtOptions>()
                .Bind(configuration.GetSection(JwtOptions.Section))
                .Validate(o => o.Key.Length >= 32, "Jwt:Key must be at least 32 characters.")
                .Validate(o => !string.IsNullOrWhiteSpace(o.Issuer) && !string.IsNullOrWhiteSpace(o.Audience),
                    "Jwt:Issuer and Jwt:Audience are required.")
                .ValidateOnStart();
            services.AddOptions<SeedOptions>().Bind(configuration.GetSection(SeedOptions.Section));

            services.AddIdentityCore<ApplicationUser>(o =>
            {
                o.Password.RequiredLength = 8;
                o.Password.RequireNonAlphanumeric = false;
                o.Lockout.MaxFailedAccessAttempts = 5;
                o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                o.User.RequireUniqueEmail = false;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddScoped<IIdentityService, IdentityService>();
            services.AddSingleton<IReportExporter, ExcelReportExporter>();
            return services;
        }
    }
}
