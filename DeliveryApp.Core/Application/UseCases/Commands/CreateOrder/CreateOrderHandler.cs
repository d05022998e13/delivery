using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;

public sealed class CreateOrderHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    IGeoClient geoClient) : IRequestHandler<CreateOrderCommand, bool>
{
    public async Task<bool> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        if (await orderRepository.CheckIfOrderExists(request.BasketId, cancellationToken))
            throw new Exception($"Уже существует заказ с идентификатором: {request.BasketId}");
        
        var location = await geoClient.GetLocation(request.Street, cancellationToken);
        
        var order = Order.Create(request.BasketId, location);
        await orderRepository.CreateAsync(order, cancellationToken);
        
        return await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}