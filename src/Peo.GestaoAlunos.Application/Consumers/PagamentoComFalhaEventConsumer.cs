using MassTransit;
using Microsoft.Extensions.Logging;
using Peo.Core.Messages.IntegrationEvents;

namespace Peo.GestaoAlunos.Application.Consumers
{
    public class PagamentoComFalhaEventConsumer(ILogger<PagamentoComFalhaEventConsumer> logger) : IConsumer<PagamentoComFalhaEvent>
    {
        public Task Consume(ConsumeContext<PagamentoComFalhaEvent> context)
        {
            var message = context.Message;

            logger.LogWarning("Processing PagamentoComFalhaEvent for MatriculaId: {MatriculaId}, Motivo: {Motivo}",
                message.MatriculaId, message.Motivo);

            return Task.CompletedTask;
        }
    }
}