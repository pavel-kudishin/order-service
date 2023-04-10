using System.Text.Json.Serialization;

namespace Ozon.Route256.Five.OrderService.Rest.Dto;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderSourceDto
{
    WebSite = 1,
    Mobile = 2,
    Api = 3
}