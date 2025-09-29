using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Peo.GestaoAlunos.Domain.Repositories;
using Peo.GestaoAlunos.Infra.Data.Contexts;
using Peo.GestaoAlunos.Infra.Data.Repositories;

namespace Peo.GestaoAlunos.Infra.Data.DependencyInjectionConfiguration
{
    public static class DependenciesSetup
    {
        public static IServiceCollection AddDataDependenciesForGestaoAlunos(this IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment)
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

            // Alunos
            services.AddDbContext<GestaoAlunosContext>(options =>
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
            services.AddScoped<IAlunoRepository, AlunoRepository>();

            return services;
        }
    }
}