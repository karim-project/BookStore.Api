using BookStore.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Api.Data.EntityConfigurations
{
    public class OrderItemEntityTypeConfiguration : IEntityTypeConfiguration<OrdersItem>
    {
        public void Configure(EntityTypeBuilder<OrdersItem> builder)
        {
            // Decimal precision for UnitPrice
            builder
                .Property(i => i.UnitPrice)
                .HasColumnType("decimal(18,2)");
        }
    }
}
