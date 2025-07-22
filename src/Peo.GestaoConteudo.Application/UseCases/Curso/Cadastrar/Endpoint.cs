using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using MiniValidation;
using Peo.Core.DomainObjects;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Web.Api;

namespace Peo.GestaoConteudo.Application.UseCases.Curso.Cadastrar
{
    public class Endpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("/curso/", HandleCreate)
               .WithSummary("Cadastrar um novo curso")
               .RequireAuthorization(AccessRoles.Admin);
        }

        private static async Task<Results<Ok<Response>, ValidationProblem, BadRequest, BadRequest<Error>>> HandleCreate(Command command, IMediator mediator, ILogger<Endpoint> logger)
        {
            if (!MiniValidator.TryValidate(command, out var errors))
            {
                return TypedResults.ValidationProblem(errors);
            }

            Result<Response> result;

            try
            {
                result = await mediator.Send(command);
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