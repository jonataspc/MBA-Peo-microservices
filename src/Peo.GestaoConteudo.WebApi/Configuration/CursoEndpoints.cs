using Peo.Core.Web.Extensions;

namespace Peo.GestaoConteudo.WebApi.Configuration
{
    public static class CursoEndpoints
    {
        public static void MapCursoEndpoints(this IEndpointRouteBuilder app)
        {
            var endpoints = app
            .MapGroup("");

            endpoints.MapGroup("v1/conteudo")
            .WithTags("Conteúdo")
            .MapEndpoint<Application.UseCases.Curso.Cadastrar.Endpoint>()
            .MapEndpoint<Application.UseCases.Curso.ObterPorId.Endpoint>()
            .MapEndpoint<Application.UseCases.Curso.ObterTodos.Endpoint>()
            .MapEndpoint<Application.UseCases.Curso.ObterConteudoProgramatico.Endpoint>()
            .MapEndpoint<Application.UseCases.Aula.ObterTodos.Endpoint>()
            .MapEndpoint<Application.UseCases.Aula.Cadastrar.Endpoint>()
            .MapEndpoint<Application.UseCases.Curso.Atualizar.Endpoint>();
        }
    }
}