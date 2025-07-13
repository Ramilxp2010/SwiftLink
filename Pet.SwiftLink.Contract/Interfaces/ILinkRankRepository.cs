using Pet.SwiftLink.Domain.Model;

namespace Pet.SwiftLink.Domain.Interfaces;

public interface ILinkRankRepository : IGenericRepository<LinkRank, Guid>
{
    Task<LinkRank> GetOrCreateAsync(Guid itemId);

    Task UpdateAsync(LinkRank stat);

    Task<IEnumerable<LinkRank>> GetTopItemsAsync(int count);

    Task<LinkRank> IncrementClickAsync(Guid itemId, DateTime timestamp);
}