using JsonNet.ContractResolvers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Primitives;
using Quartz;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Outbox;

public class OutboxMessagesJob(ApplicationDbContext dbContext, IMediator mediator) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        // Получаем все DomainEvents, которые еще не были отправлены (где ProcessedOnUtc == null)
        var outboxMessages = await dbContext.OutboxMessages
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(o => o.OccurredOnUtc)
            .Take(20)
            .ToListAsync(context.CancellationToken);

        // Если такие есть, то перебираем их в цикле
        if (outboxMessages.Any())
        {
            foreach (var outboxMessage in outboxMessages)
            {
                // Настройки сериализатора
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new PrivateSetterContractResolver(),
                    TypeNameHandling = TypeNameHandling.All
                };

                try
                {
                    // Десериализуем запись из OutboxMessages в DomainEvent
                    var domainEvent = JsonConvert.DeserializeObject<DomainEvent>(outboxMessage.Content, settings);
                    Console.WriteLine(domainEvent.ToString());

                    // Отправляем
                    await mediator.Publish(domainEvent, context.CancellationToken);

                    // Если предыдущий метод не вернул ошибку, значит отправка была успешной
                    // Ставим дату отправки, это будет признаком, что сообщение отправлять больше не нужно 
                    outboxMessage.ProcessedOnUtc = DateTime.UtcNow;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            // Сохраняем изменения
            await dbContext.SaveChangesAsync();
        }
    }
}