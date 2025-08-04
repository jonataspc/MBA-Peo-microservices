namespace Peo.Core.Interfaces.Services
{
    public interface IMessageBus
    {
        Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;

        Task PublishAsync<T>(T message, Type messageType, CancellationToken cancellationToken = default) where T : class;
    }
}