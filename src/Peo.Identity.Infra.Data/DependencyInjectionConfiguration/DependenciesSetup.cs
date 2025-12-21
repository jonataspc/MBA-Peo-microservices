using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Peo.Identity.Infra.Data.Contexts;

namespace Peo.Identity.Infra.Data.DependencyInjectionConfiguration
{
    public static class DependenciesSetup
    {
        public static IServiceCollection AddDataDependenciesForIdentity(this IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            string connectionString = configuration.GetConnectionString("identity-db") ?? throw new InvalidOperationException("Não localizada connection string");

            // Identity
            services.AddDbContext<IdentityContext>(options =>
            {
                options.UseSqlServer(connectionString);
                options.UseLazyLoadingProxies();

                if (hostEnvironment.IsDevelopment())
                {
                    options.EnableDetailedErrors();
                    options.EnableSensitiveDataLogging();
                }
            });

            if (hostEnvironment.IsDevelopment())
            {
                services.AddDatabaseDeveloperPageExceptionFilter();
            }

            return services;
        }
    }
}