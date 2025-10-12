namespace Peo.Web.Bff.Services.GestaoAlunos.Dtos;

public record AulaMatriculaResponse(
    Guid MatriculaId,
    Guid CursoId,
    Guid AulaId,
    DateTime? DataInicio,
    DateTime? DataConclusao,
    string Status,
    string? TituloCurso,
    string? TituloAula
);