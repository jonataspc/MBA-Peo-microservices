using Peo.GestaoAlunos.Application.Dtos.Requests;
using Peo.GestaoAlunos.Application.Dtos.Responses;

namespace Peo.GestaoAlunos.Application.Commands.Aula;

public class ConcluirAulaCommand : IRequest<Result<ProgressoAulaResponse>>
{
    public ConcluirAulaRequest Request { get; }

    public ConcluirAulaCommand(ConcluirAulaRequest request)
    {
        Request = request;
    }
}