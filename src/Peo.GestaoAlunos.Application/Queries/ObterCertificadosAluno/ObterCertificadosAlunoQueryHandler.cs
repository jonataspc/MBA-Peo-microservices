using Microsoft.Extensions.Logging;
using Peo.Core.Interfaces.Services;
using Peo.GestaoAlunos.Application.Dtos.Responses;
using Peo.GestaoAlunos.Domain.Services;

namespace Peo.GestaoAlunos.Application.Queries.ObterCertificadosAluno;

public class ObterCertificadosAlunoQueryHandler : IRequestHandler<ObterCertificadosAlunoQuery, Result<IEnumerable<CertificadoAlunoResponse>>>
{
    private readonly IAlunoService _alunoService;
    private readonly ILogger<ObterCertificadosAlunoQueryHandler> _logger;
    private readonly IAppIdentityUser _appIdentityUser;

    public ObterCertificadosAlunoQueryHandler(IAlunoService alunoService, ILogger<ObterCertificadosAlunoQueryHandler> logger, IAppIdentityUser appIdentityUser)
    {
        _alunoService = alunoService;
        _logger = logger;
        _appIdentityUser = appIdentityUser;
    }

    public async Task<Result<IEnumerable<CertificadoAlunoResponse>>> Handle(ObterCertificadosAlunoQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var aluno = await _alunoService.ObterAlunoPorUserIdAsync(_appIdentityUser.GetUserId(), cancellationToken);
            var certificados = await _alunoService.ObterCertificadosDoAlunoAsync(aluno.Id, cancellationToken);

            var response = certificados.Select(c => new CertificadoAlunoResponse(
                c.Id,
                c.MatriculaId,
                c.Conteudo,
                c.DataEmissao,
                c.NumeroCertificado
            ));

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter certificados do aluno");
            return Result.Failure<IEnumerable<CertificadoAlunoResponse>>(new Error(ex.Message));
        }
    }
}