using Pet.SwiftLink.Contract.Interfaces;
using Pet.SwiftLink.Contract.Model;

namespace Pet.SwiftLink.Infrastructure.Repositories;

public class JsonRankRepository : JsonGenericRepository<LinkRank, Guid>, ILinkRankRepository
{
    public JsonRankRepository(string filePath = "statistics.json")
        : base(filePath, linkRank => linkRank.ItemId)
    {
    }

    public Task<LinkRank> GetOrCreateAsync(Guid itemId)
    {
        return Task.FromResult(DataSource.GetOrAdd(itemId, id => new LinkRank
        {
            ItemId = id,
            ClickCount = 0,
            LastClicked = DateTime.MinValue,
            DailyStats = new Dictionary<DateTime, int>()
        }));
    }

    public Task UpdateAsync(LinkRank stat)
    {
        DataSource.AddOrUpdate(stat.ItemId, stat, (id, existing) =>
        {
            existing.ClickCount = stat.ClickCount;
            existing.LastClicked = stat.LastClicked;
            existing.DailyStats = stat.DailyStats;
            return existing;
        });

        return Task.CompletedTask;
    }

    public Task<IEnumerable<LinkRank>> GetTopItemsAsync(int count)
    {
        return Task.FromResult(DataSource.Values
            .OrderByDescending(s => s.ClickCount)
            .Take(count)
            .AsEnumerable());
    }

    public override void Add(LinkRank entity)
    {
        DataSource.AddOrUpdate(entity.ItemId, entity, (id, old) => entity);
    }
}