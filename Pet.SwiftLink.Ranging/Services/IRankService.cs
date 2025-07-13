using Pet.SwiftLink.Domain.Model;

namespace Pet.SwiftLink.Application.Interfaces;

public interface IRankService
{
    Task RecordAsync(Guid itemId);
    Task<IEnumerable<LinkRank>> GetTopItemsAsync(int count);
    Task<LinkRank> GetItemStatsAsync(Guid itemId);
}