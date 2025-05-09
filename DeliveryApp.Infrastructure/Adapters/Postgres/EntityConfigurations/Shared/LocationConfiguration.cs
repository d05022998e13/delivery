using DeliveryApp.Core.Domain.Models.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.EntityConfigurations.Shared;

public static class LocationConfiguration
{
    public static void OwnsOneRequiredLocation<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : class, ILocationOwner
    {
        builder.OwnsOne(entity => entity.Location, b =>
        {
            b.Property(x => x.X).HasColumnName("location_x").IsRequired();
            b.Property(x => x.Y).HasColumnName("location_y").IsRequired();
            b.WithOwner();
        });
        
        builder.Navigation(entity => entity.Location).IsRequired();
    }
}