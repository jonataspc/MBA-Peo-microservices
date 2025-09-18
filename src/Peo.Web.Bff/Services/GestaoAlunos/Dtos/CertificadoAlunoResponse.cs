namespace Peo.Web.Bff.Services.GestaoAlunos.Dtos
{
    public record CertificadoAlunoResponse(
        Guid CertificadoId,
        Guid MatriculaId,
        string Conteudo,
        DateTime? DataEmissao,
        string? NumeroCertificado);
} 