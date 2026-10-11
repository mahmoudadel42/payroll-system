using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Payroll.Application.Common.Behaviors;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Features.PayrollRuns.Services;

namespace Payroll.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);
            services.AddScoped<IPayrollDataService, PayrollDataService>();
            return services;
        }
    }

}
