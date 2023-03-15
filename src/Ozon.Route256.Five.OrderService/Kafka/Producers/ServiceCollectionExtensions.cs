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
        ProducerSettings? producerSettings = configuration.GetSection("Kafka:Producer")
            .Get<ProducerSettings>();

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