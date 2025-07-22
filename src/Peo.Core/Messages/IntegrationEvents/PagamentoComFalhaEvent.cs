using Peo.Core.Messages.IntegrationEvents.Base;

namespace Peo.Core.Messages.IntegrationEvents
{
    public class PagamentoComFalhaEvent : IntegrationEvent
    {
        public Guid MatriculaId { get; private set; }
        public string? Motivo { get; private set; }

        public PagamentoComFalhaEvent(Guid matriculaId, string? motivo)
        {
            MatriculaId = matriculaId;
            Motivo = motivo;
        }
    }
}