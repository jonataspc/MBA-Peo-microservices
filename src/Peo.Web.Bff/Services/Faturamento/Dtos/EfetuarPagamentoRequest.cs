using Peo.Core.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Peo.Web.Bff.Services.Faturamento.Dtos
{
    public class EfetuarPagamentoRequest
    {
        [Required]
        public Guid MatriculaId { get; set; }

        [Required]
        public CartaoCredito DadosCartao { get; set; } = null!;
    }
}