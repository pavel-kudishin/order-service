using FluentMigrator;

namespace Ozon.Route256.Five.OrderService.Db.Migrations;

[Migration(3, description: "orders table added")]
public class Migration0003 : Migration
{
    public override void Up()
    {
        Create.Table("orders")
            .WithColumn("id").AsInt64().NotNullable().PrimaryKey()
            .WithColumn("goods_count").AsInt32().NotNullable()
            .WithColumn("price").AsDecimal().NotNullable()
            .WithColumn("weight").AsDecimal().NotNullable()
            .WithColumn("order_source").AsByte().NotNullable()
            .WithColumn("date_created").AsDateTime().NotNullable()
            .WithColumn("state").AsByte().NotNullable()
            .WithColumn("phone").AsString(30).NotNullable()
            .WithColumn("customer_id").AsInt32().NotNullable().Indexed()
            .WithColumn("address_city").AsString(100).NotNullable()
            .WithColumn("address_street").AsString(150).NotNullable()
            .WithColumn("address_building").AsString(30).NotNullable()
            .WithColumn("address_apartment").AsString(30).NotNullable()
            .WithColumn("address_region").AsString(100).NotNullable()
            .WithColumn("address_latitude").AsDouble().NotNullable()
            .WithColumn("address_longitude").AsDouble().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("orders");
    }
}