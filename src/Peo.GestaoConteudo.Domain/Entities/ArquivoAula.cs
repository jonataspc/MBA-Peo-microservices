using Peo.Core.DomainObjects;
using Peo.Core.Entities.Base;

namespace Peo.GestaoConteudo.Domain.Entities
{
    public class ArquivoAula : EntityBase
    {
        public string Titulo { get; private set; } = null!;
        public string Url { get; private set; } = null!;

        public virtual Aula? Aula { get; }
        public Guid AulaId { get; private set; }

        protected ArquivoAula()
        {
        }

        public ArquivoAula(string titulo, string url, Guid aulaId)
        {
            Titulo = titulo;
            Url = url;
            AulaId = aulaId;
            Validar();
        }

        private void Validar()
        {
            if (string.IsNullOrEmpty(Titulo))
                throw new DomainException("O campo Titulo é obrigatório.");
            if (AulaId == Guid.Empty)
                throw new DomainException("O campo AulaId é obrigatório.");
        }
    }
}