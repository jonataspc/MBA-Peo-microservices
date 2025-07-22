using Microsoft.AspNetCore.Identity;
using Peo.Core.Communication.Mediator;
using Peo.Core.Dtos;
using Peo.Core.Interfaces.Services;
using Peo.Faturamento.Application.DiConfig;
using Peo.Faturamento.Domain.Interfaces.Brokers;
using Peo.Faturamento.Infra.Data.DiConfig;
using Peo.Faturamento.Integrations.Paypal.Services;
using Peo.GestaoAlunos.Application.DiConfig;
using Peo.GestaoAlunos.Application.Endpoints;
using Peo.GestaoAlunos.Infra.Data.DiConfig;
using Peo.GestaoConteudo.Application;
using Peo.GestaoConteudo.Application.DiConfig;
using Peo.GestaoConteudo.Infra.Data.DiConfig;
using Peo.Identity.Application.DiConfig;
using Peo.Identity.Application.Extensions;
using Peo.Identity.Application.Services;
using Peo.Identity.Domain.Interfaces.Data;
using Peo.Identity.Domain.Interfaces.Services;
using Peo.Identity.Infra.Data.Contexts;
using Peo.Identity.Infra.Data.DiConfig;
using Peo.Identity.Infra.Data.Repositories;
using Peo.Web.Api.Services;

namespace Peo.Web.Api.Configuration
{
    public static class Dependencies
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            services.AddDataDependenciesForIdentity(configuration, hostEnvironment)
                    .AddDataDependenciesForGestaoAlunos(configuration, hostEnvironment)
                    .AddDataDependenciesForGestaoConteudo(configuration, hostEnvironment)
                    .AddDataDependenciesForFaturamento(configuration, hostEnvironment)

                    .AddServicesForIdentity()
                    .AddServicesForGestaoAlunos()
                    .AddServicesForFaturamento()
                    .AddServicesForGestaoConteudo()

                    .AddIdentity()
                    .AddAppSettings(configuration)
                    .AddMediator()
                    .AddApiServices()
                    .AddExternalServices();

            return services;
        }

        private static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddScoped<IAppIdentityUser, AppIdentityUser>();
            return services;
        }

        private static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services.AddIdentityCore<IdentityUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = false;

                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
            })
                     .AddRoles<IdentityRole>()
                     .AddEntityFrameworkStores<IdentityContext>()
                     .AddApiEndpoints();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }

        private static IServiceCollection AddAppSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Position));
            return services;
        }

        private static IServiceCollection AddMediator(this IServiceCollection services)
        {
            services.AddScoped<IMediatorHandler, MediatorHandler>();
            return services;
        }

        private static IServiceCollection AddExternalServices(this IServiceCollection services)
        {
            services.AddScoped<IBrokerPagamentoService, PaypalBrokerService>();
            return services;
        }

        public static WebApplication MapEndpoints(this WebApplication app)
        {
            app.MapIdentityEndpoints();
            app.MapCursoEndpoints();
            app.MapEstudanteEndpoints();
            return app;
        }
    }
}