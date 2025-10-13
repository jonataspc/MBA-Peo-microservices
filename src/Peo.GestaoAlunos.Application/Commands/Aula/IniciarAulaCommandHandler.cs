using Microsoft.Extensions.Logging;
using Peo.GestaoAlunos.Application.Dtos.Responses;
using Peo.GestaoAlunos.Domain.Services;

namespace Peo.GestaoAlunos.Application.Commands.Aula;

public class IniciarAulaCommandHandler : IRequestHandler<IniciarAulaCommand, Result<ProgressoAulaResponse>>
{
    private readonly IAlunoService _alunoService;
    private readonly ILogger<IniciarAulaCommandHandler> _logger;

    public IniciarAulaCommandHandler(IAlunoService alunoService, ILogger<IniciarAulaCommandHandler> logger)
    {
        _alunoService = alunoService;
        _logger = logger;
    }

    public async Task<Result<ProgressoAulaResponse>> Handle(IniciarAulaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var progresso = await _alunoService.IniciarAulaAsync(request.Request.MatriculaId, request.Request.AulaId, cancellationToken);

            var response = new ProgressoAulaResponse(
                progresso.MatriculaId,
                progresso.AulaId,
                progresso.EstaConcluido,
                progresso.DataInicio,
                progresso.DataConclusao,
                await _alunoService.ObterProgressoGeralCursoAsync(progresso.MatriculaId, cancellationToken)
            );

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao iniciar aula para matrícula {MatriculaId} e aula {AulaId}",
                request.Request.MatriculaId, request.Request.AulaId);
            return Result.Failure<ProgressoAulaResponse>(new Error(ex.Message));
        }
    }
}