using Peo.Core.Entities.Base;

namespace Peo.GestaoConteudo.Domain.Entities
{
    public class ArquivoAula : EntityBase
    {
        public string Titulo { get; private set; } = null!;
        public string Url { get; private set; } = null!;

        public virtual Aula? Aula { get; }
        public Guid AulaId { get; private set; }

        public ArquivoAula()
        {
        }

        public ArquivoAula(string titulo, string url, Guid aulaId)
        {
            Titulo = titulo;
            Url = url;
            AulaId = aulaId;
        }
    }
}