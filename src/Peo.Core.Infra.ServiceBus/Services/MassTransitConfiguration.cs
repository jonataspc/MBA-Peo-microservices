using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Peo.Core.Interfaces.Services;
using Peo.Core.Messages.IntegrationRequests;
using System.Reflection;

namespace Peo.Core.Infra.ServiceBus.Services
{
    public static class MassTransitConfiguration
    {
        public static IServiceCollection AddServiceBus(this IServiceCollection services, IConfiguration configuration, Assembly[]? consumers = null)
        {
            services.AddMassTransit(x =>
            {
                // Configure consumers
                if (consumers is not null)
                {
                    x.AddConsumers(consumers);
                }

                // Configure RabbitMQ
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration.GetConnectionString("messaging"));
                    cfg.ConfigureJsonSerializerOptions(o =>
                    {
                        o.IncludeFields = true;
                        return o;
                    }
                    );
                    cfg.ConfigureEndpoints(context);
                });

                Type[] requestClients = [
                    typeof(ObterDetalhesCursoRequest),
                    typeof(ObterMatriculaRequest)
                    ];

                // Configure request clients
                foreach (var requestClient in requestClients)
                {
                    x.AddRequestClient(requestClient);
                }
            });

            // Register the IMessageBus implementation
            services.AddScoped<IMessageBus, MassTransitMessageBus>();

            return services;
        }
    }
}