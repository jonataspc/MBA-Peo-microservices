namespace Peo.Web.Bff.Services.GestaoConteudo.Dtos
{
    public class ArquivoAulaResponse
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; } = null!;
        public string Url { get; set; } = null!;
    }
} 