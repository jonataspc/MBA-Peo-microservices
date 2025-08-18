namespace Peo.Web.Bff.Services.Faturamento.Dtos
{
    public record EfetuarPagamentoResponse(Guid MatriculaId, string? StatusPagamento, decimal ValorPago);
}