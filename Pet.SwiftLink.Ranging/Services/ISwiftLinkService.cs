using Pet.SwiftLink.Domain.Model;

namespace Pet.SwiftLink.Application.Interfaces
{
    public interface ISwiftLinkService
    {
        IEnumerable<QuickLink> GetQuickLinks();

        Task RecordAsync(QuickLink link);

        Task RecordAsync(IEnumerable<QuickLink> links);
    }
}
