using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Peo.Web.Spa;
using Peo.Web.Spa.Services;
using Peo.Web.Spa.Services.Identity.Home;
using Peo.Web.Spa.Services.Identity.Login;
using Peo.Web.Spa.Services.Identity.Login.Interface;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// MudBlazor + Snackbar config
builder.Services.AddMudServices(cfg =>
{
    cfg.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.TopRight;
    cfg.SnackbarConfiguration.PreventDuplicates = true;
    cfg.SnackbarConfiguration.NewestOnTop = true;
    cfg.SnackbarConfiguration.ShowCloseIcon = true;
    cfg.SnackbarConfiguration.VisibleStateDuration = 4000;
    cfg.SnackbarConfiguration.HideTransitionDuration = 150;
    cfg.SnackbarConfiguration.ShowTransitionDuration = 150;
});

// ThemeService para lembrar dark/light
builder.Services.AddScoped<ThemeService>();

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthStateProvider>();

//builder.Services.AddScoped<AuthHeaderHandler>();
builder.Services.AddTransient<AuthHeaderHandler>();

var apiBase = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7276/";

builder.Services.AddHttpClient("Api", c => c.BaseAddress = new Uri(apiBase))
        .AddHttpMessageHandler<AuthHeaderHandler>();

// >>> registre o serviço usado pela página Aulas <<<
builder.Services.AddScoped<AulasService>();

builder.Services.AddScoped(sp =>
{
    var http = sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api");
    return new WebApiClient(apiBase, http); // passa baseUrl e httpClient
});

builder.Services.AddScoped<IAuthService, WebApiClientAuthAdapter>();
builder.Services.AddScoped<ITokenStore, TokenStoreLocalStorage>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
