using Peo.GestaoAlunos.Application.Dtos.Responses;

namespace Peo.GestaoAlunos.Application.Queries.ObterCertificadosAluno;

public class ObterCertificadosAlunoQuery : IRequest<Result<IEnumerable<CertificadoAlunoResponse>>>
{
    public ObterCertificadosAlunoQuery()
    {
    }
}