using Ozon.Route256.Five.OrderService.Core.BusinessObjects;
using Ozon.Route256.Five.OrderService.Core.Handlers.ResultTypes;
using Ozon.Route256.Five.OrderService.Core.Repository;
using Ozon.Route256.Five.OrderService.Core.Repository.Dto;
using Ozon.Route256.Five.OrderService.Core.Repository.Extensions;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.OrdersByCustomerGet;

public class OrdersByCustomerGettingHandler : IOrdersByCustomerGettingHandler
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IOrderRepository _orderRepository;

    public OrdersByCustomerGettingHandler(
        ICustomerRepository customerRepository,
        IOrderRepository orderRepository)
    {
        _customerRepository = customerRepository;
        _orderRepository = orderRepository;
    }

    public async Task<HandlerResult<OrderBo[]>> Handle(
        IOrdersByCustomerGettingHandler.Request request,
        CancellationToken token)
    {
        CustomerDto? customer = await _customerRepository.Find(request.CustomerId, token);

        if (customer is null)
        {
            return HandlerResult<OrderBo[]>.FromError(
                new CustomerNotFoundException($"Customer #{request.CustomerId} not found"));
        }

        OrderDto[] orders = await _orderRepository.FindByCustomer(
            request.CustomerId, request.StartDate, request.EndDate, request.PageNumber, request.ItemsPerPage, token);

        OrderBo[] ordersBo = orders.ToOrdersBo().ToArray();

        return HandlerResult<OrderBo[]>.FromValue(ordersBo);
    }
}