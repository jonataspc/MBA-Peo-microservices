using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Peo.Core.Infra.Data.Configurations.Base;
using Peo.GestaoConteudo.Domain.Entities;

namespace Peo.GestaoConteudo.Infra.Data.Configurations
{
    internal class ArquivoAulaEntityConfiguration : EntityBaseConfiguration<ArquivoAula>
    {
        public override void Configure(EntityTypeBuilder<ArquivoAula> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.Titulo)
                   .HasMaxLength(256)
                   .IsRequired();

            builder.Property(e => e.Url)
                   .HasMaxLength(1024)
                   .IsRequired();

            builder.HasOne(o => o.Aula)
                   .WithMany(c => c.Arquivos)
                   .HasForeignKey(o => o.AulaId);
        }
    }
}