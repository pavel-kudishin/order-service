namespace Ozon.Route256.Five.OrderService.Core.BusinessObjects;

public class RegionBo
{
    public string Name { get; init; } = string.Empty;
    public WarehouseBo? Warehouse { get; init; }
}