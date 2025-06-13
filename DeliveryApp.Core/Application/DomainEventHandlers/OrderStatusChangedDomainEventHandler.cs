using DeliveryApp.Core.Domain.Models.OrderAggregate.DomainEvents;
using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.DomainEventHandlers;

public class OrderStatusChangedDomainEventHandler(IMessageBusProducer messageBusProducer)
    : INotificationHandler<OrderStatusChangedDomainEvent>
{
    public async Task Handle(OrderStatusChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        await messageBusProducer.Publish(notification, cancellationToken);
    }
}