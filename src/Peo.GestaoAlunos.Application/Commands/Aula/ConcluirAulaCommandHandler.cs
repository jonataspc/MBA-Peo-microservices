using Microsoft.Extensions.Logging;
using Peo.GestaoAlunos.Application.Dtos.Responses;
using Peo.GestaoAlunos.Domain.Services;

namespace Peo.GestaoAlunos.Application.Commands.Aula;

public class ConcluirAulaCommandHandler : IRequestHandler<ConcluirAulaCommand, Result<ProgressoAulaResponse>>
{
    private readonly IAlunoService _alunoService;
    private readonly ILogger<ConcluirAulaCommandHandler> _logger;

    public ConcluirAulaCommandHandler(IAlunoService alunoService, ILogger<ConcluirAulaCommandHandler> logger)
    {
        _alunoService = alunoService;
        _logger = logger;
    }

    public async Task<Result<ProgressoAulaResponse>> Handle(ConcluirAulaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var progresso = await _alunoService.ConcluirAulaAsync(request.Request.MatriculaId, request.Request.AulaId, cancellationToken);

            var percent = await _alunoService.ObterProgressoGeralCursoAsync(progresso.MatriculaId, cancellationToken);

            var response = new ProgressoAulaResponse(
                progresso.MatriculaId,
                progresso.AulaId,
                progresso.EstaConcluido,
                progresso.DataInicio,
                progresso.DataConclusao,
                percent
            );

            if (percent == 100)
            {
                await _alunoService.ConcluirMatriculaAsync(progresso.MatriculaId, cancellationToken);
            }

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao concluir aula para matrícula {MatriculaId} e aula {AulaId}",
                request.Request.MatriculaId, request.Request.AulaId);
            return Result.Failure<ProgressoAulaResponse>(new Error(ex.Message));
        }
    }
}