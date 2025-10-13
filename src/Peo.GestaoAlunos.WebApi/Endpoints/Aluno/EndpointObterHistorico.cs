using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Peo.Core.DomainObjects;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Web.Api;
using Peo.GestaoAlunos.Application.Dtos.Responses;
using Peo.GestaoAlunos.Application.Queries.ObterHistoricoAluno;

namespace Peo.GestaoAlunos.WebApi.Endpoints.Aluno
{
    public class EndpointObterHistorico : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/progresso-matriculas/", Handle)
              .WithSummary("Consultar histórico do aluno")
              .RequireAuthorization(AccessRoles.Aluno);
        }

        private static async Task<Results<Ok<IEnumerable<HistoricoAlunoResponse>>, ValidationProblem, BadRequest<Error>>> Handle(IMediator mediator, CancellationToken cancellationToken)
        {
            var command = new ObterHistoricoAlunoQuery();
            var response = await mediator.Send(command, cancellationToken);

            if (response.IsSuccess)
            {
                return TypedResults.Ok(response.Value);
            }

            return TypedResults.BadRequest(response.Error);
        }
    }
}