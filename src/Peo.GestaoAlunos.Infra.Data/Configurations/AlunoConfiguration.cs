using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Peo.Core.Infra.Data.Configurations.Base;
using Peo.GestaoAlunos.Domain.Entities;

namespace Peo.GestaoAlunos.Infra.Data.Configurations;

public class AlunoConfiguration : EntityBaseConfiguration<Aluno>
{
    public override void Configure(EntityTypeBuilder<Aluno> builder)
    {
        base.Configure(builder);

        builder.Property(s => s.UsuarioId)
               .IsRequired();

        builder.Property(s => s.EstaAtivo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(s => s.UsuarioId)
            .IsUnique();
    }
}