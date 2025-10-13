namespace Peo.GestaoAlunos.Application.Dtos.Responses;

public record AulaMatriculaResponse(
    Guid MatriculaId,
    Guid CursoId,
    Guid AulaId,
    DateTime? DataInicio,
    DateTime? DataConclusao,
    string Status
);

