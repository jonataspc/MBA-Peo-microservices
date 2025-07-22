using Peo.GestaoAlunos.Application.Dtos.Requests;
using Peo.GestaoAlunos.Application.Dtos.Responses;

namespace Peo.GestaoAlunos.Application.Commands.PagamentoMatricula;

public class PagamentoMatriculaCommand : IRequest<Result<PagamentoMatriculaResponse>>
{
    public PagamentoMatriculaRequest Request { get; }

    public PagamentoMatriculaCommand(PagamentoMatriculaRequest request)
    {
        Request = request;
    }
}