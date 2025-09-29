using Peo.Core.Messages;
using Peo.Core.Messages.DomainEvents;
using Peo.Core.Messages.Notifications;

namespace Peo.Core.Communication.Mediator
{
    public interface IMediatorHandler
    {
        Task PublicarEventoAsync<T>(T evento, CancellationToken token) where T : Event;
        Task<bool> EnviarComandoAsync<T>(T comando, CancellationToken token) where T : Command;
        Task PublicarNotificacaoAsync<T>(T notificacao, CancellationToken token) where T : DomainNotification;
        Task PublicarDomainEventAsync<T>(T notificacao, CancellationToken token) where T : DomainEvent;
    }
}