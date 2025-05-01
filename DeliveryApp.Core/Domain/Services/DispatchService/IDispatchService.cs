using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate;

namespace DeliveryApp.Core.Domain.Services.DispatchService;

public interface IDispatchService
{ 
    void Dispatch(Order order, List<Courier> couriers);
}