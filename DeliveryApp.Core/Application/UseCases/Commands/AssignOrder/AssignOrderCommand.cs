using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Commands.AssignOrder;

public sealed class AssignOrderCommand : IRequest<bool>;