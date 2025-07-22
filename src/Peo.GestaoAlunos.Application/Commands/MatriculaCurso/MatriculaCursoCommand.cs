using Peo.GestaoAlunos.Application.Dtos.Requests;
using Peo.GestaoAlunos.Application.Dtos.Responses;

namespace Peo.GestaoAlunos.Application.Commands.MatriculaCurso;

public class MatriculaCursoCommand : IRequest<Result<MatriculaCursoResponse>>
{
    public MatriculaCursoRequest Request { get; }

    public MatriculaCursoCommand(MatriculaCursoRequest request)
    {
        Request = request;
    }
}