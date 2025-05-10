using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Ports;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetNotCompletedOrders;

public sealed class GetNotCompletedOrdersHandler(IQueryDbContext context) : IRequestHandler<GetNotCompletedOrdersQuery, GetNotCompletedOrdersResponse>
{
    public async Task<GetNotCompletedOrdersResponse> Handle(
        GetNotCompletedOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var orders = await context.Orders
            .Where(x => x.Status.Name != OrderStatus.Completed.Name)
            .Select(x => new GetNotCompletedOrdersResponse.Order
            {
                Id = x.Id,
                Location = new GetNotCompletedOrdersResponse.Location
                {
                    X = x.Location.X,
                    Y = x.Location.Y,
                }
            }).ToListAsync(cancellationToken);
        
        return new GetNotCompletedOrdersResponse(orders);
    }
}