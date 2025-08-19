using System.ComponentModel.DataAnnotations;

namespace Peo.Web.Bff.Services.GestaoConteudo.Dtos
{
    public record ArquivoAulaRequest(
        [Required]
        string Titulo,

        [Required]
        string Url);
} 