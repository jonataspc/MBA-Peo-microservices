using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Peo.Core.DomainObjects;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Web.Api;
using Peo.GestaoAlunos.Application.Dtos.Responses;
using Peo.GestaoAlunos.Application.Queries.ObterMatriculas;

namespace Peo.GestaoAlunos.WebApi.Endpoints.Aluno
{
    public class EndpointObterMatriculas : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/matricula/", Handle)
              .WithSummary("Consultar matrículas do aluno")
              .RequireAuthorization(AccessRoles.Aluno);
        }

        private static async Task<Results<Ok<IEnumerable<MatriculaResponse>>, ValidationProblem, BadRequest<Error>>> Handle(IMediator mediator, CancellationToken cancellationToken)
        {
            var command = new ObterMatriculasQuery();
            var response = await mediator.Send(command, cancellationToken);

            if (response.IsSuccess)
            {
                return TypedResults.Ok(response.Value);
            }

            return TypedResults.BadRequest(response.Error);
        }
    }
}