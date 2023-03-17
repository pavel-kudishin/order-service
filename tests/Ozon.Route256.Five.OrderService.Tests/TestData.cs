using Ozon.Route256.Five.OrderService.Core.Repository.Dto;

namespace Ozon.Route256.Five.OrderService.Tests;

public static class TestData
{
    public const int CUSTOMER_ID = 1315;
    public const string REGION_NAME = "Москва";
    public const long ORDER_ID = 20116;

    public static OrderDto GetTestOrder()
    {
        return new OrderDto(
            ORDER_ID,
            5,
            200.5m,
            320,
            OrderSourceDto.WebSite,
            DateTime.UtcNow,
            GetTestRegion().Name,
            OrderStateDto.Created,
            GetTestCustomer(),
            GetTestAddress(),
            "+7 905 200 30 40"
        );
    }

    public static CustomerDto GetTestCustomer()
    {
        return new CustomerDto(
            CUSTOMER_ID,
            "First",
            "Last",
            "+7 901 300 20 10",
            "test@test.ru",
            GetTestAddress(),
            GetTestAddresses()
        );
    }

    public static AddressDto GetTestAddress()
    {
        return new AddressDto(
            "Москва",
            "Перерва",
            "52",
            "127",
            45d,
            45d,
            GetTestRegion().Name
        );
    }

    public static RegionDto GetTestRegion()
    {
        return new RegionDto(REGION_NAME, GetTestWarehouse());
    }

    public static WarehouseDto GetTestWarehouse()
    {
        return new WarehouseDto(55.729595, 37.639303);
    }

    public static OrderDto[] GetTestOrders()
    {
        return new[] { GetTestOrder() };
    }

    public static CustomerDto[] GetTestCustomers()
    {
        return new[] { GetTestCustomer() };
    }

    public static AddressDto[] GetTestAddresses()
    {
        return new[] { GetTestAddress() };
    }

    public static RegionDto[] GetTestRegions()
    {
        return new[] { GetTestRegion() };
    }

    public static AggregateOrdersDto[] GetTestAggregateOrders()
    {
        return new[] { new AggregateOrdersDto() };
    }
}
