namespace Peo.GestaoAlunos.Application.Dtos.Responses;

public record CertificadoEstudanteResponse(
    Guid CertificadoId,
    Guid MatriculaId,
    string Conteudo,
    DateTime? DataEmissao,
    string? NumeroCertificado
);