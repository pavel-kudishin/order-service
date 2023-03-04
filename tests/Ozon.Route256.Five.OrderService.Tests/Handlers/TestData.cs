using Ozon.Route256.Five.OrderService.Core.Repository.Dto;

namespace Ozon.Route256.Five.OrderService.Tests.Handlers;

public static class TestData
{
    public const int CUSTOMER_ID = 1315;
    public const int ADDRESS_ID = 150;
    public const int REGION_ID = 0;
    public const long ORDER_ID = 20116;

    public static Task<CustomerDto[]> GetTestCustomers()
    {
        return Task.FromResult(new[]
        {
            new CustomerDto(
                CUSTOMER_ID,
                "First",
                "Last",
                "+7 901 300 20 10",
                "test@test.ru",
                ADDRESS_ID,
                new[] { ADDRESS_ID }
            )
        });
    }

    public static Task<AddressDto[]> GetTestAddresses()
    {
        return Task.FromResult(new[]
        {
            new AddressDto(
                ADDRESS_ID,
                "Москва",
                "Перерва",
                "52",
                "127",
                45d,
                45d,
                REGION_ID
            )
        });
    }

    public static Task<RegionDto[]> GetTestRegions()
    {
        return Task.FromResult(new[] { new RegionDto(REGION_ID, "Москва") });
    }

    public static Task<AggregateOrdersDto[]> GetTestAggregateOrders()
    {
        return Task.FromResult(new[]
        {
            new AggregateOrdersDto
            {
            }
        });
    }

    public static Task<OrderDto?> GetTestOrder()
    {
        return Task.FromResult<OrderDto?>(new OrderDto(
            ORDER_ID,
            5,
            200.5m,
            320,
            OrderTypesDto.Delivery,
            DateTime.UtcNow,
            REGION_ID,
            "Created",
            CUSTOMER_ID,
            ADDRESS_ID,
            "+7 905 200 30 40"
        ));
    }

    public static Task<CustomerDto?> GetTestCustomer()
    {
        return Task.FromResult<CustomerDto?>(new CustomerDto(
            CUSTOMER_ID,
            "First",
            "Last",
            "+7 901 300 20 10",
            "test@test.ru",
            ADDRESS_ID,
            new[] { ADDRESS_ID }
        ));
    }

    public static Task<AddressDto?> GetTestAddress()
    {
        return Task.FromResult<AddressDto?>(new AddressDto(
            ADDRESS_ID,
            "Москва",
            "Перерва",
            "52",
            "127",
            45d,
            45d,
            REGION_ID
        ));
    }

    public static Task<RegionDto?> GetTestRegion()
    {
        return Task.FromResult<RegionDto?>(new RegionDto(REGION_ID, "Москва"));
    }

    public static Task<OrderDto[]> GetTestOrders()
    {
        return Task.FromResult(new[]
        {
            new OrderDto(
                ORDER_ID,
                2,
                3,
                4,
                OrderTypesDto.Pickup,
                DateTime.UtcNow,
                REGION_ID,
                "Delivered",
                CUSTOMER_ID,
                ADDRESS_ID,
                "7 910 300 00 10"
            )
        });
    }
}
