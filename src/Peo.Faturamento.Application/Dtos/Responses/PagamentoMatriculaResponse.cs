namespace Peo.Faturamento.Application.Dtos.Responses;

public record PagamentoMatriculaResponse(
    Guid MatriculaId,
    string? StatusPagamento,
    decimal ValorPago
);