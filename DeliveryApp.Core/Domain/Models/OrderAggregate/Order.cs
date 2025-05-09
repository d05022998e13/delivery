using DeliveryApp.Core.Domain.Models.SharedKernel;
using DeliveryApp.Core.Domain.Models.SharedKernel.Interfaces;
using Primitives;

namespace DeliveryApp.Core.Domain.Models.OrderAggregate;

public class Order : Aggregate<Guid>, ILocationOwner
{
    public Location Location { get; private set; }
    
    public OrderStatus Status { get; private set;}
    
    public Guid? CourierId { get; private set;}

    public static Order Create(Guid id, Location location)
    {
        if (id.Equals(Guid.Empty)) throw new ArgumentException("Не указан идентификатор", nameof(id));
        
        if (location == null) throw new ArgumentException("Не указано местоположение", nameof(location));

        return new Order
        {
            Id = id,
            Location = location,
            Status = OrderStatus.Created
        };
    }

    public void Assign(Guid courierId)
    {
        if (courierId.Equals(Guid.Empty)) throw new ArgumentException("Не указан идентификатор курьера", nameof(courierId));
        
        if (Status != OrderStatus.Created) throw new Exception($"Невозможно назначить заказ в статусе: \"{Status}\"");
        
        CourierId = courierId;
        Status = OrderStatus.Assigned;
    }

    public void Complete()
    {
        if (Status != OrderStatus.Assigned) throw new Exception($"Невозможно завершить заказ в статусе: \"{Status}\"");
        
        Status = OrderStatus.Completed;
    }
}