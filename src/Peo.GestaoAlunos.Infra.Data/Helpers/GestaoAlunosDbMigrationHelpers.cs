using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Peo.GestaoAlunos.Infra.Data.Contexts;

namespace Peo.GestaoAlunos.Infra.Data.Helpers
{
    public static class GestaoAlunosDbMigrationHelpers
    {
        public static async Task UseGestaoAlunosDbMigrationHelperAsync(this WebApplication app)
        {
            await EnsureSeedDataAsync(app);
        }

        private static async Task EnsureSeedDataAsync(WebApplication serviceScope)
        {
            var services = serviceScope.Services.CreateScope().ServiceProvider;
            await EnsureSeedDataAsync(services);
        }

        private static async Task EnsureSeedDataAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>()
                                             .CreateScope();
            var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();

            // Allow migrations in Development or when explicitly enabled via environment variable
            var enableMigrations = env.IsDevelopment() || 
                                   Environment.GetEnvironmentVariable("ENABLE_MIGRATIONS")?.ToLowerInvariant() == "true";

            if (enableMigrations)
            {
                var context = scope.ServiceProvider.GetRequiredService<GestaoAlunosContext>();

                await context.Database.MigrateAsync();
            }
        }
    }
}