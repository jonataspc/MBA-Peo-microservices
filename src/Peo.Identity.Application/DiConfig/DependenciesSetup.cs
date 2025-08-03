using Microsoft.Extensions.DependencyInjection;
using Peo.Core.Interfaces.Services.Acls;
using Peo.Identity.Application.Services;
using System.Reflection;

namespace Peo.Identity.Application.DiConfig
{
    public static class DependenciesSetup
    {
        public static IServiceCollection AddServicesForIdentity(this IServiceCollection services)
        {
            // Mediator
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            // Anti-corruption layers
            services.AddScoped<IDetalhesUsuarioService, UserService>();
            return services;
        }
    }
}