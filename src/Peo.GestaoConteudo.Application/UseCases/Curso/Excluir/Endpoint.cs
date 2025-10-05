using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Peo.Core.DomainObjects;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Web.Api;

namespace Peo.GestaoConteudo.Application.UseCases.Aula.Excluir;

public class Endpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/curso/{cursoId:guid}/aula/{aulaId:guid}", HandleDelete)
           .WithSummary("Excluir uma aula do curso")
           .RequireAuthorization(AccessRoles.Admin);
    }

    private static async Task<Results<NoContent, NotFound, BadRequest, BadRequest<Error>>>
        HandleDelete(Guid cursoId, Guid aulaId, IMediator mediator, ILogger<Endpoint> logger)
    {
        Result<Response> result;

        try
        {
            result = await mediator.Send(new Command(cursoId, aulaId));
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return TypedResults.BadRequest();
        }

        if (!result.IsSuccess)
        {
            // Quando o Handler retornar NotFound, devolve 404, caso contrário 400
            if (string.Equals(result.Error.Code, "NotFound", StringComparison.OrdinalIgnoreCase))
                return TypedResults.NotFound();

            return TypedResults.BadRequest(result.Error);
        }

        return TypedResults.NoContent();
    }
}
