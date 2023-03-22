using FluentMigrator;

namespace Ozon.Route256.Five.OrderService.Db.Migrations;

[Migration(2, description: "regions table added")]
public class Migration0002 : Migration
{
    public override void Up()
    {

        Create.Table("regions")
            .WithColumn("id").AsInt32().NotNullable().PrimaryKey()
            .WithColumn("name").AsString(100).NotNullable()
            .WithColumn("warehouse_id").AsInt32().NotNullable()
            .ForeignKey("warehouses", "id");

        Insert.IntoTable("regions")
            .Row(new { id = 1, name = "Moscow", warehouse_id = 11 })
            .Row(new { id = 2, name = "StPetersburg", warehouse_id = 12 })
            .Row(new { id = 3, name = "Novosibirsk", warehouse_id = 13 });
    }

    public override void Down()
    {
        Delete.Table("regions");
    }
}