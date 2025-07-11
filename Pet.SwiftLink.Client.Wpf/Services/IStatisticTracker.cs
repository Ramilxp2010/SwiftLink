using Pet.SwiftLink.Contract.Model;

namespace Pet.SwiftLink.Desktop.Services;

public interface IStatisticTracker
{
    Task TrackClickAsync(Guid itemId);
    Task<IEnumerable<QuickLink>> OrderByPopularity(IEnumerable<QuickLink> items);
}