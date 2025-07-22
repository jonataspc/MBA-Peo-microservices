using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Peo.GestaoConteudo.Infra.Data.Contexts;

namespace Peo.GestaoConteudo.Infra.Data.Helpers
{
    public static class GestaoConteudoDbMigrationHelpers
    {
        public static async Task UseGestaoConteudoDbMigrationHelperAsync(this WebApplication app)
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

            if (env.IsDevelopment())
            {
                var context = scope.ServiceProvider.GetRequiredService<GestaoConteudoContext>();

                await context.Database.MigrateAsync();
            }
        }
    }
}