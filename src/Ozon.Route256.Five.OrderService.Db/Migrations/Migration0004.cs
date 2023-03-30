using FluentMigrator;
using FluentMigrator.Postgres;

namespace Ozon.Route256.Five.OrderService.Db.Migrations;

[Migration(4, description: "index_order_id_customer_id table added")]
public class Migration0004 : Migration
{
    public override void Up()
    {
        Create
            .Table("index_order_id_customer_id")
            .WithColumn("customer_id").AsInt64().NotNullable()
            .WithColumn("order_id").AsInt32().NotNullable()
            .WithColumn("date_created").AsDateTime().NotNullable();

        Create
            .Index()
            .OnTable("index_order_id_customer_id")
            .OnColumn("customer_id");
    }

    public override void Down()
    {
        Delete.Table("index_order_id_customer_id");
    }
}