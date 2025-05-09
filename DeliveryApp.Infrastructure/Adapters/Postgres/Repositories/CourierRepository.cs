using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Ports;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;

public class CourierRepository(ApplicationDbContext context) : ICourierRepository
{
    public async Task<Courier> GetByIdAsync(Guid id, CancellationToken ct) =>
        await context.Couriers.FirstOrDefaultAsync(o => o.Id == id, ct)
        ?? throw new Exception($"Не найден курьер с идентификатором {id}");
    
    public async Task<ICollection<Courier>> GetAllFreeAsync(CancellationToken ct) =>
        await context.Couriers.Where(o => o.Status.Name == CourierStatus.Free.Name).ToListAsync(ct);
    
    public async Task CreateAsync(Courier courier, CancellationToken ct) => await context.AddAsync(courier, ct);
    
    public void Update(Courier courier) => context.Update(courier);
}