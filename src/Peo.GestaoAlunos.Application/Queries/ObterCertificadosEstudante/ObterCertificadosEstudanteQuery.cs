using Peo.GestaoAlunos.Application.Dtos.Responses;

namespace Peo.GestaoAlunos.Application.Queries.ObterCertificadosEstudante;

public class ObterCertificadosEstudanteQuery : IRequest<Result<IEnumerable<CertificadoEstudanteResponse>>>
{
    public ObterCertificadosEstudanteQuery()
    {
    }
}