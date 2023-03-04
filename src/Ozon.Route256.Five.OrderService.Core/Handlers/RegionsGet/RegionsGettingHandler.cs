using Ozon.Route256.Five.OrderService.Core.BusinessObjects;
using Ozon.Route256.Five.OrderService.Core.Handlers.ResultTypes;
using Ozon.Route256.Five.OrderService.Core.Repository;
using Ozon.Route256.Five.OrderService.Core.Repository.Dto;
using Ozon.Route256.Five.OrderService.Core.Repository.Extensions;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.RegionsGet;

public class RegionsGettingHandler : IRegionsGettingHandler
{
    private readonly IRegionRepository _regionRepository;

    public RegionsGettingHandler(IRegionRepository regionRepository)
    {
        _regionRepository = regionRepository;
    }

    public async Task<HandlerResult<RegionBo[]>> Handle(CancellationToken token)
    {
        RegionDto[] regions = await _regionRepository.GetAll(token);

        return HandlerResult<RegionBo[]>.FromValue(regions.ToRegionsBo());
    }
}