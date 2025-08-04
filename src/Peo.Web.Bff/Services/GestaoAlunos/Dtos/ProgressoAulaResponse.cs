namespace Peo.Web.Bff.Services.GestaoAlunos.Dtos
{
    public record ProgressoAulaResponse(
        Guid MatriculaId,
        Guid AulaId,
        string Status,
        DateTime? DataInicio,
        DateTime? DataConclusao);
} 