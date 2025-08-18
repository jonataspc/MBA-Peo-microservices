using Peo.Core.Communication.Mediator;
using Peo.Core.Dtos;
using Peo.Core.Infra.ServiceBus.Services;
using Peo.Core.Interfaces.Services;
using Peo.Core.Web.Services;
using Peo.Faturamento.Application.DiConfig;
using Peo.Faturamento.Application.Endpoints;
using Peo.Faturamento.Domain.Interfaces.Brokers;
using Peo.Faturamento.Infra.Data.DiConfig;
using Peo.Faturamento.Integrations.Paypal.Services;

namespace Peo.Faturamento.WebApi.Configuration
{
    public static class Dependencies
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            services.AddDataDependenciesForFaturamento(configuration, hostEnvironment)
                    .AddServicesForFaturamento()
                    .AddAppSettings(configuration)
                    .AddMediator()
                    .AddServiceBus(configuration)
                    .AddApiServices()
                    .AddExternalServices();

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

        private static IServiceCollection AddExternalServices(this IServiceCollection services)
        {
            services.AddScoped<IBrokerPagamentoService, PaypalBrokerService>();
            return services;
        }

        public static WebApplication MapEndpoints(this WebApplication app)
        {
            app.MapFaturamentoEndpoints();
            return app;
        }
    }
}