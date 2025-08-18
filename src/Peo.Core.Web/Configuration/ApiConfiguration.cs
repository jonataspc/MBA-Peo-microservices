using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Peo.Core.DomainObjects;
using System.Text.Json.Serialization;

namespace Peo.Core.Web.Configuration
{
    public static class ApiConfiguration
    {
        public static IServiceCollection SetupWebApi(this IServiceCollection services, IConfiguration configuration)
        {
            var allowedOrigins = configuration.GetRequiredSection("Security:CorsPolicy:AllowedOrigins").Get<List<string>>()!;

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
#if DEBUG
                    // Necessario devido ao bug ao debugar Blazor com Aspire (https://github.com/dotnet/aspire/issues/5819)
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();

#else
                    builder.WithOrigins(allowedOrigins.ToArray())
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
#endif
                });
            });

            // Adiciona serviços da API
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

            return services;
        }

        public static IServiceCollection AddPolicies(this IServiceCollection services)
        {
            services.AddAuthorizationBuilder()
                    .AddPolicy(AccessRoles.Admin, policy =>
                        policy.RequireRole(AccessRoles.Admin)
                        )
                    .AddPolicy(AccessRoles.Aluno, policy =>
                        policy.RequireRole(AccessRoles.Aluno)
                        );

            return services;
        }
    }
}