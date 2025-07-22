using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Peo.Core.Infra.Data.Repositories;
using Peo.Core.Interfaces.Data;
using Peo.Faturamento.Domain.Entities;
using Peo.Faturamento.Infra.Data.Contexts;

namespace Peo.Faturamento.Infra.Data.DiConfig
{
    public static class DependenciesSetup
    {
        public static IServiceCollection AddDataDependenciesForFaturamento(this IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            string connectionString;

            if (hostEnvironment.IsDevelopment())
            {
                connectionString = configuration.GetConnectionString("SQLiteConnection") ?? throw new InvalidOperationException("Não localizada connection string para ambiente de desenvolvimento (SQLite)");
            }
            else
            {
                connectionString = configuration.GetConnectionString("SqlServerConnection") ?? throw new InvalidOperationException("Não localizada connection string para ambiente de produção (SQL Server)");
            }

            services.AddDbContext<CobrancaContext>(options =>
            {
                if (hostEnvironment.IsDevelopment())
                {
                    options.UseSqlite(connectionString);
                }
                else
                {
                    options.UseSqlServer(connectionString);
                }

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

            // Repos
            services.AddScoped<IRepository<Pagamento>, GenericRepository<Pagamento, CobrancaContext>>();

            return services;
        }
    }
}