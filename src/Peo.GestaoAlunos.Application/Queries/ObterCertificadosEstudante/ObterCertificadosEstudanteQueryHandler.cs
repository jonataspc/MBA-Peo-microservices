using Microsoft.Extensions.Logging;
using Peo.Core.Interfaces.Services;
using Peo.GestaoAlunos.Application.Dtos.Responses;
using Peo.GestaoAlunos.Domain.Interfaces;

namespace Peo.GestaoAlunos.Application.Queries.ObterCertificadosEstudante;

public class ObterCertificadosEstudanteQueryHandler : IRequestHandler<ObterCertificadosEstudanteQuery, Result<IEnumerable<CertificadoEstudanteResponse>>>
{
    private readonly IEstudanteService _estudanteService;
    private readonly ILogger<ObterCertificadosEstudanteQueryHandler> _logger;
    private readonly IAppIdentityUser _appIdentityUser;

    public ObterCertificadosEstudanteQueryHandler(IEstudanteService estudanteService, ILogger<ObterCertificadosEstudanteQueryHandler> logger, IAppIdentityUser appIdentityUser)
    {
        _estudanteService = estudanteService;
        _logger = logger;
        _appIdentityUser = appIdentityUser;
    }

    public async Task<Result<IEnumerable<CertificadoEstudanteResponse>>> Handle(ObterCertificadosEstudanteQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var estudante = await _estudanteService.ObterEstudantePorUserIdAsync(_appIdentityUser.GetUserId(), cancellationToken);
            var certificados = await _estudanteService.ObterCertificadosDoEstudanteAsync(estudante.Id, cancellationToken);

            var response = certificados.Select(c => new CertificadoEstudanteResponse(
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
            _logger.LogError(ex, "Erro ao obter certificados do estudante");
            return Result.Failure<IEnumerable<CertificadoEstudanteResponse>>(new Error(ex.Message));
        }
    }
}