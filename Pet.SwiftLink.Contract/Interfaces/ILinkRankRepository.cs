using Pet.SwiftLink.Contract.Model;

namespace Pet.SwiftLink.Contract.Interfaces;

public interface ILinkRankRepository
{
    Task<LinkRank> GetOrCreateAsync(Guid itemId);
    Task UpdateAsync(LinkRank stat);
    Task<IEnumerable<LinkRank>> GetTopItemsAsync(int count);
}