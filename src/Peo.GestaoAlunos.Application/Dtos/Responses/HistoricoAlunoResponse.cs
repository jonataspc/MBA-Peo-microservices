namespace Peo.GestaoAlunos.Application.Dtos.Responses
{
    public record HistoricoAlunoResponse(
        Guid Id,
        Guid CursoId,
        DateTime DataMatricula,
        DateTime? DataConclusao,
        string Status,
        double PercentualProgresso
    );
}