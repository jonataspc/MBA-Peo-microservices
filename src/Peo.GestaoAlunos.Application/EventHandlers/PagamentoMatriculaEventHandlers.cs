using Peo.Core.Messages.IntegrationEvents;
using Peo.GestaoAlunos.Domain.Interfaces;

namespace Peo.GestaoAlunos.Application.EventHandlers
{
    public class PagamentoMatriculaEventHandlers(
        IEstudanteRepository estudanteRepository
        ) : INotificationHandler<PagamentoMatriculaConfirmadoEvent>, 
            INotificationHandler<PagamentoComFalhaEvent>
    {
        public async Task Handle(PagamentoMatriculaConfirmadoEvent notification, CancellationToken cancellationToken)
        {
            var matricula = await estudanteRepository.GetMatriculaByIdAsync(notification.MatriculaId)
                            ?? throw new InvalidOperationException($"Matrícula com ID {notification.MatriculaId} não encontrada");

            matricula.ConfirmarPagamento();
            await estudanteRepository.UnitOfWork.CommitAsync(CancellationToken.None);
        }

        public Task Handle(PagamentoComFalhaEvent notification, CancellationToken cancellationToken)
        {
            // Sinalizar aluno da falha no pagamento, via email, por exemplo
            return Task.CompletedTask;
        }
    }
}