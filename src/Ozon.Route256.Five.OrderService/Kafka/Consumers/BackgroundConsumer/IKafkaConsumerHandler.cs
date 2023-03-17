namespace Ozon.Route256.Five.OrderService.Kafka.Consumers.BackgroundConsumer;

public interface IKafkaConsumerHandler<in TKey, in TValue, TResult>
{
    public Task<TResult> Handle(TKey key, TValue message, CancellationToken token);
}