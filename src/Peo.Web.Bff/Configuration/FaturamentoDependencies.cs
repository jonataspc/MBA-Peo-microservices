using Peo.Web.Bff.Services.Faturamento;
using Peo.Web.Bff.Services.Faturamento.Dtos;
using Peo.Web.Bff.Services.Handlers;
using Peo.Web.Bff.Services.Helpers;
using Polly;

namespace Peo.Web.Bff.Configuration
{
    public static class FaturamentoDependencies
    {
        public static IServiceCollection AddFaturamento(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<FaturamentoService>();
            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

            services.AddHttpClient<FaturamentoService>(c =>
                c.BaseAddress = new Uri(configuration.GetValue<string>("Endpoints:Faturamento")!))
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AddPolicyHandler(PollyExtensions.WaitAndRetry())
                .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

            return services;
        }

        public static WebApplication AddFaturamentoEndpoints(this WebApplication app)
        {
            var endpoints = app
            .MapGroup("v1/faturamento")
            .WithTags("Faturamento");

            endpoints.MapPost("/pagamento", async (EfetuarPagamentoRequest request, FaturamentoService service, CancellationToken ct) =>
            {
                return await service.EfetuarPagamentoAsync(request, ct);
            });

            return app;
        }
    }
}