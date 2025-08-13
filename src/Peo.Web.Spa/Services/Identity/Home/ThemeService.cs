using Microsoft.JSInterop;
using MudBlazor;

namespace Peo.Web.Spa.Services.Identity.Home;

public sealed class ThemeService : IDisposable
{
    private readonly IJSRuntime _js;
    private const string StorageKey = "theme:dark";

    public bool IsDark { get; private set; }

    public event Action<bool>? Changed;

    public ThemeService(IJSRuntime js) => _js = js;

    /// <summary>
    /// Inicializa o tema lendo do localStorage; se não houver valor salvo,
    /// usa a preferência do sistema via MudThemeProvider.
    /// </summary>
    public async Task InitializeAsync(MudThemeProvider provider)
    {
        string? saved = null;
        try
        {
            saved = await _js.InvokeAsync<string?>("localStorage.getItem", StorageKey);
        }
        catch { /* em WASM deve existir, mas ignore se estiver desabilitado */ }

        if (bool.TryParse(saved, out var dark))
            IsDark = dark;
        else
            IsDark = await provider.GetSystemDarkModeAsync();

        Changed?.Invoke(IsDark);
    }

    public async Task SetAsync(bool dark)
    {
        if (IsDark == dark) return;
        IsDark = dark;
        try
        {
            await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, dark.ToString().ToLowerInvariant());
        }
        catch { /* ignore se falhar */ }

        Changed?.Invoke(IsDark);
    }

    public Task ToggleAsync() => SetAsync(!IsDark);

    public void Dispose()
    {
        // só para permitir unsubscribe seguro via -=
        Changed = null;
    }
}
