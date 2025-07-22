using Peo.Core.Infra.Data.Repositories;
using Peo.GestaoConteudo.Domain.Entities;
using Peo.GestaoConteudo.Infra.Data.Contexts;

namespace Peo.GestaoConteudo.Infra.Data.Repositories
{
    public class CursoRepository : GenericRepository<Curso, GestaoConteudoContext>
    {
        public CursoRepository(GestaoConteudoContext dbContext) : base(dbContext)
        {
        }
    }
}