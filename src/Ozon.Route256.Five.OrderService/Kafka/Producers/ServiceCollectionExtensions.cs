using Ozon.Route256.Five.OrderService.Core;
using Ozon.Route256.Five.OrderService.Kafka.Producers.NewOrders;
using Ozon.Route256.Five.OrderService.Kafka.Settings;

namespace Ozon.Route256.Five.OrderService.Kafka.Producers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProducer(
        this IServiceCollection services,
        IConfiguration configuration,
        KafkaSettings kafkaSettings)
    {
        const string KAFKA_PRODUCER_SECTION = "Kafka:Producer";
        ProducerSettings? producerSettings = configuration.GetSection(KAFKA_PRODUCER_SECTION)
            .Get<ProducerSettings>() ?? throw new InvalidConfigurationException(KAFKA_PRODUCER_SECTION);

        services.AddSingleton<IKafkaProducer, KafkaProducer>(sp => new KafkaProducer(
            sp.GetRequiredService<ILogger<KafkaProducer>>(),
            kafkaSettings,
            producerSettings
        ));

        services.Configure<NewOrderSettings>(configuration.GetSection(NewOrderSettings.SECTIONS));
        services.AddScoped<INewOrdersKafkaPublisher, NewOrdersKafkaPublisher>();

        return services;
    }
}