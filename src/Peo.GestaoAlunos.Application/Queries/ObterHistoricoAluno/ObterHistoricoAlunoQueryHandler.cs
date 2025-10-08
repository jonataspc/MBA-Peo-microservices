using Mapster;
using Microsoft.Extensions.Logging;
using Peo.Core.Interfaces.Services;
using Peo.GestaoAlunos.Application.Dtos.Responses;
using Peo.GestaoAlunos.Domain.Entities;
using Peo.GestaoAlunos.Domain.Services;

namespace Peo.GestaoAlunos.Application.Queries.ObterHistoricoAluno
{
    public class ObterHistoricoAlunoQueryHandler : IRequestHandler<ObterHistoricoAlunoQuery, Result<IEnumerable<HistoricoAlunoResponse>>>
    {
        private readonly IAlunoService _alunoService;
        private readonly ILogger<ObterHistoricoAlunoQueryHandler> _logger;
        private readonly IAppIdentityUser _appIdentityUser;

        public ObterHistoricoAlunoQueryHandler(IAlunoService alunoService, ILogger<ObterHistoricoAlunoQueryHandler> logger, IAppIdentityUser appIdentityUser)
        {
            _alunoService = alunoService;
            _logger = logger;
            _appIdentityUser = appIdentityUser;
        }

        public async Task<Result<IEnumerable<HistoricoAlunoResponse>>> Handle(ObterHistoricoAlunoQuery request, CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<Matricula> matriculas = await _alunoService.ObterMatriculasConcluidas(_appIdentityUser.GetUserId(), cancellationToken);

                return Result.Success(matriculas.Adapt<IEnumerable<HistoricoAlunoResponse>>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Result.Failure<IEnumerable<HistoricoAlunoResponse>>(new Error(ex.Message));
            }
        }
    }
}