namespace Peo.GestaoConteudo.Application.Dtos
{
    public class AulaResponse
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; } = null!;
        public string? Descricao { get; set; }
        public string UrlVideo { get; set; } = null!;
        public TimeSpan Duracao { get; set; }
        public IEnumerable<ArquivoAulaResponse> Arquivos { get; set; } = [];
    }
}