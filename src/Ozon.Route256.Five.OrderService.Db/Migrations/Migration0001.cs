using FluentMigrator;

namespace Ozon.Route256.Five.OrderService.Db.Migrations;

[Migration(1, description: "warehouses table added")]
public class Migration0001 : Migration
{
    public override void Up()
    {
        Create.Table("warehouses")
            .WithColumn("id").AsInt32().NotNullable().PrimaryKey()
            .WithColumn("coordinates").AsCustom("point").NotNullable();

        Insert.IntoTable("warehouses")
            .Row(new { id = 11, coordinates = "(37.639303,55.729595)" })
            .Row(new { id = 12, coordinates = "(30.318006,59.938703)" })
            .Row(new { id = 13, coordinates = "(82.925218,55.030488)" });
    }

    public override void Down()
    {
        Delete.Table("warehouses");
    }
}