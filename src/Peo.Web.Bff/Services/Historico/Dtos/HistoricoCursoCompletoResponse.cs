namespace Peo.Web.Bff.Services.Historico.Dtos
{
    public record HistoricoCursoCompletoResponse(
        Guid MatriculaId,
        string Aluno,
        Guid CursoId,
        string NomeCurso,
        string? DescricaoCurso,
        string? InstrutorNome,
        DateTime DataMatricula,
        DateTime? DataConclusao,
        string Status,
        int PercentualProgresso
    );

    public record ObterHistoricoCompletoCursosResponse(
        IEnumerable<HistoricoCursoCompletoResponse> Historico
    );
}