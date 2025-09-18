namespace Peo.GestaoAlunos.Application.Dtos.Responses;

public record CertificadoAlunoResponse(
    Guid CertificadoId,
    Guid MatriculaId,
    string Conteudo,
    DateTime? DataEmissao,
    string? NumeroCertificado
);