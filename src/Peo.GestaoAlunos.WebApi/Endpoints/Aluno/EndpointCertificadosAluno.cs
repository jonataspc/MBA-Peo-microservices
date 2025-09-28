using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Peo.Core.DomainObjects;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Web.Api;
using Peo.GestaoAlunos.Application.Dtos.Responses;
using Peo.GestaoAlunos.Application.Queries.ObterCertificadosAluno;

namespace Peo.GestaoAlunos.WebApi.Endpoints.Aluno;

public class EndpointCertificadosAluno : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/certificados", Handler)
           .WithSummary("Obter certificados do aluno")
           .RequireAuthorization(AccessRoles.Aluno);
    }

    private static async Task<Results<Ok<IEnumerable<CertificadoAlunoResponse>>, BadRequest<Error>>> Handler(
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new ObterCertificadosAlunoQuery();
        var response = await mediator.Send(query, cancellationToken);

        if (response.IsSuccess)
        {
            return TypedResults.Ok(response.Value);
        }

        return TypedResults.BadRequest(response.Error);
    }
}