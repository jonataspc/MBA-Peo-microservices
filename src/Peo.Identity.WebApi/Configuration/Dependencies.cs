using Microsoft.AspNetCore.Identity;
using Peo.Core.Communication.Mediator;
using Peo.Core.Dtos;
using Peo.Core.Interfaces.Services;
using Peo.Core.Web.Services;
using Peo.Identity.Application.DiConfig;
using Peo.Identity.Application.Extensions;
using Peo.Identity.Application.Services;
using Peo.Identity.Domain.Interfaces.Data;
using Peo.Identity.Domain.Interfaces.Services;
using Peo.Identity.Infra.Data.Contexts;
using Peo.Identity.Infra.Data.DiConfig;
using Peo.Identity.Infra.Data.Repositories;

namespace Peo.Identity.WebApi.Configuration
{
    public static class Dependencies
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            services.AddDataDependenciesForIdentity(configuration, hostEnvironment)
                    .AddServicesForIdentity()
                    .AddIdentity()
                    .AddAppSettings(configuration)
                    .AddMediator()
                    .AddApiServices();

            return services;
        }

        private static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IAppIdentityUser, AppIdentityUser>();
            return services;
        }

        private static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services.AddIdentityCore<IdentityUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = false;

                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
            })
                     .AddRoles<IdentityRole>()
                     .AddEntityFrameworkStores<IdentityContext>()
                     .AddApiEndpoints();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }

        private static IServiceCollection AddAppSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Position));
            return services;
        }

        private static IServiceCollection AddMediator(this IServiceCollection services)
        {
            services.AddScoped<IMediatorHandler, MediatorHandler>();
            return services;
        }

        public static WebApplication MapEndpoints(this WebApplication app)
        {
            app.MapIdentityEndpoints();
            return app;
        }
    }
}