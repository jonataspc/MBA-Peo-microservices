namespace Peo.Web.Bff.Services.GestaoConteudo.Dtos
{
    public class ArquivoAulaResponse
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = null!;
        public string Url { get; set; } = null!;
    }
} 