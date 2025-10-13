using Peo.Web.Bff.Services.GestaoConteudo;
using Peo.Web.Bff.Services.GestaoConteudo.Dtos;
using Peo.Web.Bff.Services.Handlers;
using Peo.Web.Bff.Services.Helpers;
using Polly;

namespace Peo.Web.Bff.Configuration
{
    public static class GestaoConteudoDependencies
    {
        public static IServiceCollection AddGestaoConteudo(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<GestaoConteudoService>();
            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

            services.AddHttpClient<GestaoConteudoService>(c =>
                c.BaseAddress = new Uri(configuration.GetValue<string>("Endpoints:GestaoConteudo")!))
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AddPolicyHandler(PollyExtensions.WaitAndRetry())
                .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

            return services;
        }

        public static WebApplication AddGestaoConteudoEndpoints(this WebApplication app)
        {
            var endpoints = app
            .MapGroup("v1/conteudo")
            .WithTags("Conteúdo");

            endpoints.MapPost("/curso/", async (CursoRequest request, GestaoConteudoService service, CancellationToken ct) =>
            {
                return await service.CadastrarCursoAsync(request, ct);
            });

            endpoints.MapGet("/curso/", async (GestaoConteudoService service, CancellationToken ct) =>
            {
                return await service.ObterTodosCursosAsync(ct);
            });

            endpoints.MapGet("/curso/{id:guid}", async (Guid id, GestaoConteudoService service, CancellationToken ct) =>
            {
                return await service.ObterCursoPorIdAsync(id, ct);
            });

            endpoints.MapGet("/curso/{cursoId:guid}/aula", async (Guid cursoId, GestaoConteudoService service, CancellationToken ct) =>
            {
                return await service.ObterAulasDoCursoAsync(cursoId, ct);
            });

            endpoints.MapPost("/curso/{cursoId:guid}/aula", async (Guid cursoId, AulaRequest request, GestaoConteudoService service, CancellationToken ct) =>
            {
                return await service.CadastrarAulaAsync(cursoId, request, ct);
            });

            endpoints.MapDelete("/curso/{cursoId:guid}/aula/{aulaId:guid}",
                async (Guid cursoId, Guid aulaId, GestaoConteudoService service, CancellationToken ct) =>
                    await service.ExcluirAulaAsync(cursoId, aulaId, ct));

            return app;
        }
    }
}