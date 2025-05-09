using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Ports;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;

public class OrderRepository(ApplicationDbContext context) : IOrderRepository
{
    public async Task<Order> GetByIdAsync(Guid id, CancellationToken ct) =>
        await context.Orders.FirstOrDefaultAsync(o => o.Id == id, ct)
        ?? throw new Exception($"Не найден заказ с идентификатором {id}");
    
    public async Task<Order> GetFirstCreatedAsync(CancellationToken ct) =>
        await context.Orders.FirstOrDefaultAsync(o => o.Status.Name == OrderStatus.Created.Name, ct)
        ?? throw new Exception($"Не найден ни один заказ в статусе \"{OrderStatus.Created.Name}\"");
    
    public async Task<ICollection<Order>> GetAllAssignedAsync(CancellationToken ct) =>
        await context.Orders.Where(o => o.Status.Name == OrderStatus.Assigned.Name).ToListAsync(ct);
    
    public async Task CreateAsync(Order order, CancellationToken ct) => await context.AddAsync(order, ct);
    
    public void Update(Order order) => context.Update(order);
}