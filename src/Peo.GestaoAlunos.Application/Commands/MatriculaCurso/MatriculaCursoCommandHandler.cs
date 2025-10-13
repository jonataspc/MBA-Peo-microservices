using Microsoft.Extensions.Logging;
using Peo.Core.Interfaces.Services;
using Peo.GestaoAlunos.Application.Dtos.Responses;
using Peo.GestaoAlunos.Domain.Services;

namespace Peo.GestaoAlunos.Application.Commands.MatriculaCurso;

public class MatriculaCursoCommandHandler : IRequestHandler<MatriculaCursoCommand, Result<MatriculaCursoResponse>>
{
    private readonly IAlunoService _alunoService;
    private readonly IAppIdentityUser _appIdentityUser;
    private readonly ILogger<MatriculaCursoCommandHandler> _logger;

    public MatriculaCursoCommandHandler(IAlunoService alunoService, IAppIdentityUser appIdentityUser, ILogger<MatriculaCursoCommandHandler> logger)
    {
        _alunoService = alunoService;
        _appIdentityUser = appIdentityUser;
        _logger = logger;
    }

    public async Task<Result<MatriculaCursoResponse>> Handle(MatriculaCursoCommand request, CancellationToken cancellationToken)
    {
        Domain.Entities.Matricula matricula;

        try
        {
            matricula = await _alunoService.MatricularAlunoComUserIdAsync(_appIdentityUser.GetUserId(), request.Request.CursoId, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return Result.Failure<MatriculaCursoResponse>(new Error(e.Message));
        }

        return Result.Success(new MatriculaCursoResponse(matricula.Id));
    }
}