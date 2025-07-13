using Pet.SwiftLink.Application.Interfaces;
using Pet.SwiftLink.Domain.Interfaces;
using Pet.SwiftLink.Domain.Model;

namespace Pet.SwiftLink.Desktop.Services;

public class WpfStatisticTracker : IStatisticTracker
{
    private readonly IRankService _service;

    public WpfStatisticTracker(IRankService service)
    {
        _service = service;
    }

    public async Task TrackClickAsync(Guid itemId)
    {
        await _service.RecordAsync(itemId);
        // Обновляем локальный кэш при необходимости
    }
    
    public async Task<IEnumerable<QuickLink>> OrderByPopularity(IEnumerable<QuickLink> items)
    {
        var stats = await _service.GetTopItemsAsync(int.MaxValue);
        return items.OrderByDescending(x => 
            stats.FirstOrDefault(s => s.ItemId == x.Id)?.ClickCount ?? 0);
    }
}