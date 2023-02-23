using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Ozon.Route256.Five.Orders.Grpc;

namespace Ozon.Route256.Five.OrderService.GrpcServices;

public sealed class OrdersService : Orders.Grpc.Orders.OrdersBase
{
	public override Task<GetOrderResponse> GetOrder(GetOrderRequest request, ServerCallContext context)
	{
		if (request.Id > 10)
		{
			throw new RpcException(new Status(StatusCode.NotFound, $"Заказ #{request.Id} не найден"));
		}

		Random random = new Random();
		return Task.FromResult(new GetOrderResponse()
		{
			Id = request.Id,
			ArticlesCount = random.Next(1, 20),
			CustomerName = "Иванов Иван",
			DateCreated = new DateTime(2023, 2, 25, 18, 25, 10, DateTimeKind.Utc).ToTimestamp(),
			OrderType = OrderTypes.Pickup,
			Phone = 7_905_200_40_50,
			Region = new Region()
			{
				Id = 1020,
				Name = "Саратовская обл.",
			},
			Status = "Создан",
			TotalPrice = random.Next(1000, 100_000) / 100d,
			TotalWeight = random.Next(100, 5_000) / 1000d,
			DeliveryAddress = new Orders.Grpc.Address()
			{
				Region = "Саратовская обл.",
				City = "Саратов",
				Street = "Рабочая",
				Building = "3/14",
				Apartment = "3а",
				Latitude = 54.23234545345,
				Longitude = 47.15456098456,
			},
		});
	}
}