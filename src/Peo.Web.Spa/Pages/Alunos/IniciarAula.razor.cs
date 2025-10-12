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
        private bool _carregando = true;
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
            _carregando = true;
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
                _mensagemErro = "Erro ao carregar dados. Tente novamente.";
                Snackbar.Add(_mensagemErro, Severity.Error);
                // Log: ex.Message, ex.StackTrace
            }
            finally
            {
                _carregando = false;
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
                .Where(cm => cm != null)
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

        private void OnCursoSelecionado(Guid cursoId)
        {
            _cursoSelecionadoId = cursoId;
        }

        #region aulas disponiveis
        private async Task ListarAulas()
        {
            
            RetornaAulasDaMatriculaMock(_cursoSelecionadoId);

        }

        private async Task StartClass(ProgressoMatricula itemClicado)
        {
            // Sua lógica para iniciar a aula vai aqui.
            // Por exemplo, navegar para outra página, salvar no banco, etc.
            Console.WriteLine($"Iniciando a aula: {itemClicado.TituloAula}");
            Snackbar.Add($"Iniciando a aula: {itemClicado.TituloAula}", Severity.Info);

            // Exemplo: Navegar para a página da aula
            // NavigationManager.NavigateTo($"/aula/{itemClicado.Id}");
        }
        private async Task CloseClass(ProgressoMatricula itemClicado)
        {
            // Sua lógica para iniciar a aula vai aqui.
            // Por exemplo, navegar para outra página, salvar no banco, etc.
            Console.WriteLine($"Finalizando a aula: {itemClicado.TituloAula}");
            Snackbar.Add($"Finalizando a aula: {itemClicado.TituloAula}", Severity.Info);

            // Exemplo: Navegar para a página da aula
            // NavigationManager.NavigateTo($"/aula/{itemClicado.Id}");
        }
        #endregion




        private void RetornaAulasDaMatriculaMock(Guid MatriculaId)
        {
            _progressoMatricula = new List<ProgressoMatricula>
            {
                new ProgressoMatricula
                {
                    id = Guid.NewGuid(),
                    MatriculaId = MatriculaId,
                    AulaId = Guid.NewGuid(),
                    TituloAula = "Introdução ao Curso",
                    DataInicio = DateTime.Now.AddDays(-10),
                    DataConclusao = DateTime.Now.AddDays(-9)
                },
                new ProgressoMatricula
                {
                    id = Guid.NewGuid(),
                    MatriculaId = MatriculaId,
                    AulaId = Guid.NewGuid(),
                    TituloAula = "Aula 1: Conceitos Básicos",
                    DataInicio = DateTime.Now.AddDays(-8),
                    DataConclusao = DateTime.Now.AddDays(-7)
                },
                new ProgressoMatricula
                {
                    id = Guid.NewGuid(),
                    MatriculaId = MatriculaId,
                    AulaId = Guid.NewGuid(),
                    TituloAula = "Aula 2: Ferramentas Essenciais",
                    DataInicio = DateTime.Now.AddDays(-6),
                    DataConclusao = null // Ainda não concluída
                },
                new ProgressoMatricula
                {
                    id = Guid.NewGuid(),
                    MatriculaId = MatriculaId,
                    AulaId = Guid.NewGuid(),
                    TituloAula = "Aula 3: Práticas Avançadas",
                    DataInicio = DateTime.MinValue, // Ainda não iniciada
                    DataConclusao = null
                }
            };
            foreach (var aula in _progressoMatricula)
            {
                Console.WriteLine($"Aula: {aula.TituloAula}, Início: {aula.DataInicio}, Conclusão: {aula.DataConclusao}");
            }

        }
        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }

    public class CursoMatriculado
    {
        public Guid CursoId { get; set; }
        public string? NomeCurso { get; set; }
        public string? DescricaoCurso { get; set; }
        public DateTime DataMatricula { get; set; }
        public DateTime? DataConclusao { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class ProgressoMatricula
    {
        public Guid id { get; set; }
        public Guid MatriculaId { get; set; }
        public Guid AulaId { get; set; }
        public String TituloAula { get; set; } 
        public DateTime DataInicio { get; set; }
        public DateTime? DataConclusao { get; set; }

    }

}