using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Peo.Identity.Application.DependencyInjectionConfiguration
{
    public static class DependenciesSetup
    {
        public static IServiceCollection AddServicesForIdentity(this IServiceCollection services)
        {
            // Mediator
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            return services;
        }
    }
}