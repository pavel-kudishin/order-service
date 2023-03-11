using Confluent.Kafka;
using Ozon.Route256.Five.OrderService.Kafka.Settings;

namespace Ozon.Route256.Five.OrderService.Kafka.Consumers.BackgroundConsumer;

public class BackgroundKafkaConsumer<TKey, TMessage, THandler, TResult>
    : BackgroundService where THandler : IKafkaConsumerHandler<TKey, TMessage, TResult>
{
    private readonly ILogger<BackgroundKafkaConsumer<TKey, TMessage, THandler, TResult>> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IDeserializer<TKey> _keyDeserializer;
    private readonly IDeserializer<TMessage> _messageDeserializer;
    private readonly ConsumerConfig _consumerConfig;
    private readonly string _topic;
    private readonly TimeSpan _timeoutForRetry;

    public BackgroundKafkaConsumer(
        IServiceProvider serviceProvider,
        KafkaSettings kafkaSettings,
        ConsumerSettings consumerSettings,
        IDeserializer<TKey> keyDeserializer,
        IDeserializer<TMessage> messageDeserializer)
    {
        _logger = serviceProvider.GetRequiredService<ILogger<BackgroundKafkaConsumer<TKey, TMessage, THandler, TResult>>>();
        _serviceProvider = serviceProvider;
        _keyDeserializer = keyDeserializer;
        _messageDeserializer = messageDeserializer;
        _timeoutForRetry = TimeSpan.FromSeconds(kafkaSettings.TimeoutForRetryInSeconds);

        _consumerConfig = new ConsumerConfig()
        {
            GroupId = kafkaSettings.GroupId,
            BootstrapServers = kafkaSettings.BootstrapServers,
            EnableAutoCommit = consumerSettings.AutoCommit,

        };

        if (string.IsNullOrWhiteSpace(consumerSettings.Topic))
        {
            throw new InfrastructureKafkaException("Topic is empty");
        }

        _topic = consumerSettings.Topic;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Workaround to avoid blocking of main host starting
        await Task.Yield();

        while (stoppingToken.IsCancellationRequested == false)
        {
            try
            {
                await Consume(stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in topic {Topic} during kafka consume", _topic);
                await Task.Delay(_timeoutForRetry, stoppingToken);
            }
        }
    }

    private async Task Consume(CancellationToken token)
    {
        using IConsumer<TKey, TMessage>? consumer =
            new ConsumerBuilder<TKey, TMessage>(_consumerConfig)
                .SetValueDeserializer(_messageDeserializer)
                .SetKeyDeserializer(_keyDeserializer)
                .Build();

        consumer.Subscribe(_topic);
        _logger.LogInformation("Success subscribe to {Topic}", _topic);

        while (token.IsCancellationRequested == false)
        {
            ConsumeResult<TKey, TMessage> consumeResult = consumer.Consume(token);
            await _serviceProvider
                .CreateScope()
                .ServiceProvider
                .GetRequiredService<THandler>()
                .Handle(consumeResult.Message.Key, consumeResult.Message.Value, token);

            consumer.Commit();
        }
    }
}