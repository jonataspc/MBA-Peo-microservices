using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Peo.Core.Infra.Data.Configurations.Base;
using Peo.Faturamento.Domain.Entities;
using Peo.Faturamento.Domain.ValueObjects;

namespace Peo.Faturamento.Infra.Data.Configurations;

public class PagamentoConfiguration : EntityBaseConfiguration<Pagamento>
{
    public override void Configure(EntityTypeBuilder<Pagamento> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.MatriculaId)
            .IsRequired();

        builder.Property(p => p.Valor)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.Detalhes)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.Property(p => p.DataPagamento)
            .IsRequired(false);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (StatusPagamento)Enum.Parse(typeof(StatusPagamento), v))
            .HasDefaultValue(StatusPagamento.Pendente);

        builder.Property(p => p.IdTransacao)
            .IsRequired(false)
            .HasMaxLength(100);

        builder.OwnsOne(p => p.DadosCartao, creditCard =>
        {
            creditCard.Property(c => c.Hash)
                .IsRequired(false)
                .HasMaxLength(2048);
        });

        // Indexes
        builder.HasIndex(p => p.MatriculaId);
    }
}