using Peo.Core.Messages.IntegrationEvents.Base;

namespace Peo.Core.Messages.IntegrationEvents
{
    public class PagamentoMatriculaConfirmadoEvent : IntegrationEvent
    {
        public Guid MatriculaId { get; private set; }
        public decimal Valor { get; private set; }
        public DateTime? DataPagamento { get; private set; }

        public PagamentoMatriculaConfirmadoEvent(Guid matriculaId, decimal valor, DateTime? dataPagamento)
        {
            MatriculaId = matriculaId;
            Valor = valor;
            DataPagamento = dataPagamento;
        }
    }
}