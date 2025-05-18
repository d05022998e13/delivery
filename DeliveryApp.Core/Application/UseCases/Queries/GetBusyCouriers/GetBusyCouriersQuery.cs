using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetBusyCouriers;

public sealed class GetBusyCouriersQuery : IRequest<GetBusyCouriersResponse>;