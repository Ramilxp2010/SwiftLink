using Pet.SwiftLink.Domain.Interfaces;
using Pet.SwiftLink.Domain.Model;

namespace Pet.SwiftLink.Infrastructure.Repositories
{
    internal class JsonSwiftLinkRepository : JsonGenericRepository<QuickLink, Guid>, ISwiftLinkRepository
    {
        public JsonSwiftLinkRepository(string filePath = "quicklinks.json") 
            : base(filePath, quickLink => quickLink.Id)
        {
        }

        public override void Add(QuickLink entity)
        {
            DataSource.AddOrUpdate(entity.Id, entity, (id, old) => old);
        }
    }
}
