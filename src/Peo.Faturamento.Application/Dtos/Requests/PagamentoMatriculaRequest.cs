using Peo.Core.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Peo.Faturamento.Application.Dtos.Requests;

public class PagamentoMatriculaRequest
{
    [Required]
    public Guid MatriculaId { get; set; }

    [Required]
    public CartaoCredito DadosCartao { get; set; } = null!;
}