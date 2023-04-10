using System.Text.Json.Serialization;

namespace Ozon.Route256.Five.OrderService.Rest.Dto;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderStateDto
{
    Created,
    SentToCustomer,
    Delivered,
    Lost,
    Cancelled
}