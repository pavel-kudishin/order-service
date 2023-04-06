using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Ozon.Route256.Five.OrderService.Core.Handlers.PreOrders;
using Ozon.Route256.Five.OrderService.Kafka.Exceptions;

namespace Ozon.Route256.Five.OrderService.Kafka.Producers.NewOrders;

internal sealed class NewOrdersKafkaPublisher : INewOrdersKafkaPublisher
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly IKafkaProducer _kafkaProducer;
    private readonly NewOrderSettings _newOrderSettings;

    public NewOrdersKafkaPublisher(
        IKafkaProducer kafkaProducer,
        IOptionsSnapshot<NewOrderSettings> optionsSnapshot)
    {
        _kafkaProducer = kafkaProducer;
        _newOrderSettings = optionsSnapshot.Value;

        if (string.IsNullOrWhiteSpace(_newOrderSettings.Topic))
        {
            throw new InfrastructureKafkaException($"Topic for {nameof(NewOrdersKafkaPublisher)} is empty");
        }
    }

    public Task PublishToKafka(NewOrderDto dto, CancellationToken token)
    {
        string value = JsonSerializer.Serialize(dto, _jsonSerializerOptions);
        return _kafkaProducer.SendMessage(dto.OrderId.ToString(), value, _newOrderSettings.Topic, token);
    }
}