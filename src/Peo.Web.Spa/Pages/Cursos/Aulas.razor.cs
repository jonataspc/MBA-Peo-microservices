using Microsoft.AspNetCore.Components;
using MudBlazor;
using Peo.Web.Spa.Services;

namespace Peo.Web.Spa.Pages.Cursos
{
    public partial class Aulas : ComponentBase
    {
        private IEnumerable<AulaResponse> _aulasLista = new List<AulaResponse>();
        [Inject] WebApiClient Api { get; set; } = null!;
        [Inject] IDialogService DialogService { get; set; } = null!;
        [Inject] ISnackbar Snackbar { get; set; } = null!;

        private CancellationTokenSource? _cts;

        protected override async Task OnInitializedAsync()
        {
            await ObterAulas();
        }

        private async Task AdicionaAula()
        {
            var curusoId = new Guid();
            var request = new AulaRequest();
            var resp = await Api.PostV1ConteudoCursoAulaAsync(curusoId,request);
        }

        private async Task ObterAulas()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            var cursoId = Guid.NewGuid();
            try
            {
                var resp = await Api.GetV1ConteudoCursoAulaAsync(cursoId,_cts.Token);
                _aulasLista = resp?.Aulas ?? Enumerable.Empty<AulaResponse>();
            }
            catch (ApiException ex) { Snackbar.Add($"Falha ao listar: {ex.Message}", Severity.Error); }

        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}
