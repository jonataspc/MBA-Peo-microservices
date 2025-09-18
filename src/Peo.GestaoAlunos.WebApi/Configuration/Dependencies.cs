using Peo.Core.Communication.Mediator;
using Peo.Core.Dtos;
using Peo.Core.Infra.ServiceBus.Services;
using Peo.Core.Interfaces.Services;
using Peo.Core.Web.Services;
using Peo.GestaoAlunos.Application.Consumers;
using Peo.GestaoAlunos.Application.DiConfig;
using Peo.GestaoAlunos.Application.Endpoints;
using Peo.GestaoAlunos.Infra.Data.DiConfig;

namespace Peo.GestaoAlunos.WebApi.Configuration
{
    public static class Dependencies
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            services.AddDataDependenciesForGestaoAlunos(configuration, hostEnvironment)
                    .AddServicesForGestaoAlunos()
                    .AddAppSettings(configuration)
                    .AddMediator()
                    .AddServiceBus(configuration, [typeof(PagamentoMatriculaEventConsumer).Assembly])
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
            app.MapAlunoEndpoints();
            return app;
        }
    }
}