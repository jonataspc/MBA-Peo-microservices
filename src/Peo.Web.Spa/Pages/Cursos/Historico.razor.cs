using Microsoft.AspNetCore.Components;
using MudBlazor;
using Peo.Web.Spa.Services;

namespace Peo.Web.Spa.Pages.Cursos
{
    public partial class Historico : IDisposable
    {
        private IEnumerable<HistoricoCursoCompletoResponse> _historicoLista = new List<HistoricoCursoCompletoResponse>();
        [Inject] WebApiClient Api { get; set; } = null!;
        [Inject] IDialogService DialogService { get; set; } = null!;
        [Inject] ISnackbar Snackbar { get; set; } = null!;
        private CancellationTokenSource? _cts;

        protected override async Task OnInitializedAsync()
        {
            await ObterHistorico();
        }

        private async Task ObterHistorico()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            try
            {
                var resp = await Api.ObterHistoricoCompletoCursosAsync(_cts.Token);
                _historicoLista = resp?.Historico ?? Enumerable.Empty<HistoricoCursoCompletoResponse>();
            }
            catch (ApiException ex)
            {
                Snackbar.Add($"Falha ao listar histórico: {ex.Message}", Severity.Error);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Erro inesperado: {ex.Message}", Severity.Error);
            }
        }

        private async Task VerDetalhes(HistoricoCursoCompletoResponse historico)
        {
            var mensagem = $@"
<strong>Curso:</strong> {historico.NomeCurso}<br/>
<strong>Descrição:</strong> {historico.DescricaoCurso ?? "Não informada"}<br/>
<strong>Instrutor:</strong> {historico.InstrutorNome}<br/>
<strong>Aluno:</strong> {historico.Aluno}<br/>
<strong>Data Matrícula:</strong> {historico.DataMatricula:dd/MM/yyyy}<br/>
<strong>Data Conclusão:</strong> {(historico.DataConclusao.HasValue ? historico.DataConclusao.Value.ToString("dd/MM/yyyy") : "Em andamento")}<br/>
<strong>Status:</strong> {historico.Status}<br/>
<strong>Progresso:</strong> {historico.PercentualProgresso}%
";

            var parameters = new DialogParameters
            {
                { "ContentText", new MarkupString(mensagem) },
                { "ButtonText", "Fechar" },
                { "Color", Color.Primary }
            };

            var options = new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true
            };

            await DialogService.ShowMessageBox("Detalhes do Curso", new MarkupString(mensagem), "Fechar", null, null, options);
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}
