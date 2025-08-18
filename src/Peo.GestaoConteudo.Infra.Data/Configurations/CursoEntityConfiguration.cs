using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Peo.Core.Infra.Data.Configurations.Base;
using Peo.GestaoConteudo.Domain.Entities;

namespace Peo.GestaoConteudo.Infra.Data.Configurations
{
    internal class CursoEntityConfiguration : EntityBaseConfiguration<Curso>
    {
        public override void Configure(EntityTypeBuilder<Curso> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.Titulo)
                   .HasMaxLength(256)
                   .IsRequired();

            builder.Property(e => e.Descricao)
                   .HasMaxLength(1024);

            builder.Property(e => e.Preco)
                   .HasPrecision(10, 2)
                   .IsRequired();

            builder.OwnsOne(c => c.ConteudoProgramatico, pc =>
            {
                pc.Property(p => p.Conteudo).HasMaxLength(1024);
            });
        }
    }
}