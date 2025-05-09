using DeliveryApp.Core.Domain.Models.CourierAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.EntityConfigurations.CourierAggregate;

public class TransportEntityTypeConfiguration : IEntityTypeConfiguration<Transport>
{
    public void Configure(EntityTypeBuilder<Transport> builder)
    {
        builder.ToTable("transports");

        builder.HasKey(entity => entity.Id);

        builder
            .Property(entity => entity.Id)
            .ValueGeneratedNever()
            .HasColumnName("id")
            .IsRequired();

        builder
            .Property(entity => entity.Name)
            .HasColumnName("name")
            .IsRequired();
        
        builder.OwnsOne(entity => entity.Speed, b =>
        {
            b.Property(x => x.Value).HasColumnName("speed").IsRequired();
            b.WithOwner();
        });
        builder.Navigation(entity => entity.Speed).IsRequired();
    }
}