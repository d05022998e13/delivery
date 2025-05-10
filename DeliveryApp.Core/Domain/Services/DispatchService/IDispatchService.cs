using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate;

namespace DeliveryApp.Core.Domain.Services.DispatchService;

public interface IDispatchService
{ 
    /// <summary>
    /// Назначение подходящего курьера на заказ.
    /// </summary>
    /// <param name="order">Новый заказ.</param>
    /// <param name="couriers">Список свободных курьеров.</param>
    /// <returns>Подходящий курьер.</returns>
    Courier Dispatch(Order order, List<Courier> couriers);
}