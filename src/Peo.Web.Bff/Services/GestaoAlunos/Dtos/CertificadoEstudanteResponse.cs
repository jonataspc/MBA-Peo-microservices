namespace Peo.Web.Bff.Services.GestaoAlunos.Dtos
{
    public record CertificadoEstudanteResponse(
        Guid CertificadoId,
        Guid MatriculaId,
        string Conteudo,
        DateTime? DataEmissao,
        string? NumeroCertificado);
} 