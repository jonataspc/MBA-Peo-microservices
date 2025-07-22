using Peo.Faturamento.Infra.Data.Helpers;
using Peo.GestaoAlunos.Infra.Data.Helpers;
using Peo.GestaoConteudo.Infra.Data.Helpers;
using Peo.Identity.Infra.Data.Helpers;

namespace Peo.Web.Api.Configuration
{
    public static class DbMigrationHelpers
    {
        public static async Task UseDbMigrationHelperAsync(this WebApplication app)
        {
            await app.UseIdentityDbMigrationHelperAsync();
            await app.UseGestaoConteudoDbMigrationHelperAsync();
            await app.UseGestaoAlunosDbMigrationHelperAsync();
            await app.UseFaturamentoDbMigrationHelperAsync();
        }
    }
}