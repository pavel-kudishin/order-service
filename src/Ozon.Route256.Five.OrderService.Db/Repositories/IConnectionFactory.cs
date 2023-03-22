using Npgsql;

namespace Ozon.Route256.Five.OrderService.Db.Repositories;

public interface IConnectionFactory
{
    Task<NpgsqlConnection> GetConnection();
}