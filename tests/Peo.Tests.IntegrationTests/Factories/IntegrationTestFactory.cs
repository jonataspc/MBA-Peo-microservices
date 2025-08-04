using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Peo.Core.Interfaces.Data;
using Peo.GestaoAlunos.Domain.Interfaces;
using Peo.GestaoAlunos.Infra.Data.Contexts;
using Peo.GestaoAlunos.Infra.Data.Repositories;
using Peo.GestaoConteudo.Domain.Entities;
using Peo.GestaoConteudo.Infra.Data.Contexts;
using Peo.GestaoConteudo.Infra.Data.Repositories;
using Peo.Identity.Application.Services;
using Peo.Identity.Domain.Interfaces.Data;
using Peo.Identity.Domain.Interfaces.Services;
using Peo.Identity.Infra.Data.Contexts;
using Peo.Identity.Infra.Data.Repositories;
using Testcontainers.RabbitMq;

namespace Peo.Tests.IntegrationTests.Factories
{
    public class IntegrationTestFactory<TProgram> : WebApplicationFactory<TProgram>, IAsyncLifetime
    where TProgram : class
    {
 

        private readonly RabbitMqContainer _rabbitMqContainer = null!;

        public IntegrationTestFactory()
        {
            _rabbitMqContainer = new RabbitMqBuilder()
                  .WithImage("rabbitmq:3.11")
                  .Build();
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureHostConfiguration(config =>
            {
            });

            return base.CreateHost(builder);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {


                services.AddDbContext<GestaoConteudoContext>(options =>
                {
                    options.UseSqlite("Data Source=Peo.db");
                    options.UseLazyLoadingProxies();
                    options.EnableDetailedErrors();
                    options.EnableSensitiveDataLogging();
                });


                services.AddDbContext<GestaoEstudantesContext>(options =>
                {
                    options.UseSqlite("Data Source=Peo.db");
                    options.UseLazyLoadingProxies();
                    options.EnableDetailedErrors();
                    options.EnableSensitiveDataLogging();
                });

                services.AddScoped<IEstudanteRepository, EstudanteRepository>();


                services.AddDbContext<IdentityContext>(options =>
                {
                    options.UseSqlite("Data Source=Peo.db");
                    options.UseLazyLoadingProxies();
                    options.EnableDetailedErrors();
                    options.EnableSensitiveDataLogging();
                });

                services.AddScoped<IUserRepository, UserRepository>();
                services.AddScoped<IRepository<Curso>, CursoRepository>();

                //// 
                ///

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

                   
                string connectionString;
                 
            });
        }

        public async Task InitializeAsync()
        {
            await _rabbitMqContainer.StartAsync();

            Environment.SetEnvironmentVariable("ConnectionStrings__messaging", _rabbitMqContainer.GetConnectionString());


            using var scope = Services.CreateScope();
           
            await  scope.ServiceProvider.GetRequiredService<GestaoEstudantesContext>().Database.MigrateAsync();

            await scope.ServiceProvider.GetRequiredService<IdentityContext>().Database.MigrateAsync();


             
        }
         

        public new async Task DisposeAsync()
        {
            await _rabbitMqContainer.DisposeAsync().AsTask();
        }
    }
}