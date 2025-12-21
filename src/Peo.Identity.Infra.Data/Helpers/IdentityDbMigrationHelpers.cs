using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Peo.Core.DomainObjects;
using Peo.Core.Entities;
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

            // Allow migrations in Development or when explicitly enabled via environment variable
            var enableMigrations = env.IsDevelopment() || 
                                   Environment.GetEnvironmentVariable("ENABLE_MIGRATIONS")?.ToLowerInvariant() == "true";

            if (enableMigrations)
            {
                var context = scope.ServiceProvider.GetRequiredService<IdentityContext>();

                await context.Database.MigrateAsync();
                await SeedAspNetIdentity(scope.ServiceProvider, context);
            }
        }

        private static async Task SeedAspNetIdentity(IServiceProvider serviceProvider, IdentityContext context)
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

            await AddAdminUserAsync(serviceProvider, context);
        }

        private static async Task AddAdminUserAsync(IServiceProvider serviceProvider, IdentityContext context)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var adminUser = new IdentityUser
            {
                UserName = "admin@admin.com",
                Email = "admin@admin.com",
            };

            if (await userManager.Users.AnyAsync(u => u.UserName == adminUser.UserName))
            {
                return;
            }

            // ONLY FOR DEMO PURPOSES
            // CHANGE THE PASSWORD BEFORE DEPLOYING TO PRODUCTION !!!
            var result = await userManager.CreateAsync(adminUser, "@dmin!");

            if (result.Succeeded)
            {
                await userManager.ConfirmEmailAsync(adminUser, await userManager.GenerateEmailConfirmationTokenAsync(adminUser));
                await userManager.AddToRoleAsync(adminUser, AccessRoles.Aluno);
                await userManager.AddToRoleAsync(adminUser, AccessRoles.Admin);

                context.ApplicationUsers.Add(new Usuario(Guid.Parse(adminUser.Id), "Administrador do Sistema", adminUser.Email));
                await context.SaveChangesAsync();
            }
        }
    }
}