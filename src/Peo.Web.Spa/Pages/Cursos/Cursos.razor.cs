using Microsoft.AspNetCore.Components;
using MudBlazor;
using Peo.Web.Spa.Services;
using System.Globalization;


namespace Peo.Web.Spa.Pages.Cursos
{
    public partial class Cursos
    {
        private IEnumerable<Curso> _cursosLista = new List<Curso>();
        [Inject] WebApiClient Api { get; set; } = null!;
        [Inject] IDialogService DialogService { get; set; } = null!;
        [Inject] ISnackbar Snackbar { get; set; } = null!;
        private CancellationTokenSource? _cts;
        public CultureInfo _pt = CultureInfo.GetCultureInfo("pt-BR");

        protected override async Task OnInitializedAsync()
        {
            await ObterCursos();            
        }

        private async Task AdicionarCurso()
        {
            var options = new DialogOptions {
                CloseButton = true,        
                CloseOnEscapeKey = true,   
                BackdropClick = false,  
                FullWidth = true,          
                MaxWidth = MaxWidth.Medium  }; 
            var response = await DialogService.ShowAsync<AdicionarCursos>("Adicionar_Cursos", options);
            await response.Result;
            await ObterCursos();
        }

        private async Task ObterCursos()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            try
            {
                var resp = await Api.GetV1ConteudoCursoAsync(_cts.Token);
                _cursosLista = resp?.Cursos ?? Enumerable.Empty<Curso>();
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
