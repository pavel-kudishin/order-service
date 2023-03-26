using Dapper;
using Ozon.Route256.Five.OrderService.Core.Repository;
using Ozon.Route256.Five.OrderService.Core.Repository.Dto;
using Ozon.Route256.Five.OrderService.Shared.ClientBalancing;
using System.Data.Common;

namespace Ozon.Route256.Five.OrderService.Db.Repositories;

public sealed class RegionDbRepository : IRegionRepository
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly IShardingRule<string> _stringShardingRule;
    private readonly IDbStore _dbStore;

    public RegionDbRepository(
        IConnectionFactory connectionFactory,
        IShardingRule<string> stringShardingRule,
        IDbStore dbStore)
    {
        _connectionFactory = connectionFactory;
        _stringShardingRule = stringShardingRule;
        _dbStore = dbStore;
    }

    public async Task<RegionDto?> Find(string name, CancellationToken token)
    {
        await using DbConnection connection = _connectionFactory.GetConnectionByKey(name);

        const string SQL = @"
        SELECT
            name AS Name,
            w.coordinates AS Coordinates
        FROM
            __bucket__.regions r
        INNER JOIN __bucket__.warehouses w ON r.warehouse_id = w.id
        WHERE
            name = @name
        LIMIT 1";

        IEnumerable<RegionDto> regions = await connection.QueryAsync<string, WarehouseDto, RegionDto>(
            SQL,
            (regionName, warehouse) => new RegionDto(regionName, warehouse),
            new { name },
            splitOn: "Coordinates");

        RegionDto? region = regions.FirstOrDefault();

        return region;
    }

    public async Task<RegionDto[]> GetAll(CancellationToken token)
    {
        const string SQL = @"
        SELECT
            name AS Name,
            w.coordinates AS Coordinates
        FROM
            __bucket__.regions r
        INNER JOIN __bucket__.warehouses w ON r.warehouse_id = w.id";

        int bucketsCount = _dbStore.BucketsCount;

        List<RegionDto> result = new();
        for (int bucketId = 0; bucketId < bucketsCount; bucketId++)
        {
            await using DbConnection connection = _connectionFactory.GetConnectionByBucket(bucketId);

            IEnumerable<RegionDto> query = await connection.QueryAsync<string, WarehouseDto, RegionDto>(
                SQL,
                (regionName, warehouse) => new RegionDto(regionName, warehouse),
                splitOn: "Coordinates");

            result.AddRange(query);
        }

        return result.ToArray();
    }

    public async Task<RegionDto[]> FindMany(IEnumerable<string> names, CancellationToken token)
    {
        Dictionary<int, string[]> bucketToRegionNamesMap =
            names
                .Select(regionName =>
                    (BucketId: _stringShardingRule.GetBucketId(regionName, _dbStore.BucketsCount), RegionName: regionName))
                .GroupBy(x => x.BucketId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.RegionName).ToArray());

        const string SQL = @"
        SELECT
            name AS Name,
            w.coordinates AS Coordinates
        FROM
            __bucket__.regions r
        INNER JOIN __bucket__.warehouses w ON r.warehouse_id = w.id
        WHERE
            name = ANY(@region_names)";

        List<RegionDto> result = new();

        foreach ((int bucketId, string[] namesInBucket) in bucketToRegionNamesMap)
        {
            DynamicParameters param = new();
            param.Add("@region_names", namesInBucket);

            await using DbConnection connection = _connectionFactory.GetConnectionByBucket(bucketId);
            IEnumerable<RegionDto> query = await connection.QueryAsync<string, WarehouseDto, RegionDto>(
                SQL,
                (regionName, warehouse) => new RegionDto(regionName, warehouse),
                param,
                splitOn: "Coordinates");

            result.AddRange(query);
        }

        return result.ToArray();
    }
}