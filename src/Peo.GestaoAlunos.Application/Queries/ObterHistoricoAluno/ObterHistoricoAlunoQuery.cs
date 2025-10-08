using Peo.GestaoAlunos.Application.Dtos.Responses;

namespace Peo.GestaoAlunos.Application.Queries.ObterHistoricoAluno
{
    public class ObterHistoricoAlunoQuery : IRequest<Result<IEnumerable<HistoricoAlunoResponse>>>
    {
        public ObterHistoricoAlunoQuery()
        {
        }
    }
}