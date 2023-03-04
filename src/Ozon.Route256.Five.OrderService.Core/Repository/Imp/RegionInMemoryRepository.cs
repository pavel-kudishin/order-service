using Ozon.Route256.Five.OrderService.Core.Repository.Dto;

namespace Ozon.Route256.Five.OrderService.Core.Repository.Imp;

public class RegionInMemoryRepository: IRegionRepository
{
    private readonly InMemoryStorage _inMemoryStorage;

    public RegionInMemoryRepository(InMemoryStorage inMemoryStorage)
    {
        _inMemoryStorage = inMemoryStorage;
    }

    public Task<RegionDto?> Find(int id, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<RegionDto?>(token);
        }

        _inMemoryStorage.Regions.TryGetValue(id, out RegionDto? region);

        return Task.FromResult(region).WaitAsync(token);
    }

    public Task<RegionDto[]> GetAll(CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<RegionDto[]>(token);
        }

        return Task.FromResult(_inMemoryStorage.Regions.Values.ToArray()).WaitAsync(token);
    }

    public Task<RegionDto[]> FindMany(IEnumerable<int> ids, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<RegionDto[]>(token);
        }

        RegionDto[] regions = FindDto(ids, token).ToArray();
        return Task.FromResult(regions).WaitAsync(token);
    }

    private IEnumerable<RegionDto> FindDto(IEnumerable<int> ids, CancellationToken token)
    {
        foreach (int id in ids.Distinct())
        {
            token.ThrowIfCancellationRequested();

            if (_inMemoryStorage.Regions.TryGetValue(id, out RegionDto? region))
            {
                yield return region;
            }
        }
    }

}