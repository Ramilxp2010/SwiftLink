using Pet.SwiftLink.Contract.Model;

namespace Pet.SwiftLink.Contract.Interfaces
{
    public interface ISwiftLinkService
    {
        IEnumerable<QuickLink> GetQuickLinks();

        Task RecordAsync(QuickLink link);

        Task RecordAsync(IEnumerable<QuickLink> links);
    }
}
