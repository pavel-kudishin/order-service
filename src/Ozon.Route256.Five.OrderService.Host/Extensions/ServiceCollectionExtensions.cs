using System.Text.Json;
using System.Text.Json.Serialization;
using Confluent.Kafka;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Ozon.Route256.Five.OrderService.Core.Exceptions;
using Ozon.Route256.Five.OrderService.Core.Extensions;
using Ozon.Route256.Five.OrderService.Core.Handlers;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrderEvents;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrderEvents.Dto;
using Ozon.Route256.Five.OrderService.Core.Handlers.PreOrders;
using Ozon.Route256.Five.OrderService.Core.Handlers.PreOrders.Dto;
using Ozon.Route256.Five.OrderService.Core.Logging;
using Ozon.Route256.Five.OrderService.Db.Extensions;
using Ozon.Route256.Five.OrderService.Grpc.Extensions;
using Ozon.Route256.Five.OrderService.Host.BackgroundServices;
using Ozon.Route256.Five.OrderService.Kafka.Extensions;
using Ozon.Route256.Five.OrderService.Kafka.Settings;
using Ozon.Route256.Five.OrderService.Redis.Extensions;
using Ozon.Route256.Five.OrderService.Rest.Extensions;

namespace Ozon.Route256.Five.OrderService.Host.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddTelemetry()
            .AddRest()
            .SetupGrpc(configuration)
            .AddHandlers()
            .AddHostedService<SdConsumerHostedService>()
            .AddRedis(configuration)
            .AddKafka(configuration)
            .AddSwaggerGen()
            .AddDb(configuration)
            .AddDbStore();

        return services;
    }

    public static IServiceCollection AddTelemetry(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .WithTracing(
                builder =>
                {
                    ResourceBuilder resourceBuilder = ResourceBuilder.CreateDefault().AddService("OrderService");
                    builder
                        .SetResourceBuilder(resourceBuilder)
                        .AddSource(OrderActivitySourceConfig.SOURCE_NAME)
                        .AddAspNetCoreInstrumentation()
                        .AddNpgsql()
                        .AddConsoleExporter()
                        .AddJaegerExporter(
                            options =>
                            {
                                options.AgentHost = "host.docker.internal";
                                options.AgentPort = 6831;
                                options.Protocol = JaegerExportProtocol.UdpCompactThrift;
                                options.ExportProcessorType = ExportProcessorType.Simple;
                            });
                });

        return services;
    }

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

        services.AddScoped<IDistanceValidator, DistanceValidator>();

        return services;
    }

    private static IServiceCollection AddConsumer<TKey, TMessage, THandler, TResult>(
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