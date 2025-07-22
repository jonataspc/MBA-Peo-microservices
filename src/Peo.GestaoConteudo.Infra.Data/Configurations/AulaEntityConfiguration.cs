using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Peo.Core.Infra.Data.Configurations.Base;
using Peo.GestaoConteudo.Domain.Entities;

namespace Peo.GestaoConteudo.Infra.Data.Configurations
{
    internal class AulaEntityConfiguration : EntityBaseConfiguration<Aula>
    {
        public override void Configure(EntityTypeBuilder<Aula> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.Titulo)
                   .HasMaxLength(256)
                   .IsRequired();

            builder.Property(e => e.Descricao)
                   .HasMaxLength(1024);

            builder.Property(e => e.UrlVideo)
                   .HasMaxLength(1024);

            builder.HasOne(o => o.Curso)
                   .WithMany(c => c.Aulas)
                   .HasForeignKey(o => o.CursoId);
        }
    }
}