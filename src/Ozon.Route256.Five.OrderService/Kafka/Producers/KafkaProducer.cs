using Confluent.Kafka;
using Ozon.Route256.Five.OrderService.Kafka.Settings;

namespace Ozon.Route256.Five.OrderService.Kafka.Producers;

public class KafkaProducer: IDisposable, IKafkaProducer
{
    private readonly ILogger<KafkaProducer> _logger;
    private readonly IProducer<string, string> _producer;

    public KafkaProducer(ILogger<KafkaProducer> logger, KafkaSettings kafkaSettings, ProducerSettings producerSettings)
    {
        _logger = logger;
        ProducerConfig config = new ProducerConfig
        {
            BootstrapServers = kafkaSettings.BootstrapServers,
            Acks = producerSettings.Acks,
            EnableIdempotence = producerSettings.EnableIdempotence
        };
        _producer = new ProducerBuilder<string, string>(config)
            .Build();
    }

    public async Task SendMessage(string key, string value, string topic, CancellationToken token)
    {
        try
        {
            Message<string, string> message = new Message<string, string>
            {
                Key = key,
                Value = value
            };
            await _producer.ProduceAsync(topic, message, token);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in {Topic} producer", topic);
            throw;
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _producer.Flush();
        _producer.Dispose();
    }
}