using System.Linq.Expressions;

namespace Pet.SwiftLink.Contract.Interfaces
{
    public interface IGenericRepository<TEntity, TKey>
    {
        void Add(TEntity entity);

        TEntity? GetById(TKey id);

        IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate);

        IEnumerable<TEntity> GetAll();

        void Delete(TKey id);

    }
}
