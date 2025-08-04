using Peo.Core.Interfaces.Services;
using Peo.Core.Web.Services;

namespace Peo.Web.Bff.Configuration
{
    public static class Dependencies
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IAppIdentityUser, AppIdentityUser>();
            return services;
        }
    }
}