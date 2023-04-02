namespace Ozon.Route256.Five.OrderService.Core.Handlers;

public interface IKafkaConsumerHandler<in TKey, in TValue, TResult>
{
    public Task<TResult> Handle(TKey key, TValue message, CancellationToken token);
}