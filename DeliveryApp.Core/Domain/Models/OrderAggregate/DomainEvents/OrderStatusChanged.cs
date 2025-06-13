using Primitives;

namespace DeliveryApp.Core.Domain.Models.OrderAggregate.DomainEvents;

public class OrderStatusChanged(Order order)
{
    public Guid Id { get; set; } = order.Id;

    public OrderStatus Status { get; set; } = order.Status;
}