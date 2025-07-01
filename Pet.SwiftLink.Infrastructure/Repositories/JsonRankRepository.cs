using System.Collections.Concurrent;
using Newtonsoft.Json;
using Pet.SwiftLink.Contract.Interfaces;
using Pet.SwiftLink.Contract.Model;

namespace Pet.SwiftLink.Infrastructure.Repositories;

public class JsonRankRepository : ILinkRankRepository, IDisposable
{
    private readonly string _filePath;
    private readonly ConcurrentDictionary<Guid, LinkRank> _statistics;
    private readonly Timer _saveTimer;
    private readonly object _fileLock = new();
    private bool _disposed;

    public JsonRankRepository(string filePath = "statistics.json")
    {
        _filePath = filePath;
        _statistics = LoadStatistics();
        _saveTimer = new Timer(SaveToDisk, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
    }

    public Task<LinkRank> GetOrCreateAsync(Guid itemId)
    {
        return Task.FromResult(_statistics.GetOrAdd(itemId, id => new LinkRank
        {
            ItemId = id,
            ClickCount = 0,
            LastClicked = DateTime.MinValue,
            DailyStats = new Dictionary<DateTime, int>()
        }));
    }

    public Task UpdateAsync(LinkRank stat)
    {
        _statistics.AddOrUpdate(stat.ItemId, stat, (id, existing) =>
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
        return Task.FromResult(_statistics.Values
            .OrderByDescending(s => s.ClickCount)
            .Take(count)
            .AsEnumerable());
    }

    private ConcurrentDictionary<Guid, LinkRank> LoadStatistics()
    {
        try
        {
            lock (_fileLock)
            {
                if (!File.Exists(_filePath))
                    return new ConcurrentDictionary<Guid, LinkRank>();

                var json = File.ReadAllText(_filePath);
                var stats = JsonConvert.DeserializeObject<List<LinkRank>>(json);
                return new ConcurrentDictionary<Guid, LinkRank>(
                    stats?.ToDictionary(s => s.ItemId) ?? new Dictionary<Guid, LinkRank>());
            }
        }
        catch
        {
            // В случае ошибки возвращаем чистый словарь
            return new ConcurrentDictionary<Guid, LinkRank>();
        }
    }

    private void SaveToDisk(object? state)
    {
        if (_disposed) return;

        try
        {
            lock (_fileLock)
            {
                var json = JsonConvert.SerializeObject(_statistics.Values.ToList());
                File.WriteAllText(_filePath, json);
            }
        }
        catch
        {
            // Логирование ошибки
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        SaveToDisk(null); // Сохраняем перед закрытием
        _saveTimer?.Dispose();
        _disposed = true;
    }
}