using Mapster;
using Microsoft.Extensions.Logging;
using Peo.Core.Interfaces.Services;
using Peo.GestaoAlunos.Application.Dtos.Responses;
using Peo.GestaoAlunos.Application.Queries.ObterAulasMatricula;
using Peo.GestaoAlunos.Domain.Dtos;
using Peo.GestaoAlunos.Domain.Services;

namespace Peo.GestaoAlunos.Application.Queries.ObterMatriculas
{
    public class ObterAulasMatriculaQueryHandler : IRequestHandler<ObterAulasMatriculaQuery, Result<IEnumerable<AulaMatriculaResponse>>>
    {
        private readonly IAlunoService _alunoService;
        private readonly ILogger<ObterAulasMatriculaQueryHandler> _logger;
        private readonly IAppIdentityUser _appIdentityUser;

        public ObterAulasMatriculaQueryHandler(IAlunoService alunoService, ILogger<ObterAulasMatriculaQueryHandler> logger, IAppIdentityUser appIdentityUser)
        {
            _alunoService = alunoService;
            _logger = logger;
            _appIdentityUser = appIdentityUser;
        }

        public async Task<Result<IEnumerable<AulaMatriculaResponse>>> Handle(ObterAulasMatriculaQuery request, CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<AulaMatriculaDto> matriculas = await _alunoService.ObterAulasMatricula(_appIdentityUser.GetUserId(), request.MatriculaId, cancellationToken);

                return Result.Success(matriculas.Adapt<IEnumerable<AulaMatriculaResponse>>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Result.Failure<IEnumerable<AulaMatriculaResponse>>(new Error(ex.Message));
            }
        }
    }
}