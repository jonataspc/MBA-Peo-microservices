using Microsoft.AspNetCore.Components;
using MudBlazor;
using Peo.Web.Spa.Services;
using System.Threading.Tasks;
using static MudBlazor.CategoryTypes;

namespace Peo.Web.Spa.Pages.Alunos
{
    public partial class IniciarAula : IDisposable
    {
        // Estado
        private List<CursoMatriculado> _cursosMatriculados = new();
        private List<ProgressoMatricula> _progressoMatricula = new();
        private Dictionary<Guid, Curso> _cursosCache = new();
        private Guid _cursoSelecionadoId;
        private string? _mensagemErro;

        // Injeções de Dependência
        [Inject] private WebApiClient Api { get; set; } = null!;
        [Inject] private IDialogService DialogService { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        private CancellationTokenSource? _cts;

        protected override async Task OnInitializedAsync()
        {
            await CarregarDados();
        }

        private async Task CarregarDados()
        {
               _mensagemErro = null;

            try
            {
                // Cancela operações anteriores
                _cts?.Cancel();
                _cts?.Dispose();
                _cts = new CancellationTokenSource();

                // Carrega cursos primeiro (para ter os detalhes)
                await ObterCursos();

                // Depois carrega as matrículas
                await ObterMatriculasDoAluno();
            }
            catch (OperationCanceledException)
            {
                // Operação cancelada, ignore
            }
            catch (Exception ex)
            {
                _mensagemErro = "Erro ao carregar dados. Tente novamente. " + ex.Message;
                Snackbar.Add(_mensagemErro, Severity.Error);
            }

        }


        private async Task ObterCursos()
        {
            var respCursos = await Api.GetV1ConteudoCursoAsync(_cts!.Token);
            var cursos = respCursos?.Cursos ?? Enumerable.Empty<Curso>();

            // Cria dicionário para busca rápida
            _cursosCache = cursos
                .Where(c => Guid.TryParse(c.Id, out _))
                .ToDictionary(
                    c => Guid.Parse(c.Id),
                    c => c
                );
        }

        private async Task ObterMatriculasDoAluno()
        {
            var matriculas = await Api.GetV1AlunoMatriculaAsync(_cts!.Token);

            // Usa List para melhor performance
            _cursosMatriculados = matriculas
                .Select(m => CriarCursoMatriculado(m))
                .Where(cm => cm != null && cm.Status != "PendentePagamento")
                .Select(cm => cm!)
                .ToList();

            // Seleciona o primeiro curso automaticamente
            if (_cursosMatriculados.Any())
            {
                _cursoSelecionadoId = _cursosMatriculados.First().CursoId;
                await ListarAulas();
            }
        }

        private CursoMatriculado? CriarCursoMatriculado(MatriculaResponse matricula)
        {
            if (!_cursosCache.TryGetValue(matricula.CursoId, out var curso))
            {
                return null;
            }

            return new CursoMatriculado
            {
                CursoId = matricula.CursoId,
                MatriculaId = matricula.Id,
                NomeCurso = curso.Titulo,
                DescricaoCurso = curso.Descricao,
                DataMatricula = matricula.DataMatricula.DateTime,
                DataConclusao = matricula.DataConclusao?.DateTime,
                Status = matricula.Status
            };
        }


        private string ObterNomeCurso(Guid cursoId)
        {
            return _cursosMatriculados
                .FirstOrDefault(c => c.CursoId == cursoId)
                ?.NomeCurso ?? "Curso não encontrado";
        }



        #region aulas disponiveis
        private async Task ListarAulas()
        {
            var matriculaId = _cursosMatriculados.FirstOrDefault(c => c.CursoId == _cursoSelecionadoId)?.MatriculaId;
            if (matriculaId == null || matriculaId == Guid.Empty)
            {
                _progressoMatricula.Clear();
                return;
            }

            await RetornaAulasDaMatricula((Guid)matriculaId);

        }

        private async Task StartClass(ProgressoMatricula itemClicado)
        {

            var iniciaAula = await Api.PostV1AlunoMatriculaAulaIniciarAsync(new()
            {
                MatriculaId = itemClicado.MatriculaId,
                AulaId = itemClicado.AulaId
            }, _cts!.Token);

            Snackbar.Add($"Iniciando a aula: {itemClicado.TituloAula}", Severity.Info);
            await ListarAulas();
        }
        private async Task CloseClass(ProgressoMatricula itemClicado)
        {

            var finalizaAula = await Api.PostV1AlunoMatriculaAulaConcluirAsync(new()
            {
                MatriculaId = itemClicado.MatriculaId,
                AulaId = itemClicado.AulaId
            }, _cts!.Token);
            Snackbar.Add($"Finalizando a aula: {itemClicado.TituloAula}", Severity.Info);
            await ListarAulas();
        }

        private async Task RetornaAulasDaMatricula(Guid matriculaId)
        { 
            _progressoMatricula = new List<ProgressoMatricula>();

            var minhasAulas = await Api.GetV1AlunoMatriculaAulasAsync(matriculaId, _cts!.Token);
            foreach (var aula in minhasAulas)
            {
                _progressoMatricula.Add(new ProgressoMatricula
                {
                    MatriculaId = matriculaId,
                    AulaId = aula.AulaId,
                    TituloAula = aula.TituloAula,
                    DataInicio = aula.DataInicio?.DateTime,
                    DataConclusao = aula.DataConclusao?.DateTime,
                    Status = aula.Status
                });

            }

        }
        #endregion

       
        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }

    public class CursoMatriculado
    {
        public Guid CursoId { get; set; }
        public Guid MatriculaId { get; set; }
        public string? NomeCurso { get; set; }
        public string? DescricaoCurso { get; set; }
        public DateTime DataMatricula { get; set; }
        public DateTime? DataConclusao { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class ProgressoMatricula
    {
       // public Guid id { get; set; }
        public Guid MatriculaId { get; set; }
        public Guid AulaId { get; set; }
        public String? TituloAula { get; set; } 
        public DateTime? DataInicio { get; set; }
        public DateTime? DataConclusao { get; set; }
        public string? Status { get;set; } 

    }

}