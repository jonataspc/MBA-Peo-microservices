using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Peo.Core.Web.Extensions;

namespace Peo.GestaoConteudo.Application
{
    public static class CursoEndpoints
    {
        public static void MapCursoEndpoints(this IEndpointRouteBuilder app)
        {
            var endpoints = app
            .MapGroup("");

            endpoints.MapGroup("v1/conteudo")
            .WithTags("Conteúdo")
            .MapEndpoint<UseCases.Curso.Cadastrar.Endpoint>()
            .MapEndpoint<UseCases.Curso.ObterPorId.Endpoint>()
            .MapEndpoint<UseCases.Curso.ObterTodos.Endpoint>()
            .MapEndpoint<UseCases.Curso.ObterConteudoProgramatico.Endpoint>()
            .MapEndpoint<UseCases.Aula.ObterTodos.Endpoint>()
            .MapEndpoint<UseCases.Aula.Cadastrar.Endpoint>()
            .MapEndpoint<UseCases.Curso.Atualizar.Endpoint>()
        }
    }
}
