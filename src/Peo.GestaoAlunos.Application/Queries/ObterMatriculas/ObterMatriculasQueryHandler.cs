using Mapster;
using Microsoft.Extensions.Logging;
using Peo.Core.Interfaces.Services;
using Peo.GestaoAlunos.Application.Dtos.Responses;
using Peo.GestaoAlunos.Domain.Entities;
using Peo.GestaoAlunos.Domain.Interfaces;

namespace Peo.GestaoAlunos.Application.Queries.ObterMatriculas
{
    public class ObterMatriculasQueryHandler : IRequestHandler<ObterMatriculasQuery, Result<IEnumerable<MatriculaResponse>>>
    {
        private readonly IAlunoService _alunoService;
        private readonly ILogger<ObterMatriculasQueryHandler> _logger;
        private readonly IAppIdentityUser _appIdentityUser;

        public ObterMatriculasQueryHandler(IAlunoService alunoService, ILogger<ObterMatriculasQueryHandler> logger, IAppIdentityUser appIdentityUser)
        {
            _alunoService = alunoService;
            _logger = logger;
            _appIdentityUser = appIdentityUser;
        }

        public async Task<Result<IEnumerable<MatriculaResponse>>> Handle(ObterMatriculasQuery request, CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<Matricula> matriculas = await _alunoService.ObterMatriculas(_appIdentityUser.GetUserId(), cancellationToken);

                return Result.Success(matriculas.Adapt<IEnumerable<MatriculaResponse>>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Result.Failure<IEnumerable<MatriculaResponse>>(new Error(ex.Message));
            }
        }
    }
}