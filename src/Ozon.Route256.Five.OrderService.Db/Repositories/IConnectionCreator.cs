using Npgsql;

namespace Ozon.Route256.Five.OrderService.Db.Repositories;

public interface IConnectionCreator
{
    Task<NpgsqlConnection> GetConnection();
}