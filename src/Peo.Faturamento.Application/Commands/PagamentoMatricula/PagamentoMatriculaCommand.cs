using MediatR;
using Peo.Core.DomainObjects.Result;
using Peo.Faturamento.Application.Dtos.Requests;
using Peo.Faturamento.Application.Dtos.Responses;

namespace Peo.Faturamento.Application.Commands.PagamentoMatricula;

public class PagamentoMatriculaCommand : IRequest<Result<PagamentoMatriculaResponse>>
{
    public PagamentoMatriculaRequest Request { get; }

    public PagamentoMatriculaCommand(PagamentoMatriculaRequest request)
    {
        Request = request;
    }
}