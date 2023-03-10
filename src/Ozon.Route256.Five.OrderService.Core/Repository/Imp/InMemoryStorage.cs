using System.Collections.Concurrent;
using System.Linq;
using Ozon.Route256.Five.OrderService.Core.Repository.Dto;

namespace Ozon.Route256.Five.OrderService.Core.Repository.Imp;

public class InMemoryStorage
{
    private const int INITIAL_REGIONS_COUNT = 3;
    private const int INITIAL_CUSTOMERS_COUNT = 50;
    private const int INITIAL_ADDRESSES_COUNT = INITIAL_CUSTOMERS_COUNT * 5;
    private const int INITIAL_ORDERS_COUNT = INITIAL_CUSTOMERS_COUNT * 3;

    public readonly ConcurrentDictionary<long, OrderDto> Orders =
        new(concurrencyLevel: 3, capacity: INITIAL_ORDERS_COUNT);

    public readonly ConcurrentDictionary<int, RegionDto> Regions =
        new(concurrencyLevel: 3, capacity: INITIAL_REGIONS_COUNT);

    public readonly ConcurrentDictionary<long, AddressDto> Addresses =
        new(concurrencyLevel: 3, capacity: INITIAL_ADDRESSES_COUNT);

    public readonly ConcurrentDictionary<long, CustomerDto> Customers =
        new(concurrencyLevel: 3, capacity: INITIAL_CUSTOMERS_COUNT);

    public InMemoryStorage()
    {
        FillRegions();
        FillAddresses();
        FillCustomers();
        FillOrders();
    }

    private RegionDto GetRandomRegion()
    {
        return Regions[Faker.RandomNumber.Next(0, Regions.Count - 1)];
    }

    private AddressDto GetRandomAddress()
    {
        return Addresses[Faker.RandomNumber.Next(1, Addresses.Count)];
    }

    private CustomerDto GetRandomCustomer()
    {
        return Customers[Faker.RandomNumber.Next(1, Customers.Count)];
    }

    private void FillCustomers()
    {
        IEnumerable<CustomerDto> customers = Enumerable
            .Range(1, INITIAL_CUSTOMERS_COUNT)
            .Select(id =>
            {
                int addressesCount = Faker.RandomNumber.Next(1, 5);
                int[] addresses = Enumerable
                    .Range(1, addressesCount)
                    .Select(i => GetRandomAddress().Id)
                    .ToArray();

                return new CustomerDto(
                    Id: id,
                    FirstName: Faker.Name.First(),
                    LastName: Faker.Name.Last(),
                    MobileNumber: Faker.Phone.Number(),
                    Email: Faker.Internet.Email(),
                    AddressId: addresses[Faker.RandomNumber.Next(0, addresses.Length - 1)],
                    Addresses: addresses
                );
            });

        foreach (CustomerDto customer in customers)
        {
            Customers[customer.Id] = customer;
        }
    }

    private void FillAddresses()
    {
        IEnumerable<AddressDto> addresses = Enumerable
            .Range(1, INITIAL_ADDRESSES_COUNT)
            .Select(id => new AddressDto(
                Id: id,
                City: Faker.Address.City(),
                Street: Faker.Address.StreetName(),
                Building: Faker.RandomNumber.Next(1, 100).ToString(),
                Apartment: Faker.RandomNumber.Next(1, 200).ToString(),
                Latitude: Faker.RandomNumber.Next(40, 55) + Faker.RandomNumber.Next() / (double)int.MaxValue,
                Longitude: Faker.RandomNumber.Next(40, 55) + Faker.RandomNumber.Next() / (double)int.MaxValue,
                RegionId: GetRandomRegion().Id
            ));

        foreach (AddressDto address in addresses)
        {
            Addresses[address.Id] = address;
        }
    }

    private void FillOrders()
    {
        OrderTypesDto[] orderTypes = { OrderTypesDto.Pickup, OrderTypesDto.Delivery };
        string[] orderStates = { "Created", "SentToCustomer", "Delivered", "Lost", "Cancelled" };

        IEnumerable<OrderDto> orders = Enumerable
            .Range(1, INITIAL_ORDERS_COUNT)
            .Select(id =>
            {
                AddressDto addressDto = GetRandomAddress();

                return new OrderDto(
                    Id: id,
                    ArticlesCount: Faker.RandomNumber.Next(1, 5),
                    TotalPrice: Faker.RandomNumber.Next(200 * 100, 10_000 * 100) / 100m,
                    TotalWeight: Faker.RandomNumber.Next(100, 5_000) / 1000m,
                    OrderType: orderTypes[Faker.RandomNumber.Next(0, 1)],
                    DateCreated: DateTime.UtcNow
                        .AddMinutes(-Faker.RandomNumber.Next(0, 60 * 24 * 100)), // 100 days
                    RegionId: addressDto.RegionId,
                    Status: orderStates[Faker.RandomNumber.Next(0, orderStates.Length - 1)],
                    CustomerId: GetRandomCustomer().Id,
                    AddressId: addressDto.Id,
                    Phone: Faker.Phone.Number()
                );
            });

        foreach (OrderDto order in orders)
        {
            Orders[order.Id] = order;
        }
    }

    private void FillRegions()
    {
        string[] regions = { "Moscow", "StPetersburg", "Novosibirsk" };

        for (int i = 0; i < regions.Length; i++)
        {
            string name = regions[i];
            Regions[i] = new RegionDto(i, name);
        }
    }
}