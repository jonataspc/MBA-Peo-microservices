namespace Peo.Core.Messages.IntegrationResponses
{
    public record ObterDetalhesCursoResponse(Guid? CursoId, int? TotalAulas, string? Titulo, decimal? Preco);
}