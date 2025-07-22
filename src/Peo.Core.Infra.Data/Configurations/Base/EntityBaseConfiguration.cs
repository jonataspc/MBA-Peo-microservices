using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Peo.Core.Entities.Base;

namespace Peo.Core.Infra.Data.Configurations.Base
{
    public abstract class EntityBaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
            where TEntity : EntityBase

    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.Property(e => e.Id)
                .IsRequired()
                .ValueGeneratedNever();

            builder.Property(e => e.CreatedAt)
                .IsRequired()
                .HasPrecision(0);

            builder.Property(e => e.ModifiedAt)
                .HasPrecision(0);
        }
    }
}