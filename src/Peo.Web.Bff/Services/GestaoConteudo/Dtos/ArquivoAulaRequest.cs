using System.ComponentModel.DataAnnotations;

namespace Peo.Web.Bff.Services.GestaoConteudo.Dtos
{
    public record ArquivoAulaRequest(
        [Required]
        string Nome,

        [Required]
        string Url);
} 