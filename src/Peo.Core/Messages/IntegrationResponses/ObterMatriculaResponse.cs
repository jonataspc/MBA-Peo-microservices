namespace Peo.Core.Messages.IntegrationResponses
{
    public record ObterMatriculaResponse(Guid? MatriculaId, Guid? CursoId, bool PermitePagamento);
}