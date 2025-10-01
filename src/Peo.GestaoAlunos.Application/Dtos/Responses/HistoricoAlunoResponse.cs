namespace Peo.GestaoAlunos.Application.Dtos.Responses
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