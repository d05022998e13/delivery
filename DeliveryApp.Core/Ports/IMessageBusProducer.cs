using DeliveryApp.Core.Domain.Models.OrderAggregate.DomainEvents;

namespace DeliveryApp.Core.Ports;

public interface IMessageBusProducer
{
    Task Publish(OrderStatusChangedDomainEvent notification, CancellationToken cancellationToken);
}