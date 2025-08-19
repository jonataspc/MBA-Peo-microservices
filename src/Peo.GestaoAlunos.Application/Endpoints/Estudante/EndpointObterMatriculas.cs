using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Peo.Core.DomainObjects;
using Peo.Core.Web.Api;
using Peo.GestaoAlunos.Application.Dtos.Responses;
using Peo.GestaoAlunos.Application.Queries.ObterMatriculas;

namespace Peo.GestaoAlunos.Application.Endpoints.Estudante
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