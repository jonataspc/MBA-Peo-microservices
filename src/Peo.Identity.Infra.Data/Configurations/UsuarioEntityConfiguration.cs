using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Peo.Core.Entities;
using Peo.Core.Infra.Data.Configurations.Base;

namespace Peo.Identity.Infra.Data.Configurations
{
    internal class UsuarioEntityConfiguration : EntityBaseConfiguration<Usuario>
    {
        public override void Configure(EntityTypeBuilder<Usuario> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.NomeCompleto)
                   .HasMaxLength(256)
                   .IsRequired();

            builder.Property(e => e.Email)
                   .HasMaxLength(256)
                   .IsRequired();
        }
    }
}