using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Interfaces.Services.Acls;
using Peo.GestaoConteudo.Application.Services;
using Peo.GestaoConteudo.Application.UseCases.Aula.Cadastrar;

namespace Peo.GestaoConteudo.Application.DiConfig
{
    public static class DependenciesSetup
    {
        public static IServiceCollection AddServicesForGestaoConteudo(this IServiceCollection services)
        {
            // Mediator
            services.AddMediatR(x =>
            {
                x.RegisterServicesFromAssembly(typeof(CursoAulaService).Assembly);
            });

            // Handlers
            services.AddScoped<IRequestHandler<UseCases.Curso.Cadastrar.Command, Result<UseCases.Curso.Cadastrar.Response>>, UseCases.Curso.Cadastrar.Handler>();
            services.AddScoped<IRequestHandler<UseCases.Curso.ObterPorId.Query, Result<UseCases.Curso.ObterPorId.Response>>, UseCases.Curso.ObterPorId.Handler>();
            services.AddScoped<IRequestHandler<UseCases.Curso.ObterTodos.Query, Result<UseCases.Curso.ObterTodos.Response>>, UseCases.Curso.ObterTodos.Handler>();
            services.AddScoped<IRequestHandler<UseCases.Aula.ObterTodos.Query, Result<UseCases.Aula.ObterTodos.Response>>, UseCases.Aula.ObterTodos.Handler>();
            services.AddScoped<IRequestHandler<UseCases.Aula.Cadastrar.Command, Result<Response>>, Handler>();

            // Anti-corruption layers
            services.AddScoped<ICursoAulaService, CursoAulaService>();

            return services;
        }
    }
}