using Peo.Core.DomainObjects;
using Peo.Core.Entities.Base;

namespace Peo.GestaoConteudo.Domain.Entities
{
    public class Aula : EntityBase
    {
        public string Titulo { get; private set; } = null!;
        public string? Descricao { get; private set; }
        public string UrlVideo { get; private set; } = null!;
        public TimeSpan Duracao { get; private set; }
        public virtual ICollection<ArquivoAula> Arquivos { get; private set; } = [];

        public virtual Curso Curso { get; private set; } = null!;
        public Guid CursoId { get; private set; }

        protected Aula()
        {
        }

        public Aula(string titulo, string? descricao, string urlVideo, TimeSpan duracao, ICollection<ArquivoAula> arquivos, Guid cursoId)
        {
            Titulo = titulo;
            Descricao = descricao;
            UrlVideo = urlVideo;
            Duracao = duracao;
            Arquivos = arquivos;
            CursoId = cursoId;
            Validar();
        }

        public void AtualizarTituloDescricao(string titulo, string? descricao)
        {
            Titulo = titulo;
            Descricao = descricao;
        }

        private void Validar()
        {
            if (string.IsNullOrEmpty(Titulo))
                throw new DomainException("O campo Titulo é obrigatório.");
            if (CursoId == Guid.Empty)
                throw new DomainException("O campo CursoId é obrigatório.");
        }
    }
}