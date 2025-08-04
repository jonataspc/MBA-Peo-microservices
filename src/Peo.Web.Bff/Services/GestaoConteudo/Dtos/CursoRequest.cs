using System.ComponentModel.DataAnnotations;

namespace Peo.Web.Bff.Services.GestaoConteudo.Dtos
{
    public record CursoRequest(
        [Required]
        string Titulo,

        string? Descricao,

        [Required]
        Guid InstrutorId,

        string? ConteudoProgramatico,

        [Required]
        decimal Preco,

        List<string>? Tags);
} 