using System.Threading.Tasks;
using Peo.Core.Messages;
using Peo.Core.Messages.DomainEvents;
using Peo.Core.Messages.Notifications;

namespace Peo.Core.Communication.Mediator
{
    public interface IMediatorHandler
    {
        Task PublicarEventoAsync<T>(T evento) where T : Event;
        Task<bool> EnviarComandoAsync<T>(T comando) where T : Command;
        Task PublicarNotificacaoAsync<T>(T notificacao) where T : DomainNotification;
        Task PublicarDomainEventAsync<T>(T notificacao) where T : DomainEvent;
    }
}