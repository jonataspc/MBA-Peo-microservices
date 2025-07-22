using Peo.GestaoConteudo.Domain.ValueObjects;

namespace Peo.GestaoConteudo.Application.Dtos
{
    public class CursoResponse
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; } = null!;
        public string? Descricao { get; set; }
        public string? NomeInstrutor { get; set; }
        public ConteudoProgramatico? ConteudoProgramatico { get; set; }
        public decimal Preco { get; set; }
        public bool EstaPublicado { get; set; }
        public DateTime? DataPublicacao { get; set; }
        public IEnumerable<string> Tags { get; set; } = [];
    }
}