using Dapper;
using Npgsql;
using NpgsqlTypes;
using Ozon.Route256.Five.OrderService.Core.Repository;
using Ozon.Route256.Five.OrderService.Core.Repository.Dto;

namespace Ozon.Route256.Five.OrderService.Db.Repositories;

public sealed class RegionDbRepository : IRegionRepository
{
    private readonly IConnectionCreator _connectionCreator;

    public RegionDbRepository(IConnectionCreator connectionCreator)
    {
        _connectionCreator = connectionCreator;
    }

    public async Task<RegionDto?> Find(string name, CancellationToken token)
    {
        await using NpgsqlConnection connection = await _connectionCreator.GetConnection();

        const string SQL = @"
        SELECT
            name AS Name,
            warehouse_coordinates AS Coordinates
        FROM
            regions
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
        await using NpgsqlConnection connection = await _connectionCreator.GetConnection();

        const string SQL = @"
        SELECT
            name AS Name,
            warehouse_coordinates AS Coordinates
        FROM
            regions";

        IEnumerable<RegionDto> query = await connection.QueryAsync<string, WarehouseDto, RegionDto>(
            SQL,
            (regionName, warehouse) => new RegionDto(regionName, warehouse),
            splitOn: "Coordinates");
        RegionDto[] regions = query.ToArray();

        return regions;
    }

    public async Task<RegionDto[]> FindMany(IEnumerable<string> names, CancellationToken token)
    {
        await using NpgsqlConnection connection = await _connectionCreator.GetConnection();

        const string SQL = @"
        SELECT
            name AS Name,
            warehouse_coordinates AS Coordinates
        FROM
            regions
        WHERE
            name = ANY(@names)";

        IEnumerable<RegionDto> query = await connection.QueryAsync<string, WarehouseDto, RegionDto>(
            SQL,
            (regionName, warehouse) => new RegionDto(regionName, warehouse),
            new { names },
            splitOn: "Coordinates");

        RegionDto[] regions = query.ToArray();

        return regions;
    }
}