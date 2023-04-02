using System.Text.Json.Serialization;

namespace Ozon.Route256.Five.OrderService.Rest.Dto;

/// <summary>
/// Направление сортировки
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderingDirectionDto
{
    /// <summary>
    /// Sorts the items in an ascending way, from smallest to largest.
    /// </summary>
    Asc,
    /// <summary>
    /// Sorts the items in an descending way, from largest to smallest.
    /// </summary>
    Desc,
}