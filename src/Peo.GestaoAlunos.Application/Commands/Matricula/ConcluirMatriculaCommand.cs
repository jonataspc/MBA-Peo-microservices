using Peo.GestaoAlunos.Application.Dtos.Requests;
using Peo.GestaoAlunos.Application.Dtos.Responses;

namespace Peo.GestaoAlunos.Application.Commands.Matricula;

public class ConcluirMatriculaCommand : IRequest<Result<ConcluirMatriculaResponse>>
{
    public ConcluirMatriculaRequest Request { get; }

    public ConcluirMatriculaCommand(ConcluirMatriculaRequest request)
    {
        Request = request;
    }
}