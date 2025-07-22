using Microsoft.Extensions.DependencyInjection;
using Peo.Core.Interfaces.Services.Acls;
using Peo.Identity.Application.Services;

namespace Peo.Identity.Application.DiConfig
{
    public static class DependenciesSetup
    {
        public static IServiceCollection AddServicesForIdentity(this IServiceCollection services)
        {
            // Anti-corruption layers
            services.AddScoped<IDetalhesUsuarioService, UserService>();
            return services;
        }
    }
}