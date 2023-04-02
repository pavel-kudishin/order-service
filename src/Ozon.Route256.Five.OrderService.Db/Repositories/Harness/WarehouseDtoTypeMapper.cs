using System.Data;
using Dapper;
using Npgsql;
using NpgsqlTypes;
using Ozon.Route256.Five.OrderService.Db.Dto;

namespace Ozon.Route256.Five.OrderService.Db.Repositories.Harness;

internal sealed class WarehouseDtoTypeMapper : SqlMapper.TypeHandler<WarehouseDto>
{
    public override void SetValue(IDbDataParameter parameter, WarehouseDto value)
    {
        if (parameter is NpgsqlParameter npgsqlParameter)
        {
            npgsqlParameter.NpgsqlDbType = NpgsqlDbType.Geometry;
            npgsqlParameter.NpgsqlValue = value;
        }
        else
        {
            throw new ArgumentException();
        }
    }

    public override WarehouseDto Parse(object value)
    {
        if (value is NpgsqlPoint point)
        {
            return new WarehouseDto(point.Y, point.X);
        }

        throw new ArgumentException();
    }
}