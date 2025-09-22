using Microsoft.Extensions.Logging;
using Peo.GestaoAlunos.Application.Dtos.Responses;
using Peo.GestaoAlunos.Domain.Interfaces;

namespace Peo.GestaoAlunos.Application.Commands.Matricula;

public class ConcluirMatriculaCommandHandler : IRequestHandler<ConcluirMatriculaCommand, Result<ConcluirMatriculaResponse>>
{
    private readonly IAlunoService _alunoService;
    private readonly ILogger<ConcluirMatriculaCommandHandler> _logger;

    public ConcluirMatriculaCommandHandler(IAlunoService alunoService, ILogger<ConcluirMatriculaCommandHandler> logger)
    {
        _alunoService = alunoService;
        _logger = logger;
    }

    public async Task<Result<ConcluirMatriculaResponse>> Handle(ConcluirMatriculaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var matricula = await _alunoService.ConcluirMatriculaAsync(request.Request.MatriculaId, cancellationToken);

            var response = new ConcluirMatriculaResponse(matricula.Id, matricula.Status.ToString(), matricula.DataConclusao, matricula.PercentualProgresso);

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao concluir matrícula {MatriculaId}", request.Request.MatriculaId);
            return Result.Failure<ConcluirMatriculaResponse>(new Error(ex.Message));
        }
    }
}