using MassTransit;
using Microsoft.Extensions.Logging;
using Peo.Core.Messages.IntegrationEvents;

namespace Peo.GestaoAlunos.Application.Consumers
{
    public class PagamentoComFalhaEventConsumer : IConsumer<PagamentoComFalhaEvent>
    {
        private readonly ILogger<PagamentoComFalhaEventConsumer> _logger;

        public PagamentoComFalhaEventConsumer(ILogger<PagamentoComFalhaEventConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<PagamentoComFalhaEvent> context)
        {
            var message = context.Message;
            var originalEvent = (PagamentoComFalhaEvent)message;

            _logger.LogWarning("Processing PagamentoComFalhaEvent for MatriculaId: {MatriculaId}, Motivo: {Motivo}",
                originalEvent.MatriculaId, originalEvent.Motivo);

            return Task.CompletedTask;
        }
    }
}