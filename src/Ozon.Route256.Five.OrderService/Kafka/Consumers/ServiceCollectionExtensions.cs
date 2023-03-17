﻿using Confluent.Kafka;
using Ozon.Route256.Five.OrderService.Core;
using Ozon.Route256.Five.OrderService.Kafka.Consumers.BackgroundConsumer;
using Ozon.Route256.Five.OrderService.Kafka.Settings;

namespace Ozon.Route256.Five.OrderService.Kafka.Consumers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConsumer<TKey, TMessage, THandler, TResult>(
        this IServiceCollection services,
        IConfiguration configuration,
        ConsumerType consumerType,
        KafkaSettings kafkaSettings,
        IDeserializer<TKey> keyDeserializer,
        IDeserializer<TMessage> messageDeserializer)
        where THandler : class, IKafkaConsumerHandler<TKey, TMessage, TResult>
    {
        string section = $"Kafka:Consumer:{consumerType.Name}";
        ConsumerSettings consumerSettings = configuration
            .GetSection(section)
            .Get<ConsumerSettings>() ?? throw new InvalidConfigurationException(section);

        if (consumerSettings.Enabled == false)
        {
            return services;
        }

        services.AddHostedService(sp =>
            new BackgroundKafkaConsumer<TKey, TMessage, THandler, TResult>(
                sp, kafkaSettings, consumerSettings, keyDeserializer, messageDeserializer));

        services.AddScoped<THandler>();

        return services;
    }
}