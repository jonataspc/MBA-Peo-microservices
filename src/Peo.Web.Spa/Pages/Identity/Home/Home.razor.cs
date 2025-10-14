using MudBlazor;
using Peo.Web.Spa.Services;

namespace Peo.Web.Spa.Pages.Identity.Home
{
    public partial class Home
    {
        private string? UserName;
        private IEnumerable<HistoricoCursoCompletoResponse> _cursos = [];
        private bool _isLoading = true;
        private CancellationTokenSource? _cts;

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            UserName = user.Identity?.Name ?? "Usuário";

            await CarregarCursosMatriculados();
        }

        private async Task CarregarCursosMatriculados()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            try
            {
                _isLoading = true;
                StateHasChanged();

                var response = await Api.ObterHistoricoCompletoCursosAsync(_cts.Token);
                _cursos = response?.Historico.Where(c => c.Status != "PendentePagamento") ?? Enumerable.Empty<HistoricoCursoCompletoResponse>();
            }
            catch (ApiException ex)
            {
                Snackbar.Add($"Erro ao carregar cursos: {ex.Message}", Severity.Error);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Erro inesperado: {ex.Message}", Severity.Error);
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        private void IniciarOuContinuarCurso(Guid matriculaId)
        {
            Navigation.NavigateTo($"/alunosaulas");
        }

        private static Color GetProgressColor(int progress)
        {
            return progress switch
            {
                >= 80 => Color.Success,
                >= 50 => Color.Warning,
                _ => Color.Error
            };
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}