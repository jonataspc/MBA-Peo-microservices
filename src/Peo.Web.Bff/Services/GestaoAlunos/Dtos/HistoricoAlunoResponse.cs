namespace Peo.Web.Bff.Services.GestaoAlunos.Dtos
{
    public record HistoricoAlunoResponse(
        Guid Id,
        string NomeAluno,
        Guid CursoId,
        string NomeCurso,
        DateTime DataMatricula,
        DateTime? DataConclusao,
        string Status,
        double PercentualProgresso
    );
}
