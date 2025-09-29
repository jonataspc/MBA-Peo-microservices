using Peo.GestaoAlunos.Application.Dtos.Responses;

namespace Peo.GestaoAlunos.Application.Queries.ObterMatriculas
{
    public class ObterMatriculasQuery : IRequest<Result<IEnumerable<MatriculaResponse>>>
    {
        public ObterMatriculasQuery()
        {
        }
    }
}