using FluentMigrator;

namespace Ozon.Route256.Five.OrderService.Db.Migrations;

[Migration(2, description: "regions table altered")]
public class Migration0002 : Migration
{
    public override void Up()
    {
        Alter.Table("regions")
            .AddColumn("warehouse_coordinates").AsCustom("point").NotNullable();

        Insert.IntoTable("regions")
            .Row(new { name = "Moscow", warehouse_coordinates = "(37.639303,55.729595)" })
            .Row(new { name = "StPetersburg", warehouse_coordinates = "(30.318006,59.938703)" })
            .Row(new { name = "Novosibirsk", warehouse_coordinates = "(82.925218,55.030488)" });
    }

    public override void Down()
    {
        Delete.FromTable("regions").AllRows();

        Delete.Column("warehouse_coordinates").FromTable("regions");
    }
}