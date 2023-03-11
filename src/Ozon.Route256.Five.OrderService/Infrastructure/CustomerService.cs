using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Ozon.Route256.Five.OrderService.Core;
using Ozon.Route256.Five.OrderService.Core.Repository.Dto;
using Ozon.Route256.Five.OrderService.Grpc.Extensions;

namespace Ozon.Route256.Five.OrderService.Infrastructure;

public class CustomerService : ICustomerService
{
    private readonly Customers.CustomersClient _client;

    public CustomerService(Customers.CustomersClient client)
    {
        _client = client;
    }

    public async Task<CustomerDto?> GetCustomerAsync(int customerId, CancellationToken token)
    {
        GetCustomerByIdRequest request = new GetCustomerByIdRequest()
        {
            Id = customerId,
        };
        try
        {
            Customer customer = await _client.GetCustomerAsync(request, null, null, token);
            CustomerDto customerDto = customer.ToCustomerDto();
            return customerDto;
        }
        catch (RpcException e)
        {
            if (e.StatusCode == StatusCode.NotFound)
            {
                return null;
            }
            throw;
        }
    }

    public async Task<CustomerDto[]> GetCustomersAsync(CancellationToken token)
    {
        Empty request = new Empty();
        GetCustomersResponse customers = await _client.GetCustomersAsync(request, null, null, token);
        CustomerDto[] customerDto = customers.Customers.ToCustomersDto();
        return customerDto;
    }
}