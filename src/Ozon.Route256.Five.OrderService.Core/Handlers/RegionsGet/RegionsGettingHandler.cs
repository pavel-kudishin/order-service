using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;
using Ozon.Route256.Five.OrderService.Domain.Repository;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.RegionsGet;

internal sealed class RegionsGettingHandler : IRegionsGettingHandler
{
    private readonly IRegionRepository _regionRepository;

    public RegionsGettingHandler(IRegionRepository regionRepository)
    {
        _regionRepository = regionRepository;
    }

    public async Task<HandlerResult<RegionBo[]>> Handle(CancellationToken token)
    {
        RegionBo[] regions = await _regionRepository.GetAll(token);

        return HandlerResult<RegionBo[]>.FromValue(regions);
    }
}