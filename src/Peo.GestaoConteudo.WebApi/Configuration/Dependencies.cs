using Peo.Core.Communication.Mediator;
using Peo.Core.Dtos;
using Peo.Core.Infra.ServiceBus.Services;
using Peo.Core.Interfaces.Services;
using Peo.Core.Web.Services;
using Peo.GestaoConteudo.Application.Consumers;
using Peo.GestaoConteudo.Application.DiConfig;
using Peo.GestaoConteudo.Infra.Data.DiConfig;

namespace Peo.GestaoConteudo.WebApi.Configuration
{
    public static class Dependencies
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            services.AddDataDependenciesForGestaoConteudo(configuration, hostEnvironment)
                    .AddServicesForGestaoConteudo()
                    .AddAppSettings(configuration)
                    .AddMediator()
                    .AddServiceBus(configuration, [typeof(ObterDetalhesCursoConsumer).Assembly])
                    .AddApiServices();

            return services;
        }

        private static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IAppIdentityUser, AppIdentityUser>();
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

        public static WebApplication MapEndpoints(this WebApplication app)
        {
            app.MapCursoEndpoints();

            return app;
        }
    }
}