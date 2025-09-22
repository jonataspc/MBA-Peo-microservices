using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Peo.Core.Infra.Data.Configurations.Base;
using Peo.GestaoAlunos.Domain.Entities;
using Peo.GestaoAlunos.Domain.ValueObjects;

namespace Peo.GestaoAlunos.Infra.Data.Configurations;

public class MatriculaConfiguration : EntityBaseConfiguration<Matricula>
{
    public override void Configure(EntityTypeBuilder<Matricula> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.AlunoId)
            .IsRequired();

        builder.Property(e => e.CursoId)
            .IsRequired();

        builder.Property(e => e.DataMatricula)
            .IsRequired();

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (StatusMatricula)Enum.Parse(typeof(StatusMatricula), v))
            .HasDefaultValue(StatusMatricula.PendentePagamento);

        builder.Property(e => e.PercentualProgresso)
            .IsRequired()
            .HasDefaultValue(0);

        // Indexes
        builder.HasIndex(e => new { e.AlunoId, e.CursoId })
            .IsUnique();

        // Relationships
        builder.HasOne<Aluno>(e => e.Aluno)
            .WithMany(e => e.Matriculas)
            .HasForeignKey(e => e.AlunoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}