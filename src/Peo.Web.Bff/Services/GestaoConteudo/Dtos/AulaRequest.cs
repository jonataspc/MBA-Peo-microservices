using System.ComponentModel.DataAnnotations;

namespace Peo.Web.Bff.Services.GestaoConteudo.Dtos
{
    public record AulaRequest(
        [Required]
        string Titulo,

        string? Descricao,

        [Required]
        string UrlVideo,

        [Required]
        TimeSpan Duracao,

        IEnumerable<ArquivoAulaRequest> Arquivos);
} 