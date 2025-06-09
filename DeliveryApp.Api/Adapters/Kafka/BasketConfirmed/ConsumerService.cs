using Confluent.Kafka;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using DeliveryApp.Infrastructure;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using BasketConfirmed;

namespace DeliveryApp.Api.Adapters.Kafka.BasketConfirmed;

public class ConsumerService(IOptions<Settings> settings, IServiceProvider serviceProvider) : BackgroundService
{
    private readonly IConsumer<Ignore, string> _consumer =
        new ConsumerBuilder<Ignore, string>(
            new ConsumerConfig
            {
                BootstrapServers = settings.Value.MessageBrokerHost,
                GroupId = "DeliveryConsumerGroup",
                EnableAutoOffsetStore = false,
                EnableAutoCommit = true,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnablePartitionEof = true
            }).Build();
    
    private readonly string _topic = settings.Value.BasketConfirmedTopic;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _consumer.Subscribe(_topic);
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                var consumeResult = _consumer.Consume(cancellationToken);

                if (consumeResult.IsPartitionEOF) continue;

                var stocksChangedIntegrationEvent =
                    JsonConvert.DeserializeObject<BasketConfirmedIntegrationEvent>(consumeResult.Message.Value);
                
                var command = new CreateOrderCommand(
                    Guid.Parse(stocksChangedIntegrationEvent.BasketId),
                    stocksChangedIntegrationEvent.Address.Street);
                
                using var scope = serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                
                var response = await mediator.Send(command, cancellationToken);
                if (!response)
                {
                    Console.WriteLine("Error");
                }

                try
                {
                    _consumer.StoreOffset(consumeResult);
                }
                catch (KafkaException e)
                {
                    Console.WriteLine($"Kafka error: {e.Error.Reason}");
                }
            }
        }
        catch (OperationCanceledException e)
        {
            Console.WriteLine(e.Message);
        }

    }
}