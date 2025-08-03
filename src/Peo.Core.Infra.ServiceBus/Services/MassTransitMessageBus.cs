using MassTransit;
using Microsoft.Extensions.Logging;
using Peo.Core.Interfaces.Services;

namespace Peo.Core.Infra.ServiceBus.Services
{
    public class MassTransitMessageBus : IMessageBus
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<MassTransitMessageBus> _logger;

        public MassTransitMessageBus(IPublishEndpoint publishEndpoint, ILogger<MassTransitMessageBus> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                _logger.LogInformation("Publishing message of type {MessageType}", typeof(T).Name);

                await _publishEndpoint.Publish(message, cancellationToken);

                _logger.LogInformation("Successfully published message of type {MessageType}", typeof(T).Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing message of type {MessageType}", typeof(T).Name);
                throw;
            }
        }

        public async Task PublishAsync<T>(T message, Type messageType, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                _logger.LogInformation("Publishing message of type {MessageType}", messageType.Name);

                await _publishEndpoint.Publish(message, messageType, cancellationToken);

                _logger.LogInformation("Successfully published message of type {MessageType}", messageType.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing message of type {MessageType}", messageType.Name);
                throw;
            }
        }
    }
}