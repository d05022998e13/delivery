using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Ports;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetBusyCouriers;

public sealed class GetBusyCouriersHandler(IQueryDbContext context): IRequestHandler<GetBusyCouriersQuery, GetBusyCouriersResponse>
{
    public async Task<GetBusyCouriersResponse> Handle(GetBusyCouriersQuery request, CancellationToken cancellationToken)
    {
        var couriers = await context.Couriers
            .Where(x => x.Status.Name == CourierStatus.Busy.Name)
            .Select(x => new Courier
            {
                Id = x.Id,
                Name = x.Name,
                Location = new Location
                {
                    X = x.Location.X,
                    Y = x.Location.Y,
                }
            }).ToListAsync(cancellationToken);
        
        return new GetBusyCouriersResponse(couriers);
    }
}