using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Peo.Core.Infra.Data.Configurations.Base;
using Peo.GestaoAlunos.Domain.Entities;

namespace Peo.GestaoAlunos.Infra.Data.Configurations;

public class CertificadoConfiguration : EntityBaseConfiguration<Certificado>
{
    public override void Configure(EntityTypeBuilder<Certificado> builder)
    {
        base.Configure(builder);

        builder.Property(c => c.MatriculaId)
            .IsRequired();

        builder.Property(c => c.Conteudo)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(c => c.NumeroCertificado)
            .IsRequired(false)
            .HasMaxLength(50);

        // Relacionamentos
        builder.HasOne(c => c.Matricula)
            .WithMany()
            .HasForeignKey(c => c.MatriculaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(c => c.MatriculaId);
        builder.HasIndex(c => c.NumeroCertificado)
            .IsUnique();
    }
}