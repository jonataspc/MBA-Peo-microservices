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
            string connectionString = configuration.GetConnectionString("gestao-alunos-db") ?? throw new InvalidOperationException("Não localizada connection string");
            
            // Alunos
            services.AddDbContext<GestaoAlunosContext>(options =>
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

            // Repos
            services.AddScoped<IAlunoRepository, AlunoRepository>();

            return services;
        }
    }
}