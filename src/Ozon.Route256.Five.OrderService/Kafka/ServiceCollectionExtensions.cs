using System.Text.Json;
using Confluent.Kafka;
using Ozon.Route256.Five.OrderService.Kafka.Consumers;
using Ozon.Route256.Five.OrderService.Kafka.Consumers.BackgroundConsumer;
using Ozon.Route256.Five.OrderService.Kafka.Consumers.OrderEvents;
using Ozon.Route256.Five.OrderService.Kafka.Consumers.PreOrders;
using Ozon.Route256.Five.OrderService.Kafka.Producers;
using Ozon.Route256.Five.OrderService.Kafka.Settings;
using System.Text.Json.Serialization;
using Ozon.Route256.Five.OrderService.Core;

namespace Ozon.Route256.Five.OrderService.Kafka;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafka(this IServiceCollection services, IConfiguration configuration)
    {
        const string SECTION = "Kafka";
        KafkaSettings kafkaSettings = configuration.GetSection(SECTION).Get<KafkaSettings>()
                                      ?? throw new InvalidConfigurationException(SECTION);

        services.AddConsumer<string, PreOrderDto, PreOrdersConsumerHandler, PreOrdersConsumerHandlerResult>(
            configuration,
            ConsumerType.PreOrder,
            kafkaSettings,
            Deserializers.Utf8,
            new KafkaJsonSerializer<PreOrderDto>()
            );

        JsonSerializerOptions jsonSerializerOptions = new()
        {
            Converters = { new JsonStringEnumConverter() }
        };

        services.AddConsumer<string, OrderEventsChangedDto, OrderEventsConsumerHandler, OrderEventsConsumerHandlerResult>(
            configuration,
            ConsumerType.OrderEvents,
            kafkaSettings,
            Deserializers.Utf8,
            new KafkaJsonSerializer<OrderEventsChangedDto>(jsonSerializerOptions)
        );

        services.AddProducer(configuration, kafkaSettings);

        return services;
    }
}