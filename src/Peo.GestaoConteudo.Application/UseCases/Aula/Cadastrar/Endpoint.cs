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

namespace Peo.GestaoConteudo.Application.UseCases.Aula.Cadastrar
{
    public class Endpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("/curso/{id:guid}/aula", HandleCreate)
             .WithSummary("Cadastrar uma nova aula para o curso")
             .RequireAuthorization(AccessRoles.Admin);
        }

        private static async Task<Results<Ok<Response>, ValidationProblem, BadRequest, BadRequest<Error>>> HandleCreate(Guid id, Command command, IMediator mediator, ILogger<Endpoint> logger)
        {
            command.CursoId = id;

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