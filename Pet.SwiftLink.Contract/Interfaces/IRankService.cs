using Pet.SwiftLink.Contract.Model;

namespace Pet.SwiftLink.Contract.Interfaces;

public interface IRankService
{
    Task RecordAsync(Guid itemId);
    Task<IEnumerable<LinkRank>> GetTopItemsAsync(int count);
    Task<LinkRank> GetItemStatsAsync(Guid itemId);
}