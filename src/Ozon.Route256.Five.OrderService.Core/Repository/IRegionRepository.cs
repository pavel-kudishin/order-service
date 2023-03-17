using Ozon.Route256.Five.OrderService.Core.Repository.Dto;

namespace Ozon.Route256.Five.OrderService.Core.Repository;

public interface IRegionRepository
{
    Task<RegionDto?> Find(string name, CancellationToken token);
    Task<RegionDto[]> GetAll(CancellationToken token);
    Task<RegionDto[]> FindMany(IEnumerable<string> names, CancellationToken token);

}