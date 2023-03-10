using System.Text.Json.Serialization;

namespace Ozon.Route256.Five.OrderService.Rest.Dto;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderTypesDto
{
    Pickup = 0,
    Delivery = 1,
}