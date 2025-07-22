using Peo.Core.DomainObjects;
using Peo.Web.Api.Middlewares;
using System.Text.Json.Serialization;

namespace Peo.Web.Api.Configuration
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
                    builder.WithOrigins(allowedOrigins.ToArray())
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
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

            // Middlares
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

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