using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace Ozon.Route256.Five.OrderService.Kafka.Producers.NewOrders;

public class NewOrdersKafkaPublisher : INewOrdersKafkaPublisher
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
    }

    public Task PublishToKafka(NewOrderDto dto, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(_newOrderSettings.Topic))
        {
            throw new InfrastructureKafkaException($"Topic for {nameof(NewOrdersKafkaPublisher)} is empty");
        }

        string value = JsonSerializer.Serialize(dto, _jsonSerializerOptions);
        return _kafkaProducer.SendMessage(dto.OrderId.ToString(), value, _newOrderSettings.Topic, token);
    }
}