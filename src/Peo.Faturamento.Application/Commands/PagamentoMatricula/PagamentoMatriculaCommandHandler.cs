using MassTransit;
using MediatR;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Messages.IntegrationCommands;
using Peo.Core.Messages.IntegrationRequests;
using Peo.Core.Messages.IntegrationResponses;
using Peo.Faturamento.Application.Dtos.Responses;

namespace Peo.Faturamento.Application.Commands.PagamentoMatricula;

public class PagamentoMatriculaCommandHandler(
    IMediator mediator,
    IRequestClient<ObterDetalhesCursoRequest> requestClientObterDetalhesCurso,
    IRequestClient<ObterMatriculaRequest> requestClientObterMatricula
    ) : IRequestHandler<PagamentoMatriculaCommand, Result<PagamentoMatriculaResponse>>
{
    public async Task<Result<PagamentoMatriculaResponse>> Handle(PagamentoMatriculaCommand request, CancellationToken cancellationToken)
    {
        var responseMatricula = await requestClientObterMatricula.GetResponse<ObterMatriculaResponse>(new ObterMatriculaRequest(request.Request.MatriculaId));

        ObterMatriculaResponse? matricula = responseMatricula.Message;

        if (matricula?.MatriculaId is null)
        {
            return Result.Failure<PagamentoMatriculaResponse>(new Error("Matrícula não encontrada"));
        }

        if (!matricula.PermitePagamento)
        {
            return Result.Failure<PagamentoMatriculaResponse>(new Error($"Pagamento já realizado"));
        }

        var responseCurso = await requestClientObterDetalhesCurso.GetResponse<ObterDetalhesCursoResponse>(new ObterDetalhesCursoRequest(matricula.CursoId!.Value));

        var preco = responseCurso.Message!.Preco!.Value;

        var resultPagamento = await mediator.Send(
            new ProcessarPagamentoMatriculaCommand(matricula.MatriculaId.Value, preco, request.Request.DadosCartao),
            cancellationToken
            );

        if (resultPagamento.IsFailure)
        {
            return Result.Failure<PagamentoMatriculaResponse>(resultPagamento.Error);
        }

        var response = new PagamentoMatriculaResponse(matricula.MatriculaId.Value, resultPagamento.Value?.StatusPagamento, preco);

        return Result.Success(response);
    }
}