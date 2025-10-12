using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Peo.Core.DomainObjects;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Web.Api;
using Peo.GestaoAlunos.Application.Dtos.Responses;
using Peo.GestaoAlunos.Application.Queries.ObterAulasMatricula;

namespace Peo.GestaoAlunos.WebApi.Endpoints.Aluno
{
    public class EndpointObterAulasMatricula : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/matricula/{id:guid}/aulas", Handle)
              .WithSummary("Consultar aulas da matrícula")
              .RequireAuthorization(AccessRoles.Aluno);
        }

        private static async Task<Results<Ok<IEnumerable<AulaMatriculaResponse>>, ValidationProblem, BadRequest<Error>>> Handle(Guid id, IMediator mediator, CancellationToken cancellationToken)
        {
            var command = new ObterAulasMatriculaQuery(id);
            var response = await mediator.Send(command, cancellationToken);

            if (response.IsSuccess)
            {
                return TypedResults.Ok(response.Value);
            }

            return TypedResults.BadRequest(response.Error);
        }
    }
}