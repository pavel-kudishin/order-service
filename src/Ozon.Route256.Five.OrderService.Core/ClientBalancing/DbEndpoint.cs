namespace Ozon.Route256.Five.OrderService.Core.ClientBalancing;

public record DbEndpoint(string Host, int Port, DbReplicaType DbReplica, int[] Buckets);