using FluentMigrator;

namespace Ozon.Route256.Five.OrderService.Db.Migrations;

[Migration(5, description: "index_order_id_region_name table added")]
public class Migration0005 : Migration
{
    public override void Up()
    {
        Create
            .Table("index_order_id_region_name")
            .WithColumn("region_name").AsString(100).NotNullable()
            .WithColumn("order_id").AsInt32().NotNullable()
            .WithColumn("order_source").AsByte().NotNullable()
            .WithColumn("date_created").AsDateTime().NotNullable();

        Create
            .Index()
            .OnTable("index_order_id_region_name")
            .OnColumn("region_name").Ascending()
            .OnColumn("date_created").Ascending()
            .OnColumn("order_source").Ascending();
    }

    public override void Down()
    {
        Delete.Table("index_order_id_region_name");
    }
}