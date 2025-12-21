using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Peo.Core.Interfaces.Data;
using Peo.GestaoConteudo.Domain.Entities;
using Peo.GestaoConteudo.Infra.Data.Contexts;
using Peo.GestaoConteudo.Infra.Data.Repositories;

namespace Peo.GestaoConteudo.Infra.Data.DependencyInjectionConfiguration
{
    public static class DependenciesSetup
    {
        public static IServiceCollection AddDataDependenciesForGestaoConteudo(this IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            string connectionString = configuration.GetConnectionString("gestao-conteudo-db") ?? throw new InvalidOperationException("Não localizada connection string");

            // GestaoConteudo
            services.AddDbContext<GestaoConteudoContext>(options =>
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
            services.AddScoped<IRepository<Curso>, CursoRepository>();

            return services;
        }
    }
}