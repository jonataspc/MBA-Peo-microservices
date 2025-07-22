using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Peo.Core.DomainObjects;
using Peo.Identity.Infra.Data.Contexts;
using System.Diagnostics;

namespace Peo.Identity.Infra.Data.Helpers
{
    public static class IdentityDbMigrationHelpers
    {
        public static async Task UseIdentityDbMigrationHelperAsync(this WebApplication app)
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
                var context = scope.ServiceProvider.GetRequiredService<IdentityContext>();

                await context.Database.MigrateAsync();
                await SeedAspNetIdentity(scope.ServiceProvider);
            }
        }

        private static async Task SeedAspNetIdentity(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            try
            {
                if (!await roleManager.RoleExistsAsync(AccessRoles.Admin))
                {
                    await roleManager.CreateAsync(new IdentityRole(AccessRoles.Admin));
                }

                if (!await roleManager.RoleExistsAsync(AccessRoles.Aluno))
                {
                    await roleManager.CreateAsync(new IdentityRole(AccessRoles.Aluno));
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
    }
}