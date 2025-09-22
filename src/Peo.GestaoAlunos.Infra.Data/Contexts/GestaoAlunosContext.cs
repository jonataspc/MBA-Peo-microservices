using Microsoft.EntityFrameworkCore;
using Peo.Core.Infra.Data.Contexts.Base;
using Peo.Core.Infra.Data.Extensions;
using Peo.GestaoAlunos.Domain.Entities;
using System.Reflection;

namespace Peo.GestaoAlunos.Infra.Data.Contexts
{
    public class GestaoAlunosContext : DbContextBase
    {
        public DbSet<Aluno> Alunos { get; set; } = null!;
        public DbSet<Matricula> Matriculas { get; set; } = null!;
        public DbSet<ProgressoMatricula> ProgressosMatricula { get; set; } = null!;
        public DbSet<Certificado> Certificados { get; set; } = null!;

        public GestaoAlunosContext(DbContextOptions<GestaoAlunosContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.FixPrecisionForDecimalDataTypes()
                   .ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly())
                   .RemovePluralizingTableNameConvention();

            base.OnModelCreating(modelBuilder);
        }
    }
}