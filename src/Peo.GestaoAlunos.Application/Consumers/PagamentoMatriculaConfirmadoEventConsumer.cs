using MassTransit;
using Microsoft.Extensions.Logging;
using Peo.Core.Messages.IntegrationEvents;

namespace Peo.GestaoAlunos.Application.Consumers
{
    public class PagamentoMatriculaConfirmadoEventConsumer : IConsumer<PagamentoMatriculaConfirmadoEvent>
    {
        private readonly ILogger<PagamentoMatriculaConfirmadoEventConsumer> _logger;

        public PagamentoMatriculaConfirmadoEventConsumer(ILogger<PagamentoMatriculaConfirmadoEventConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<PagamentoMatriculaConfirmadoEvent> context)
        {
            var message = context.Message;
            var originalEvent = (PagamentoMatriculaConfirmadoEvent)message;

            _logger.LogInformation("Processing PagamentoMatriculaConfirmadoEvent for MatriculaId: {MatriculaId}", originalEvent.MatriculaId);

            return Task.CompletedTask;
        }
    }
}