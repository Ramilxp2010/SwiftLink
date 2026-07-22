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
            // Normalize legacy records missing Category / IsPinned
            entity.Category = QuickLinkCategories.Normalize(entity.Category);

            DataSource.AddOrUpdate(entity.Id, entity, (_, _) => entity);
            SaveSync();
        }

        public override void Delete(Guid id)
        {
            if (!DataSource.TryRemove(id, out _))
            {
                Log($"Entity with key {id} not found for deletion.");
                return;
            }

            SaveSync();
        }
    }
}
