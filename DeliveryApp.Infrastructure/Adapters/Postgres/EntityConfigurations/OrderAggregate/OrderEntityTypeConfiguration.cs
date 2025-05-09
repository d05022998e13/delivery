using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Infrastructure.Adapters.Postgres.EntityConfigurations.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.EntityConfigurations.OrderAggregate;

public class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        builder.HasKey(entity => entity.Id);

        builder
            .Property(entity => entity.Id)
            .ValueGeneratedNever()
            .HasColumnName("id")
            .IsRequired();

        builder
            .Property(entity => entity.CourierId)
            .HasColumnName("courier_id")
            .IsRequired(false);

        builder.OwnsOne(entity => entity.Status, b =>
        {
            b.Property(x => x.Name).HasColumnName("status").IsRequired();
            b.WithOwner();
        });
        builder.Navigation(entity => entity.Status).IsRequired();

        builder.OwnsOneRequiredLocation();
    }
}