namespace Peo.GestaoAlunos.Domain.Dtos
{
    public record AulaMatriculaDto(
    Guid MatriculaId,
    Guid CursoId,
    Guid AulaId,
    DateTime? DataInicio,
    DateTime? DataConclusao,
    string Status
);
}