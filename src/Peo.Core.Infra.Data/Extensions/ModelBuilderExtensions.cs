using Microsoft.EntityFrameworkCore;

namespace Peo.Core.Infra.Data.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder RemovePluralizingTableNameConvention(this ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.DisplayName());
            }

            return modelBuilder;
        }

        public static ModelBuilder FixPrecisionForDecimalDataTypes(this ModelBuilder builder)
        {
            foreach (var property in builder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(12, 2)");
            }

            return builder;
        }
    }
}