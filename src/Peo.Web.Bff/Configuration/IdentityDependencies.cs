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
                var result = await service.RegisterAsync(request, ct);
                return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
            });

            return app;
        }
    }
}