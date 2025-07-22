using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using MiniValidation;
using Peo.Core.DomainObjects;
using Peo.Core.Web.Api;
using Peo.GestaoAlunos.Application.Commands.MatriculaCurso;
using Peo.GestaoAlunos.Application.Dtos.Requests;
using Peo.GestaoAlunos.Application.Dtos.Responses;

namespace Peo.GestaoAlunos.Application.Endpoints.Matricula
{
    public class EndpointMatriculaCurso : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("/matricula/", Handle)
              .WithSummary("Matricular em um novo curso")
              .RequireAuthorization(AccessRoles.Aluno);
        }

        private static async Task<Results<Ok<MatriculaCursoResponse>, ValidationProblem, BadRequest<Error>>> Handle(MatriculaCursoRequest request, IMediator mediator, CancellationToken cancellationToken)
        {
            if (!MiniValidator.TryValidate(request, out var errors))
            {
                return TypedResults.ValidationProblem(errors);
            }

            var command = new MatriculaCursoCommand(request);
            var response = await mediator.Send(command, cancellationToken);

            if (response.IsSuccess)
            {
                return TypedResults.Ok(response.Value);
            }

            return TypedResults.BadRequest(response.Error);
        }
    }
}