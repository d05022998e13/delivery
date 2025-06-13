using MediatR;
using Primitives;

namespace DeliveryApp.Infrastructure.Adapters.Postgres;

public class UnitOfWork(ApplicationDbContext dbContext, IMediator mediator) : IUnitOfWork, IDisposable
{
    private bool _disposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
        await PublishDomainEventsAsync(cancellationToken);
        
        return true;
    }
    
    private async Task PublishDomainEventsAsync(CancellationToken cancellationToken)
    {
        // Получили агрегаты в которых есть доменные события
        var domainEntities = dbContext.ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(x => x.Entity.GetDomainEvents().Count > 0)
            .ToList();

        // Переложили в отдельную переменную
        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.GetDomainEvents())
            .ToList();

        // Очистили Domain Event в самих агрегатах (поскольку далее они будут отправлены и больше не нужны)
        domainEntities.ToList()
            .ForEach(entity => entity.Entity.ClearDomainEvents());

        // Отправили в MediatR
        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent, cancellationToken);
        }
    }


    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing) dbContext.Dispose();
            _disposed = true;
        }
    }

}