using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;

public sealed class CreateOrderHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateOrderCommand, bool>
{
    public async Task<bool> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        if (await orderRepository.GetByIdAsync(request.BasketId, cancellationToken) != null)
            throw new Exception($"Уже существует заказ с идентификатором: {request.BasketId}");
        
        var order = Order.Create(request.BasketId, Location.Random());
        await orderRepository.CreateAsync(order, cancellationToken);
        
        return await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}