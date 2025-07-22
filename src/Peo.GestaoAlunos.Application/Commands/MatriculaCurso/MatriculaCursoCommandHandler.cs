using Microsoft.Extensions.Logging;
using Peo.Core.Interfaces.Services;
using Peo.GestaoAlunos.Application.Dtos.Responses;
using Peo.GestaoAlunos.Domain.Interfaces;

namespace Peo.GestaoAlunos.Application.Commands.MatriculaCurso;

public class MatriculaCursoCommandHandler : IRequestHandler<MatriculaCursoCommand, Result<MatriculaCursoResponse>>
{
    private readonly IEstudanteService _estudanteService;
    private readonly IAppIdentityUser _appIdentityUser;
    private readonly ILogger<MatriculaCursoCommandHandler> _logger;

    public MatriculaCursoCommandHandler(IEstudanteService estudanteService, IAppIdentityUser appIdentityUser, ILogger<MatriculaCursoCommandHandler> logger)
    {
        _estudanteService = estudanteService;
        _appIdentityUser = appIdentityUser;
        _logger = logger;
    }

    public async Task<Result<MatriculaCursoResponse>> Handle(MatriculaCursoCommand request, CancellationToken cancellationToken)
    {
        Domain.Entities.Matricula matricula;

        try
        {
            matricula = await _estudanteService.MatricularEstudanteComUserIdAsync(_appIdentityUser.GetUserId(), request.Request.CursoId, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return Result.Failure<MatriculaCursoResponse>(new Error(e.Message));
        }

        return Result.Success(new MatriculaCursoResponse(matricula.Id));
    }
}