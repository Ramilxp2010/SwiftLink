using log4net;
using Pet.SwiftLink.Application.Interfaces;
using Pet.SwiftLink.Domain.Interfaces;
using Pet.SwiftLink.Domain.Model;

namespace Pet.SwiftLink.Application.Services;

public class RankService : IRankService
{
    private readonly ILinkRankRepository _repository;
    private readonly ILog _logger = LogManager.GetLogger(typeof(RankService));

    public RankService(ILinkRankRepository repository)
    {
        _repository = repository;
    }

    public async Task RecordAsync(Guid itemId)
    {
        try
        {
            var timestamp = DateTime.UtcNow;
            await _repository.IncrementClickAsync(itemId, timestamp);
        }
        catch (Exception ex)
        {
            _logger.Error($"Error recording click for item {itemId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<LinkRank>> GetTopItemsAsync(int count)
    {
        try
        {
            return await _repository.GetTopItemsAsync(count);
        }
        catch (Exception ex)
        {
            _logger.Error($"Error getting top items (count={count})", ex);
            throw;
        }
    }

    public async Task<LinkRank> GetItemStatsAsync(Guid itemId)
    {
        try
        {
            return await _repository.GetOrCreateAsync(itemId);
        }
        catch (Exception ex)
        {
            _logger.Error($"Error getting stats for item {itemId}", ex);
            throw;
        }
    }
}
