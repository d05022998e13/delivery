using Primitives;

namespace DeliveryApp.Core.Domain.Models.OrderAggregate.DomainEvents;

public sealed record OrderStatusChangedDomainEvent(OrderStatusChanged Order) : DomainEvent;