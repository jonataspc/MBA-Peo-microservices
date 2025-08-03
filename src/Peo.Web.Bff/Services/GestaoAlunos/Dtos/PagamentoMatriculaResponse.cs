namespace Peo.Web.Bff.Services.GestaoAlunos.Dtos
{
    public record PagamentoMatriculaResponse(
        Guid PagamentoId,
        string Status,
        string? Mensagem);
} 