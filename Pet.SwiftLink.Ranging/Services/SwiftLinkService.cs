using Pet.SwiftLink.Application.Interfaces;
using Pet.SwiftLink.Domain.Interfaces;
using Pet.SwiftLink.Domain.Model;

namespace Pet.SwiftLink.Application.Implementation
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
