using System.Text.Json;
using Confluent.Kafka;

namespace Ozon.Route256.Five.OrderService.Kafka.Settings;

public sealed class KafkaJsonSerializer<TValue> : IDeserializer<TValue>
{
    private readonly JsonSerializerOptions? _options;

    public KafkaJsonSerializer(JsonSerializerOptions? options = null)
    {
        _options = options;
    }

    public TValue Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        return JsonSerializer.Deserialize<TValue>(data, _options)!;
    }
}