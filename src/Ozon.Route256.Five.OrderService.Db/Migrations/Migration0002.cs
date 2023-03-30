using FluentMigrator;
using Microsoft.Extensions.Options;
using Ozon.Route256.Five.OrderService.Db.Repositories;

namespace Ozon.Route256.Five.OrderService.Db.Migrations;

[Migration(2, description: "regions table added")]
public class Migration0002 : Migration
{
    private readonly IShardingRule<string> _stringShardingRule;
    private readonly MigrationSettings _migrationSettings;

    public Migration0002(
        IOptions<MigrationSettings> options,
        IShardingRule<string> stringShardingRule)
    {
        _stringShardingRule = stringShardingRule;
        _migrationSettings = options.Value;
    }

    public override void Up()
    {

        Create.Table("regions")
            .WithColumn("id").AsInt32().NotNullable().PrimaryKey()
            .WithColumn("name").AsString(100).NotNullable()
            .WithColumn("warehouse_id").AsInt32().NotNullable()
            .ForeignKey("warehouses", "id");

        var warehouses = new[]
        {
            new { id = 11, coordinates = "(37.639303,55.729595)" },
            new { id = 12, coordinates = "(30.318006,59.938703)" },
            new { id = 13, coordinates = "(82.925218,55.030488)" },
        };

        var regions = new[]
        {
            new { id = 1, name = "Moscow", warehouse_id = 11 },
            new { id = 2, name = "StPetersburg", warehouse_id = 12 },
            new { id = 3, name = "Novosibirsk", warehouse_id = 13 },
        };

        foreach (var region in regions)
        {
            int bucketId = _stringShardingRule.GetBucketId(region.name, _migrationSettings.BucketsCount);
            if (bucketId == _migrationSettings.BucketId)
            {
                var warehouse = warehouses.First(w => w.id == region.warehouse_id);
                Insert.IntoTable("warehouses").Row(warehouse);
                Insert.IntoTable("regions").Row(region);
            }
        }
    }

    public override void Down()
    {
        Delete.Table("regions");
    }
}