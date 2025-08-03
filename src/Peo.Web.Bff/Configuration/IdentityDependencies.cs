using Peo.Web.Bff.Services.Identity;
using Peo.Web.Bff.Services.Identity.Dtos;

namespace Peo.Web.Bff.Configuration
{
    public static class IdentityDependencies
    {
        public static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IdentityService>();

            services.AddHttpClient<IdentityService>(c =>
                c.BaseAddress = new Uri(configuration.GetValue<string>("Endpoints:Identity")!));

            return services;
        }

        public static WebApplication AddIdentityEndpoints(this WebApplication app)
        {
            app.MapPost("/v1/identity/register", async (RegisterRequest request, IdentityService service, CancellationToken ct) =>
            {
                return await service.RegisterAsync(request, ct);
            });

            app.MapPost("/v1/identity/login", async (LoginRequest request, IdentityService service, CancellationToken ct) =>
            {
                return await service.LoginAsync(request, ct);
            });

            app.MapPost("/v1/identity/refresh-token", async (RefreshTokenRequest request, IdentityService service, CancellationToken ct) =>
            {
                return await service.RefreshTokenAsync(request, ct);
            });

            return app;
        }
    }
}