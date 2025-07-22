using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Peo.Core.Entities;
using Peo.Core.Infra.Data.Configurations.Base;
using Peo.GestaoAlunos.Domain.Entities;

namespace Peo.GestaoAlunos.Infra.Data.Configurations;

public class EstudanteConfiguration : EntityBaseConfiguration<Estudante>
{
    public override void Configure(EntityTypeBuilder<Estudante> builder)
    {
        base.Configure(builder);

        builder.Property(s => s.UsuarioId)
               .IsRequired();

        builder.Property(s => s.EstaAtivo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(s => s.UsuarioId)
            .IsUnique();

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}