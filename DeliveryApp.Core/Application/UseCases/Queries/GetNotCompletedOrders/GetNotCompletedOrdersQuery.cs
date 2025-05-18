using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetNotCompletedOrders;

public sealed class GetNotCompletedOrdersQuery : IRequest<GetNotCompletedOrdersResponse>;