using Confluent.Kafka;
using DeliveryApp.Core.Domain.Models.OrderAggregate.DomainEvents;
using DeliveryApp.Core.Ports;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderStatusChanged;
using OrderStatus = DeliveryApp.Core.Domain.Models.OrderAggregate.OrderStatus;

namespace DeliveryApp.Infrastructure.Adapters.Kafka.OrderStatusChanged;

public class ProducerService(IOptions<Settings> options) : IMessageBusProducer
{
    private readonly ProducerConfig _config = new()
    {
        BootstrapServers = options.Value.MessageBrokerHost
    };
    private readonly string _topicName = options.Value.OrderStatusChangedTopic;

    
    public async Task Publish(OrderStatusChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        // Перекладываем данные из Domain Event в Integration Event
        var integrationEvent = new OrderStatusChangedIntegrationEvent
        {
            OrderId = notification.Order.Id.ToString(),
            OrderStatus = ConvertStatus(notification.Order.Status.Name),
        };

        // Создаем сообщение для Kafka
        var message = new Message<string, string>
        {
            Key = notification.EventId.ToString(),
            Value = JsonConvert.SerializeObject(integrationEvent)
        };

        try
        {
            // Отправляем сообщение в Kafka
            using var producer = new ProducerBuilder<string, string>(_config).Build();
            var dr = await producer.ProduceAsync(_topicName, message, cancellationToken);
            Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
        }
        catch (ProduceException<Null, string> e)
        {
            Console.WriteLine($"Delivery failed: {e.Error.Reason}");
        }
    }

    private static global::OrderStatusChanged.OrderStatus ConvertStatus(string statusName)
    {
        if (statusName == OrderStatus.Created.Name)
        {
            return global::OrderStatusChanged.OrderStatus.Created;
        }
        
        if (statusName == OrderStatus.Assigned.Name)
        {
            return global::OrderStatusChanged.OrderStatus.Assigned;
        }
        
        if (statusName == OrderStatus.Completed.Name)
        {
            return global::OrderStatusChanged.OrderStatus.Completed;
        }
        
        else return global::OrderStatusChanged.OrderStatus.None;
    }
}