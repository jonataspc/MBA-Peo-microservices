using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Peo.Core.Infra.Data.Configurations.Base;
using Peo.GestaoAlunos.Domain.Entities;

namespace Peo.GestaoAlunos.Infra.Data.Configurations;

public class ProgressoMatriculaConfiguration : EntityBaseConfiguration<ProgressoMatricula>
{
    public override void Configure(EntityTypeBuilder<ProgressoMatricula> builder)
    {
        base.Configure(builder);

        builder.Property(ep => ep.MatriculaId)
            .IsRequired();

        builder.Property(ep => ep.AulaId)
            .IsRequired();

        builder.Property(ep => ep.DataInicio)
            .IsRequired();

        builder.Property(ep => ep.DataConclusao)
            .IsRequired(false);

        builder.HasIndex(ep => new { ep.MatriculaId, ep.AulaId })
            .IsUnique();

        builder.HasOne<Matricula>()
            .WithMany()
            .HasForeignKey(ep => ep.MatriculaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}