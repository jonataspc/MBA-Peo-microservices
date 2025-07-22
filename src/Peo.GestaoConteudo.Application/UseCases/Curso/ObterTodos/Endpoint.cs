using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Peo.Core.DomainObjects;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Web.Api;

namespace Peo.GestaoConteudo.Application.UseCases.Curso.ObterTodos
{
    public class Endpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/curso/", HandleGetAll)
               .WithSummary("Obter todos os cursos")
               .RequireAuthorization(AccessRoles.Admin);
        }

        private static async Task<Results<Ok<Response>, ValidationProblem, BadRequest, BadRequest<Error>>> HandleGetAll(IMediator mediator, ILogger<Endpoint> logger)
        {
            var query = new Query();

            Result<Response> result;

            try
            {
                result = await mediator.Send(query);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                return TypedResults.BadRequest();
            }

            if (!result.IsSuccess)
            {
                return TypedResults.BadRequest(result.Error);
            }

            return TypedResults.Ok(result.Value);
        }
    }
}