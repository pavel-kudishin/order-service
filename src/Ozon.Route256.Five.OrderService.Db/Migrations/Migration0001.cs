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
    }

    public override void Down()
    {
        Delete.Table("warehouses");
    }
}