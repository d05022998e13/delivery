using DeliveryApp.Infrastructure.Adapters.Postgres.Outbox;
using MediatR;
using Newtonsoft.Json;
using Primitives;

namespace DeliveryApp.Infrastructure.Adapters.Postgres;

public class UnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork, IDisposable
{
    private bool _disposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Перекладываем Domain Event в Outbox
        // После выполнения этого метода в DbContext будут находится и сам Aggregate и OutboxMessages
        await SaveDomainEventsInOutboxAsync(cancellationToken);
        
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    private async Task SaveDomainEventsInOutboxAsync(CancellationToken cancellationToken = default)
    {
        var outboxMessages = dbContext.ChangeTracker
            .Entries<IAggregateRoot>() // Получили агрегаты в которых есть доменные события
            .Select(x => x.Entity)
            .SelectMany(aggregate =>
            {
                // Переложили в отдельную переменную
                var domainEvents = aggregate.GetDomainEvents();

                // Очистили Domain Event в самих агрегатах (поскольку далее они будут отправлены и больше не нужны)
                aggregate.ClearDomainEvents();
                return domainEvents;
            }
            )
            .Select(domainEvent => new OutboxMessage
            {
                // Создали объект OutboxMessage на основе Domain Event
                Id = domainEvent.EventId,
                OccurredOnUtc = DateTime.UtcNow,
                Type = domainEvent.GetType().Name,
                Content = JsonConvert.SerializeObject(
                    domainEvent,
                    new JsonSerializerSettings
                    {
                        // Эта настройка нужна, чтобы сериализовать Domain Event с указанием типов
                        // Если ее не указать, то десеарилизатор не поймет в какой тип восстанавоивать сообщение
                        TypeNameHandling = TypeNameHandling.All
                    }
                    )
            })
            .ToList();

        // Добавяляем OutboxMessages в dbContext
        // После выполнения этой строки в DbContext будут находится сам Aggregate и OutboxMessages
        await dbContext.OutboxMessages.AddRangeAsync(outboxMessages, cancellationToken);
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