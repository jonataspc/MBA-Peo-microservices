using Peo.GestaoAlunos.Application.Dtos.Responses;

namespace Peo.GestaoAlunos.Application.Queries.ObterAulasMatricula
{
    public class ObterAulasMatriculaQuery : IRequest<Result<IEnumerable<AulaMatriculaResponse>>>
    {
        public ObterAulasMatriculaQuery(Guid matriculaId)
        {
            MatriculaId = matriculaId;
        }

        public Guid MatriculaId { get; }
    }
}