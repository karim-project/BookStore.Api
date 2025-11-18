using BookStore.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stripe.Climate;

namespace BookStore.Api.Data.EntityConfigurations
{
    public class OrdersEntityTypeConfiguration : IEntityTypeConfiguration<Orders>
    {
        public void Configure(EntityTypeBuilder<Orders> builder)
        {
            // Order → OrderItems (One-to-Many)
            builder
                .HasMany(o => o.Items)
                .WithOne(i => i.Orders)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
