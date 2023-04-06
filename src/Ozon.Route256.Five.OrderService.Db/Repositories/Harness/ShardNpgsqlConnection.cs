using Npgsql;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace Ozon.Route256.Five.OrderService.Db.Repositories.Harness;

internal sealed class ShardNpgsqlConnection : DbConnection
{
    private readonly NpgsqlConnection _npgsqlConnection;

    public ShardNpgsqlConnection(
        NpgsqlConnection npgsqlConnection,
        int bucketId)
    {
        _npgsqlConnection = npgsqlConnection;
        BucketId = bucketId;
    }

    public int BucketId { get; }

    protected override DbTransaction BeginDbTransaction(
        IsolationLevel isolationLevel) => _npgsqlConnection.BeginTransaction(isolationLevel);

    public override void ChangeDatabase(
        string databaseName) => _npgsqlConnection.ChangeDatabase(databaseName);

    public override void Close() => _npgsqlConnection.Close();

    public override void Open() => _npgsqlConnection.Open();

    [AllowNull]
    public override string ConnectionString
    {
        get => _npgsqlConnection.ConnectionString;
        set => _npgsqlConnection.ConnectionString = value;
    }

    public override string Database => _npgsqlConnection.Database;
    public override ConnectionState State => _npgsqlConnection.State;
    public override string DataSource => _npgsqlConnection.DataSource;
    public override string ServerVersion => _npgsqlConnection.ServerVersion;

    protected override DbCommand CreateDbCommand()
    {
        NpgsqlCommand command = _npgsqlConnection.CreateCommand();
        return new ShardNpgsqlCommand(command, BucketId);
    }
}