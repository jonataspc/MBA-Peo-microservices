namespace Peo.Web.Bff.Services.GestaoAlunos.Dtos
{
    public record MatriculaResponse(
        Guid Id,
        Guid CursoId,
        DateTime DataMatricula,
        DateTime? DataConclusao,
        string Status,
        double PercentualProgresso
    );
}