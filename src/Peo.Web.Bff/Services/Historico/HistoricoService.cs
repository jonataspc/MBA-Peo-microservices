using Microsoft.AspNetCore.Http.HttpResults;
using Peo.Web.Bff.Services.GestaoAlunos;
using Peo.Web.Bff.Services.GestaoAlunos.Dtos;
using Peo.Web.Bff.Services.GestaoConteudo;
using Peo.Web.Bff.Services.GestaoConteudo.Dtos;
using Peo.Web.Bff.Services.Historico.Dtos;

namespace Peo.Web.Bff.Services.Historico
{
    public class HistoricoService
    {
        private readonly GestaoAlunosService _gestaoAlunosService;
        private readonly GestaoConteudoService _gestaoConteudoService;

        public HistoricoService(
            GestaoAlunosService gestaoAlunosService,
            GestaoConteudoService gestaoConteudoService)
        {
            _gestaoAlunosService = gestaoAlunosService;
            _gestaoConteudoService = gestaoConteudoService;
        }

        public async Task<Results<Ok<ObterHistoricoCompletoCursosResponse>, BadRequest, UnauthorizedHttpResult, BadRequest<object>>>
            ObterHistoricoCompletoCursosAsync(CancellationToken ct)
        {
            var historicoResult = await _gestaoAlunosService.ObterHistoricoAsync(ct);

            if (historicoResult.Result is UnauthorizedHttpResult)
                return TypedResults.Unauthorized();

            if (historicoResult.Result is BadRequest)
                return TypedResults.BadRequest();

            if (historicoResult.Result is not Ok<IEnumerable<HistoricoAlunoResponse>> okHistorico)
                return TypedResults.BadRequest<object>("Erro ao obter histórico de matrículas");

            var historico = (okHistorico.Value ?? Enumerable.Empty<HistoricoAlunoResponse>()).ToList();

            if (!historico.Any())
            {
                return TypedResults.Ok(new ObterHistoricoCompletoCursosResponse(
                    Enumerable.Empty<HistoricoCursoCompletoResponse>()));
            }

            var cursosIds = historico.Select(h => h.CursoId).Distinct().ToList();
            var cursosTasks = cursosIds.Select(id =>
                _gestaoConteudoService.ObterCursoPorIdAsync(id, ct));

            var cursosResults = await Task.WhenAll(cursosTasks);

            var cursosDict = new Dictionary<string, Curso>();
            foreach (var result in cursosResults)
            {
                if (result.Result is Ok<Curso> okCurso && okCurso.Value != null)
                {
                    var curso = okCurso.Value;
                    cursosDict[curso.Id] = curso;
                }
            }

            var historicoCompleto = historico.Select(matricula =>
            {
                var cursoIdString = matricula.CursoId.ToString();
                cursosDict.TryGetValue(cursoIdString, out var curso);

                return new HistoricoCursoCompletoResponse(
                    MatriculaId: matricula.Id,
                    NomeAluno: matricula.NomeAluno,
                    CursoId: matricula.CursoId,
                    NomeCurso: curso?.Titulo ?? matricula.NomeCurso ?? "Curso não encontrado",
                    DescricaoCurso: curso?.Descricao,
                    InstrutorNome: curso?.InstrutorNome,
                    DataMatricula: matricula.DataMatricula,
                    DataConclusao: matricula.DataConclusao,
                    Status: matricula.Status,
                    PercentualProgresso: (int)matricula.PercentualProgresso
                );
            })
            .OrderByDescending(h => h.DataConclusao ?? h.DataMatricula)
            .ToList();

            return TypedResults.Ok(new ObterHistoricoCompletoCursosResponse(historicoCompleto));
        }
    }
}