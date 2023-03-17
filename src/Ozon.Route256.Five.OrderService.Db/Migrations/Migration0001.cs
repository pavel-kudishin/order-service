using FluentMigrator;

namespace Ozon.Route256.Five.OrderService.Db.Migrations;

[Migration(1, description: "regions table added")]
public class Migration0001 : Migration
{
    public override void Up()
    {

        Create.Table("regions")
            .WithColumn("id").AsInt32().NotNullable().Identity().PrimaryKey()
            .WithColumn("name").AsString(100).NotNullable();
    }

    public override void Down()
    {
        Delete.Table("regions");
    }
}