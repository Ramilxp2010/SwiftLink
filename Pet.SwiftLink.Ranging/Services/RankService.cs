using log4net;
using log4net.Repository.Hierarchy;
using Pet.SwiftLink.Contract.Interfaces;
using Pet.SwiftLink.Contract.Model;

namespace Pet.SwiftLink.Ranging.Services;

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
            var stat = await _repository.GetOrCreateAsync(itemId);
            
            stat.ClickCount++;
            stat.LastClicked = DateTime.UtcNow;
            UpdateDailyStats(stat);
            
            await _repository.UpdateAsync(stat);
        }
        catch (Exception ex)
        {
            _logger.Error($"Error recording click for item {itemId}");
            throw;
        }
    }

    public async Task<IEnumerable<LinkRank>> GetTopItemsAsync(int count)
    {
        return await _repository.GetTopItemsAsync(count);
    }

    public async Task<LinkRank> GetItemStatsAsync(Guid itemId)
    {
        return await _repository.GetOrCreateAsync(itemId);
    }

    private void UpdateDailyStats(LinkRank stat)
    {
        var today = DateTime.UtcNow.Date;
        stat.DailyStats.TryGetValue(today, out var todayCount);
        stat.DailyStats[today] = todayCount + 1;
    }
}