using Ozon.Route256.Five.OrderService.Core.Repository;
using Ozon.Route256.Five.OrderService.Core.Repository.Dto;
using Ozon.Route256.Five.OrderService.Kafka.Consumers.BackgroundConsumer;
using Ozon.Route256.Five.OrderService.Kafka.Producers.NewOrders;

namespace Ozon.Route256.Five.OrderService.Kafka.Consumers.PreOrders;

public class PreOrdersConsumerHandler: IKafkaConsumerHandler<string, PreOrderDto, PreOrdersConsumerHandlerResult>
{
    private readonly ILogger<PreOrdersConsumerHandler> _logger;
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IRegionRepository _regionRepository;
    private readonly INewOrdersKafkaPublisher _newOrdersKafkaPublisher;

    public PreOrdersConsumerHandler(
        ILogger<PreOrdersConsumerHandler> logger,
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
        IRegionRepository regionRepository,
        INewOrdersKafkaPublisher newOrdersKafkaPublisher)
    {
        _logger = logger;
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _regionRepository = regionRepository;
        _newOrdersKafkaPublisher = newOrdersKafkaPublisher;
    }

    public async Task<PreOrdersConsumerHandlerResult> Handle(
        string key, PreOrderDto message, CancellationToken token)
    {
        int customerId = message.Customer.Id;
        CustomerDto? customerDto = await _customerRepository.Find(customerId, token);

        if (customerDto == null)
        {
            _logger.LogError($"Customer #{customerId} not found");
            return PreOrdersConsumerHandlerResult.CustomerNotFound;
        }

        OrderDto orderDto = message.ToOrderDto(customerDto);

        await _orderRepository.Insert(orderDto, token);

        RegionDto? region = await _regionRepository.Find(orderDto.Region, token);

        if (region == null)
        {
            _logger.LogError($"Region {orderDto.Region} not found");
            return PreOrdersConsumerHandlerResult.RegionNotFound;
        }

        double latitude = orderDto.Address.Latitude % 180; // Приходят случайные координаты из customer-service
        double longitude = orderDto.Address.Longitude % 180; // Подровняем случайные координаты
        double distance = Distance(
            latitude,
            longitude,
            region.Warehouse.Latitude,
            region.Warehouse.Longitude);

        const double MAX_DISTANCE_KM = 5_000d;
        if (distance > MAX_DISTANCE_KM)
        {
            // Если расстояние более 5000, то заказ не валидиный
            _logger.LogError($"Invalid order #{message.Id}");
            return PreOrdersConsumerHandlerResult.InvalidOrder;
        }

        NewOrderDto order = new(orderDto.Id);
        await _newOrdersKafkaPublisher.PublishToKafka(order, token);

        return PreOrdersConsumerHandlerResult.Success;
    }

    private static double Distance(double lat1, double lon1, double lat2, double lon2)
    {
        double lat1Radians = DegreesToRadians(lat1);
        double lat2Radians = DegreesToRadians(lat2);
        double thetaRadians = DegreesToRadians(lon1 - lon2);
        double dist = Math.Sin(lat1Radians) * Math.Sin(lat2Radians) +
                      Math.Cos(lat1Radians) * Math.Cos(lat2Radians) * Math.Cos(thetaRadians);
        dist = Math.Acos(dist);
        dist = RadiansToDegrees(dist) * 60 * 1.1515 * 1.609344;
        return dist;

        double DegreesToRadians(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        double RadiansToDegrees(double rad)
        {
            return (rad / Math.PI * 180.0);
        }
    }
}