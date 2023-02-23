using System.Text.Json.Serialization;

namespace Ozon.Route256.Five.OrderService.Dto;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderTypes
{
	Pickup = 0,
	Delivery = 1,
}