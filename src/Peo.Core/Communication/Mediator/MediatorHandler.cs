using MediatR;
using Peo.Core.Messages;
using Peo.Core.Messages.DomainEvents;
using Peo.Core.Messages.Notifications;

namespace Peo.Core.Communication.Mediator
{
    public class MediatorHandler(IMediator mediator) : IMediatorHandler
    {
        public async Task<bool> EnviarComandoAsync<T>(T comando, CancellationToken token) where T : Command
        {
            return await mediator.Send(comando);
        }

        public async Task PublicarEventoAsync<T>(T evento, CancellationToken token = default) where T : Event
        {
            await mediator.Publish(evento);
        }

        public async Task PublicarNotificacaoAsync<T>(T notificacao, CancellationToken token = default) where T : DomainNotification
        {
            await mediator.Publish(notificacao);
        }

        public async Task PublicarDomainEventAsync<T>(T notificacao, CancellationToken token = default) where T : DomainEvent
        {
            await mediator.Publish(notificacao);
        }
    }
}