using Peo.Web.Bff.Services.Historico;
using Peo.Web.Bff.Services.Historico.Dtos;

namespace Peo.Web.Bff.Configuration
{
    public static class HistoricoDependencies
    {
        public static IServiceCollection AddHistorico(this IServiceCollection services)
        {
            services.AddScoped<HistoricoService>();
            
            return services;
        }

        public static WebApplication AddHistoricoEndpoints(this WebApplication app)
        {
            var endpoints = app
                .MapGroup("v1/historico")
                .WithTags("Histórico")
                .RequireAuthorization();

            endpoints.MapGet("/cursos-completo",
                    async (HistoricoService service, CancellationToken ct) =>
                    {
                        return await service.ObterHistoricoCompletoCursosAsync(ct);
                    })
                .WithName("ObterHistoricoCompletoCursos")
                .Produces<ObterHistoricoCompletoCursosResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces<object>(StatusCodes.Status400BadRequest);

            return app;
        }
    }
}