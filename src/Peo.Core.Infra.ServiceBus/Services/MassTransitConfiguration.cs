using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Peo.Core.Infra.ServiceBus.Services
{
    public static class MassTransitConfiguration
    {
        public static IServiceCollection AddServiceBus(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMqSettings = configuration.GetSection("RabbitMQ").Get<RabbitMqSettings>();

            services.AddMassTransit(x =>
            {
                // Configure consumers
                x.AddConsumers(Assembly.GetExecutingAssembly());

                // Configure RabbitMQ
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitMqSettings?.Host ?? "localhost", "/", h =>
                    {
                        h.Username(rabbitMqSettings?.Username ?? "guest");
                        h.Password(rabbitMqSettings?.Password ?? "guest");
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }

    public class RabbitMqSettings
    {
        public string Host { get; set; } = "localhost";
        public string Username { get; set; } = "guest";
        public string Password { get; set; } = "guest";
    }
}