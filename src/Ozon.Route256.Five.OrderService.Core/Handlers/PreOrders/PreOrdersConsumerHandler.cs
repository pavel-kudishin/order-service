using Microsoft.Extensions.Logging;
using Ozon.Route256.Five.OrderService.Core.Handlers.PreOrders.Dto;
using Ozon.Route256.Five.OrderService.Core.Logging;
using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;
using Ozon.Route256.Five.OrderService.Domain.Repository;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.PreOrders;

public sealed class PreOrdersConsumerHandler: IKafkaConsumerHandler<string, PreOrderDto, PreOrdersConsumerHandlerResult>
{
    private readonly ILogger<PreOrdersConsumerHandler> _logger;
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IRegionRepository _regionRepository;
    private readonly INewOrdersKafkaPublisher _newOrdersKafkaPublisher;
    private readonly IDistanceValidator _validator;

    public PreOrdersConsumerHandler(
        ILogger<PreOrdersConsumerHandler> logger,
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
        IRegionRepository regionRepository,
        INewOrdersKafkaPublisher newOrdersKafkaPublisher,
        IDistanceValidator validator)
    {
        _logger = logger;
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _regionRepository = regionRepository;
        _newOrdersKafkaPublisher = newOrdersKafkaPublisher;
        _validator = validator;
    }

    public async Task<PreOrdersConsumerHandlerResult> Handle(
        string key, PreOrderDto message, CancellationToken token)
    {
        int customerId = message.Customer.Id;
        CustomerBo? customerBo = await _customerRepository.Find(customerId, token);

        if (customerBo == null)
        {
            _logger.LogCustomerNotFound(customerId);
            return PreOrdersConsumerHandlerResult.CustomerNotFound;
        }

        OrderBo orderBo = message.ToOrderBo(customerBo);

        RegionBo? region = await _regionRepository.Find(orderBo.Address!.Region, token);

        if (region == null)
        {
            _logger.LogRegionNotFound(orderBo.Address.Region);
            return PreOrdersConsumerHandlerResult.RegionNotFound;
        }

        double latitude = message.Customer.Address.Latitude % 180; // Приходят случайные координаты из customer-service
        double longitude = message.Customer.Address.Longitude % 180; // Подровняем случайные координаты

        bool isValid = _validator.IsValid(
            latitude,
            longitude,
            region.Warehouse.Latitude,
            region.Warehouse.Longitude);

        if (isValid == false)
        {
            // Если расстояние более 5000, то заказ не валидиный
            _logger.LogInvalidOrder(message.Id);
            return PreOrdersConsumerHandlerResult.InvalidOrder;
        }

        await _orderRepository.Insert(orderBo, token);

        NewOrderDto order = new(orderBo.Id);
        await _newOrdersKafkaPublisher.PublishToKafka(order, token);

        _logger.LogOrderPublished(message.Id);

        return PreOrdersConsumerHandlerResult.Success;
    }
}