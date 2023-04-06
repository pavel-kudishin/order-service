using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;

namespace Ozon.Route256.Five.OrderService.Tests;

public static class TestData
{
    public const int CUSTOMER_ID = 1315;
    public const string REGION_NAME = "Москва";
    public const long ORDER_ID = 20116;

    public static OrderBo GetTestOrder()
    {
        return new OrderBo(
            ORDER_ID,
            5,
            200.5m,
            320,
            OrderSourceBo.WebSite,
            DateTime.UtcNow,
            OrderStateBo.Created,
            GetTestCustomer(),
            GetTestAddress(),
            "+7 905 200 30 40"
        );
    }

    public static CustomerBo GetTestCustomer()
    {
        return new CustomerBo(
            CUSTOMER_ID,
            "First",
            "Last",
            "+7 901 300 20 10",
            "test@test.ru",
            GetTestAddress(),
            GetTestAddresses()
        );
    }

    public static AddressBo GetTestAddress()
    {
        return new AddressBo(
            "Москва",
            "Перерва",
            "52",
            "127",
            45d,
            45d,
            GetTestRegion().Name
        );
    }

    public static RegionBo GetTestRegion()
    {
        return new RegionBo(REGION_NAME, GetTestWarehouse());
    }

    public static WarehouseBo GetTestWarehouse()
    {
        return new WarehouseBo(55.729595, 37.639303);
    }

    public static OrderBo[] GetTestOrders()
    {
        return new[] { GetTestOrder() };
    }

    public static CustomerBo[] GetTestCustomers()
    {
        return new[] { GetTestCustomer() };
    }

    public static AddressBo[] GetTestAddresses()
    {
        return new[] { GetTestAddress() };
    }

    public static RegionBo[] GetTestRegions()
    {
        return new[] { GetTestRegion() };
    }

    public static AggregatedOrdersBo[] GetTestAggregateOrders()
    {
        return new[] { new AggregatedOrdersBo() };
    }
}
