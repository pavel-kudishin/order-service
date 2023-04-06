using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;

namespace Ozon.Route256.Five.OrderService.Domain.Repository;

public interface IRegionRepository
{
    Task<RegionBo?> Find(string name, CancellationToken token);
    Task<RegionBo[]> GetAll(CancellationToken token);
    Task<RegionBo[]> FindMany(IEnumerable<string> names, CancellationToken token);

}