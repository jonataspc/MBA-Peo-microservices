using MassTransit;
using Microsoft.Extensions.Logging;
using Peo.Core.Interfaces.Services;

namespace Peo.Core.Infra.ServiceBus.Services
{
    public class MassTransitMessageBus(
        IPublishEndpoint publishEndpoint,
        ILogger<MassTransitMessageBus> logger
        ) : IMessageBus
    {
        public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                logger.LogInformation("Publishing message of type {MessageType}", typeof(T).Name);
                await publishEndpoint.Publish(message, cancellationToken);
                logger.LogInformation("Successfully published message of type {MessageType}", typeof(T).Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error publishing message of type {MessageType}", typeof(T).Name);
                throw;
            }
        }

        public async Task PublishAsync<T>(T message, Type messageType, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                logger.LogInformation("Publishing message of type {MessageType}", messageType.Name);
                await publishEndpoint.Publish(message, messageType, cancellationToken);
                logger.LogInformation("Successfully published message of type {MessageType}", messageType.Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error publishing message of type {MessageType}", messageType.Name);
                throw;
            }
        }
    }
}