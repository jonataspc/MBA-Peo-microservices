using Microsoft.AspNetCore.Components;
using MudBlazor;
using Peo.Web.Spa.Services;

namespace Peo.Web.Spa.Pages.Alunos
{
    public partial class IniciarAula
    {
        private IEnumerable<MatriculaResponse> _MatriculaResponse = new List<MatriculaResponse>();
        private IEnumerable<Curso> _cursos = new List<Curso>();
        private IEnumerable<CursoMatriculado> _cursosMatriculado = new List<CursoMatriculado>();
        private Guid _cursoSelecionadoId;

        [Inject] WebApiClient Api { get; set; } = null!;
        [Inject] IDialogService DialogService { get; set; } = null!;
        [Inject] ISnackbar Snackbar { get; set; } = null!;
        private CancellationTokenSource? _cts;

        protected override async Task OnInitializedAsync()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            await ObterCursos();
            await ObterMatriculasDoAluno();
        }

        private async Task ObterMatriculasDoAluno()
        {
            try
            {
                var resp = await Api.GetV1AlunoMatriculaAsync(_cts.Token);
                foreach (var curso in resp)
                {
                    var cursoDetalhes = ProcurarCursoPorId(curso.CursoId);
                    if (cursoDetalhes != null)
                    {
                        _cursosMatriculado = _cursosMatriculado.Append(new CursoMatriculado
                        {
                            CursoId = curso.CursoId,
                            NomeCurso = cursoDetalhes.Titulo,
                            DescricaoCurso = cursoDetalhes.Descricao,
                            DataMatricula = curso.DataMatricula.DateTime, // FIX: Convert DateTimeOffset to DateTime
                            DataConclusao = curso.DataConclusao?.DateTime, // FIX: Convert nullable DateTimeOffset to nullable DateTime
                            Status = curso.Status
                        });
                    }
                }

                if (_cursosMatriculado.Any())
                {
                    _cursoSelecionadoId = _cursosMatriculado.First().CursoId;
                }

            }
            catch (Exception e)
            {

                throw;
            }

        }

        private async Task ObterCursos()
        {
            try
            {
                var respCursos = await Api.GetV1ConteudoCursoAsync(_cts.Token);

                _cursos = respCursos?.Cursos ?? Enumerable.Empty<Curso>();
                foreach (var curso in _cursos)
                {

                    _ = Guid.TryParse(curso.Id,out Guid cursoId);
                }


            }
            catch (Exception e)
            {

                throw;
            }
        }

        // Exemplo de como procurar um curso em uma lista de cursos pelo Id
        private Curso? ProcurarCursoPorId(Guid cursoId)
        {
            // Considerando que _cursos é IEnumerable<Curso> e Curso.Id é string
            return _cursos.FirstOrDefault(c => Guid.TryParse(c.Id, out var id) && id == cursoId);
        }
    }
}

public class CursoMatriculado
{
    public Guid CursoId { get; set; }
    public string? NomeCurso { get; set; }
    public string? DescricaoCurso { get; set; }
    public DateTime DataMatricula { get; set; }
    public DateTime? DataConclusao { get; set; }
    public string Status { get; set; }
}