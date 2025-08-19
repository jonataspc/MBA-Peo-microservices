using Peo.GestaoAlunos.Domain.ValueObjects;

namespace Peo.GestaoAlunos.Application.Dtos.Responses;

public record MatriculaResponse(
    Guid Id,
    Guid CursoId,
    DateTime DataMatricula,
    DateTime? DataConclusao,
    string Status,
    double PercentualProgresso
);