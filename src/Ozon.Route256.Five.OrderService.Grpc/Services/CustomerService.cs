using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Ozon.Route256.Five.OrderService.Core.Abstractions;
using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;
using Ozon.Route256.Five.OrderService.Grpc.Extensions;

namespace Ozon.Route256.Five.OrderService.Grpc.Services;

internal sealed class CustomerService : ICustomerService
{
    private readonly Customers.CustomersClient _client;

    public CustomerService(Customers.CustomersClient client)
    {
        _client = client;
    }

    public async Task<CustomerBo?> GetCustomerAsync(int customerId, CancellationToken token)
    {
        GetCustomerByIdRequest request = new()
        {
            Id = customerId,
        };
        try
        {
            Customer customer = await _client.GetCustomerAsync(request, cancellationToken: token);
            CustomerBo customerBo = customer.ToCustomerBo();
            return customerBo;
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

    public async Task<CustomerBo[]> GetCustomersAsync(CancellationToken token)
    {
        Empty request = new();
        GetCustomersResponse response = await _client.GetCustomersAsync(request, cancellationToken: token);
        CustomerBo[] customers = response.Customers.ToCustomersBo();
        return customers;
    }
}