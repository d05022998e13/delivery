using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Commands.MoveCouriers;

public sealed class MoveCouriersCommand : IRequest<bool>;