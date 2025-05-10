using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;

public sealed class CreateOrderCommand(Guid basketId, string street) : IRequest<bool>
{
    /// <summary>
    /// Идентификатор корзины.
    /// </summary>
    /// <remarks>Id корзины берется за основу при создании Id заказа, они совпадают.</remarks>
    public Guid BasketId { get; } = !basketId.Equals(Guid.Empty)
        ? basketId
        : throw new ArgumentNullException(nameof(basketId));

    /// <summary>
    /// Улица.
    /// </summary>
    public string Street { get; } = !string.IsNullOrWhiteSpace(street)
        ? street
        : throw new ArgumentNullException(nameof(street));
}