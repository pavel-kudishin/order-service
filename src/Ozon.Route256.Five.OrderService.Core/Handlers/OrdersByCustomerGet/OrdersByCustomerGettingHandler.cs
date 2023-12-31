using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;
using Ozon.Route256.Five.OrderService.Domain.Repository;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.OrdersByCustomerGet;

internal sealed class OrdersByCustomerGettingHandler : IOrdersByCustomerGettingHandler
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
        CustomerBo? customer = await _customerRepository.Find(request.CustomerId, token);

        if (customer is null)
        {
            return HandlerResult<OrderBo[]>.FromError(
                new CustomerNotFoundException($"Customer #{request.CustomerId} not found"));
        }

        OrderBo[] orders = await _orderRepository.FindByCustomer(
            request.CustomerId, request.StartDate, request.EndDate, request.PageNumber, request.ItemsPerPage, token);

        return HandlerResult<OrderBo[]>.FromValue(orders);
    }
}