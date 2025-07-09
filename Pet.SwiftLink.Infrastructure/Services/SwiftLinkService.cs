using Pet.SwiftLink.Contract.Interfaces;
using Pet.SwiftLink.Contract.Model;

namespace Pet.SwiftLink.Infrastructure.Implementation
{
    public class SwiftLinkService : ISwiftLinkService
    {
        private readonly ISwiftLinkRepository _repository;

        public SwiftLinkService(ISwiftLinkRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<QuickLink> GetQuickLinks()
        {
            return _repository.GetAll();
        }

        public async Task RecordAsync(QuickLink link)
        {
            _repository.Add(link);

            await Task.CompletedTask;
        }

        public async Task RecordAsync(IEnumerable<QuickLink> links)
        {
            foreach (var link in links)
            {
                await RecordAsync(link);
            }
        }
    }
}
