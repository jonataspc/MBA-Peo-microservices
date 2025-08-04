using System.ComponentModel.DataAnnotations;

namespace Peo.Web.Bff.Services.GestaoAlunos.Dtos
{
    public class PagamentoMatriculaRequest
    {
        [Required]
        public Guid MatriculaId { get; set; }

        [Required]
        public CartaoCredito DadosCartao { get; set; } = null!;
    }
} 