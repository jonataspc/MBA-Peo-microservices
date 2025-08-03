using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Peo.Web.Bff.Services.GestaoAlunos;
using Peo.Web.Bff.Services.GestaoAlunos.Dtos;
using Peo.Web.Bff.Services.Handlers;
using Peo.Web.Bff.Services.Helpers;

namespace Peo.Web.Bff.Configuration
{
    public static class GestaoAlunosDependencies
    {
        public static IServiceCollection AddGestaoAlunos(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<GestaoAlunosService>();

            services.AddHttpClient<GestaoAlunosService>(c =>
                c.BaseAddress = new Uri(configuration.GetValue<string>("Endpoints:GestaoAlunos")!))
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AddPolicyHandler(PollyExtensions.WaitAndRetry())
                .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

            return services;
        }

        public static WebApplication AddGestaoAlunosEndpoints(this WebApplication app)
        {
            // Matricula endpoints
            app.MapPost("/v1/estudante/matricula/", async (MatriculaCursoRequest request, GestaoAlunosService service, CancellationToken ct) =>
            {
                return await service.MatricularCursoAsync(request, ct);
            });

            app.MapPost("/v1/estudante/matricula/pagamento", async (PagamentoMatriculaRequest request, GestaoAlunosService service, CancellationToken ct) =>
            {
                return await service.PagarMatriculaAsync(request, ct);
            });

            app.MapPost("/v1/estudante/matricula/concluir", async (ConcluirMatriculaRequest request, GestaoAlunosService service, CancellationToken ct) =>
            {
                return await service.ConcluirMatriculaAsync(request, ct);
            });

            // Aula endpoints
            app.MapPost("/v1/estudante/matricula/aula/iniciar", async (IniciarAulaRequest request, GestaoAlunosService service, CancellationToken ct) =>
            {
                return await service.IniciarAulaAsync(request, ct);
            });

            app.MapPost("/v1/estudante/matricula/aula/concluir", async (ConcluirAulaRequest request, GestaoAlunosService service, CancellationToken ct) =>
            {
                return await service.ConcluirAulaAsync(request, ct);
            });

            // Certificados endpoint
            app.MapGet("/v1/estudante/certificados", async (GestaoAlunosService service, CancellationToken ct) =>
            {
                return await service.ObterCertificadosAsync(ct);
            });

            return app;
        }
    }
} 