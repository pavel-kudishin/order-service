using System.Collections.Concurrent;
using Ozon.Route256.Five.OrderService.Core.Repository.Dto;

namespace Ozon.Route256.Five.OrderService.Core.Repository.Imp;

public class InMemoryStorage
{
    public readonly ConcurrentDictionary<string, RegionDto> Regions =
        new(concurrencyLevel: 3, capacity: 3);

    public InMemoryStorage()
    {
        FillRegions();
    }

    private void FillRegions()
    {
        RegionDto[] regions =
            {
                new("Moscow", new WarehouseDto(55.729595, 37.639303)),
                new("StPetersburg", new WarehouseDto(59.938703, 30.318006)),
                new("Novosibirsk", new WarehouseDto(55.030488, 82.925218)),
            };

        foreach (RegionDto region in regions)
        {
            Regions[region.Name] = region;
        }
    }
}