
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Peo.Core.Web.Api;

namespace Peo.GestaoConteudo.Application.UseCases.Curso.Atualizar
{
    public class Endpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPut("cursos/{id}", async (Guid id, AtualizarCursoCommand command, IMediator mediator) =>
            {
                command.Id = id;
                var result = await mediator.Send(command);
                return Results.Ok(result);
            })
            .WithName("AtualizarCurso")
            .WithSummary("Atualiza um curso existente");
        }
    }
}
