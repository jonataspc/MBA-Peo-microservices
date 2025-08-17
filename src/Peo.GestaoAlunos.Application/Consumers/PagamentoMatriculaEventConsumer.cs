using MassTransit;
using Microsoft.Extensions.Logging;
using Peo.Core.Messages.IntegrationEvents;
using Peo.GestaoAlunos.Domain.Interfaces;

namespace Peo.GestaoAlunos.Application.Consumers
{
    public class PagamentoMatriculaEventConsumer(
        ILogger<PagamentoMatriculaEventConsumer> logger,
        IEstudanteRepository estudanteRepository
        ) : IConsumer<PagamentoMatriculaConfirmadoEvent>, IConsumer<PagamentoComFalhaEvent>
    {
        public Task Consume(ConsumeContext<PagamentoComFalhaEvent> context)
        {
            var message = context.Message;

            logger.LogWarning("Processing PagamentoComFalhaEvent for MatriculaId: {MatriculaId}, Motivo: {Motivo}",
                message.MatriculaId, message.Motivo);

            // Sinalizar aluno da falha no pagamento, via email, por exemplo

            return Task.CompletedTask;
        }

        public async Task Consume(ConsumeContext<PagamentoMatriculaConfirmadoEvent> context)
        {
            var message = context.Message;

            logger.LogInformation("Processing PagamentoMatriculaConfirmadoEvent for MatriculaId: {MatriculaId}", message.MatriculaId);

            var matricula = await estudanteRepository.GetMatriculaByIdAsync(message.MatriculaId)
                            ?? throw new InvalidOperationException($"Matrícula com ID {message.MatriculaId} não encontrada");

            matricula.ConfirmarPagamento();
            await estudanteRepository.UnitOfWork.CommitAsync(CancellationToken.None);

            // Dispara email ao aluno informando que a matrícula foi confirmada
        }
    }
}