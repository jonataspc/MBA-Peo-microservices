using Peo.GestaoAlunos.Application.Dtos.Requests;
using Peo.GestaoAlunos.Application.Dtos.Responses;

namespace Peo.GestaoAlunos.Application.Commands.Aula;

public class IniciarAulaCommand : IRequest<Result<ProgressoAulaResponse>>
{
    public IniciarAulaRequest Request { get; }

    public IniciarAulaCommand(IniciarAulaRequest request)
    {
        Request = request;
    }
}