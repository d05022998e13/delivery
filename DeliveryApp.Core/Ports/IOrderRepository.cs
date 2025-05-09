using DeliveryApp.Core.Domain.Models.OrderAggregate;

namespace DeliveryApp.Core.Ports;

public interface IOrderRepository
{
    /// <summary>
    /// Получение заказа по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Заказ.</returns>
    Task<Order> GetByIdAsync(Guid id, CancellationToken ct);

    /// <summary>
    /// Получение заказа в статусе "Создан".
    /// </summary>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Заказ.</returns>
    Task<Order> GetFirstCreatedAsync(CancellationToken ct);

    /// <summary>
    /// Получение всех заказов в статусе "Назначен".
    /// </summary>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Коллекция заказов.</returns>
    Task<ICollection<Order>> GetAllAssignedAsync(CancellationToken ct);
    
    /// <summary>
    /// Создание заказа.
    /// </summary>
    /// <param name="order">Заказ.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Результат выполнения.</returns>
    Task CreateAsync(Order order, CancellationToken ct);

    /// <summary>
    /// Обновление заказа.
    /// </summary>
    /// <param name="order">Заказ.</param>
    void Update(Order order);
}