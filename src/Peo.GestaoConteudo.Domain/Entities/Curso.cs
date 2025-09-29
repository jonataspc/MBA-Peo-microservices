using Peo.Core.DomainObjects;
using Peo.Core.Entities.Base;
using Peo.GestaoConteudo.Domain.ValueObjects;

namespace Peo.GestaoConteudo.Domain.Entities
{
    public class Curso : EntityBase, IAggregateRoot
    {
        public string Titulo { get; private set; } = null!;
        public string? Descricao { get; private set; }
        public string? InstrutorNome { get; private set; }
        public virtual ConteudoProgramatico? ConteudoProgramatico { get; private set; }
        public decimal Preco { get; private set; }
        public bool EstaPublicado { get; private set; }
        public DateTime? DataPublicacao { get; private set; }
        public virtual List<string> Tags { get; private set; } = [];
        public virtual ICollection<Aula> Aulas { get; private set; } = [];

        public Curso()
        {
        }

        public Curso(string titulo, string? descricao, string instrutorNome, ConteudoProgramatico? conteudoProgramatico, decimal preco, bool estaPublicado, DateTime? dataPublicacao, List<string> tags, ICollection<Aula> aulas)
        {
            Titulo = titulo;
            Descricao = descricao;
            InstrutorNome = instrutorNome;
            ConteudoProgramatico = conteudoProgramatico;
            Preco = preco;
            EstaPublicado = estaPublicado;
            DataPublicacao = dataPublicacao;
            Tags = tags;
            Aulas = aulas;
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
            if (string.IsNullOrEmpty(InstrutorNome))
                throw new DomainException("O campo InstrutorNome é obrigatório.");
            if (Preco <= 0)
                throw new DomainException("O campo Preco deve ser maior que zero.");
        }
    }
}