using Pet.SwiftLink.Domain.Interfaces;
using Pet.SwiftLink.Domain.Model;

namespace Pet.SwiftLink.Infrastructure.Repositories;

public class JsonRankRepository : JsonGenericRepository<LinkRank, Guid>, ILinkRankRepository
{
    public JsonRankRepository(string filePath = "statistics.json")
        : base(filePath, linkRank => linkRank.ItemId)
    {
    }

    public async Task<LinkRank> GetOrCreateAsync(Guid itemId)
    {
        if (DataSource.TryGetValue(itemId, out var existing))
            return existing;

        var newEntry = new LinkRank
        {
            ItemId = itemId,
            ClickCount = 0,
            LastClicked = DateTime.MinValue,
            DailyStats = new Dictionary<DateTime, int>()
        };

        var added = DataSource.GetOrAdd(itemId, newEntry);

        if (ReferenceEquals(added, newEntry))
            await SaveAsync();

        return added;
    }

    public async Task UpdateAsync(LinkRank stat)
    {
        DataSource.AddOrUpdate(stat.ItemId, stat, (id, existing) =>
        {
            existing.ClickCount = stat.ClickCount;
            existing.LastClicked = stat.LastClicked;
            existing.DailyStats = stat.DailyStats;
            return existing;
        });

        await SaveAsync();
    }

    public Task<IEnumerable<LinkRank>> GetTopItemsAsync(int count)
    {
        var top = DataSource.Values
            .OrderByDescending(s => s.ClickCount)
            .Take(count)
            .AsEnumerable();

        return Task.FromResult(top);
    }

    public override void Add(LinkRank entity)
    {
        DataSource.AddOrUpdate(entity.ItemId, entity, (id, old) => entity);
    }

    public async Task<LinkRank> IncrementClickAsync(Guid itemId, DateTime timestamp)
    {
        var today = timestamp.Date;

        var updated = DataSource.AddOrUpdate(itemId,
            id => new LinkRank
            {
                ItemId = id,
                ClickCount = 1,
                LastClicked = timestamp,
                DailyStats = new Dictionary<DateTime, int>
                {
                    [today] = 1
                }
            },
            (id, existing) =>
            {
                existing.ClickCount++;
                existing.LastClicked = timestamp;

                if (existing.DailyStats.ContainsKey(today))
                    existing.DailyStats[today]++;
                else
                    existing.DailyStats[today] = 1;

                return existing;
            });

        await SaveAsync();
        return updated;
    }
}