using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.MoveCouriers;

public sealed class MoveCouriersHandler(
    ICourierRepository courierRepository,
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<MoveCouriersCommand, bool>
{
    public async Task<bool> Handle(MoveCouriersCommand request, CancellationToken cancellationToken)
    {
        var orders = await orderRepository.GetAllAssignedAsync(cancellationToken);
        if (orders.Count == 0)
        {
            throw new Exception("Не найден ни один назначенный заказ");
        }

        foreach (var order in orders)
        {
            var courier = order.CourierId == null || order.CourierId.Equals(Guid.Empty)
                ? throw new NullReferenceException($"Не назначен курьер для заказа с идентификатором: {order.Id}")
                : await courierRepository.GetByIdAsync(order.CourierId.Value, cancellationToken);
            
            courier.Move(order.Location);

            if (courier.Location == order.Location)
            {
                order.Complete();
                courier.SetFree();
            }
            
            orderRepository.Update(order);
            courierRepository.Update(courier);
        }

        return await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}