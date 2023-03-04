using FluentAssertions;
using Moq;
using Ozon.Route256.Five.OrderService.Core.BusinessObjects;
using Ozon.Route256.Five.OrderService.Core.Handlers.CustomersGet;
using Ozon.Route256.Five.OrderService.Core.Handlers.ResultTypes;
using Ozon.Route256.Five.OrderService.Core.Repository;

namespace Ozon.Route256.Five.OrderService.Tests.Handlers;

public class CustomersGettingHandlerTests
{
    [Fact]
    public async Task GetAllCustomers()
    {
        Mock<IRegionRepository> regionRepository = new();
        regionRepository.Setup(repo => repo.GetAll(CancellationToken.None)).Returns(TestData.GetTestRegions());
        Mock<IAddressRepository> addressRepository = new();
        addressRepository.Setup(repo => repo.GetAll(CancellationToken.None)).Returns(TestData.GetTestAddresses());
        Mock<ICustomerRepository> customerRepository = new();
        customerRepository.Setup(repo => repo.GetAll(CancellationToken.None)).Returns(TestData.GetTestCustomers());

        CustomersGettingHandler handler =
            new(customerRepository.Object, regionRepository.Object, addressRepository.Object);

        HandlerResult<CustomerBo[]> result = await handler.Handle(CancellationToken.None);

        result.Success.Should().BeTrue();
        Assert.NotNull(result.Value);
        result.Value.Length.Should().Be(1);
    }
}