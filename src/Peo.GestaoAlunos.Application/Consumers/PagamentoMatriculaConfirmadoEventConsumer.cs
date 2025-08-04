using MassTransit;
using Microsoft.Extensions.Logging;
using Peo.Core.Messages.IntegrationEvents;

namespace Peo.GestaoAlunos.Application.Consumers
{
    public class PagamentoMatriculaConfirmadoEventConsumer(ILogger<PagamentoMatriculaConfirmadoEventConsumer> logger) : IConsumer<PagamentoMatriculaConfirmadoEvent>
    {
        public Task Consume(ConsumeContext<PagamentoMatriculaConfirmadoEvent> context)
        {
            var message = context.Message;

            logger.LogInformation("Processing PagamentoMatriculaConfirmadoEvent for MatriculaId: {MatriculaId}", message.MatriculaId);

            return Task.CompletedTask;
        }
    }
}