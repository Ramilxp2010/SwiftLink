using Newtonsoft.Json;
using Pet.SwiftLink.Contract.Interfaces;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Pet.SwiftLink.Infrastructure.Repositories
{
    public abstract class JsonGenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>, IDisposable 
        where TKey:notnull
    {
        private readonly string _filePath;
        protected readonly ConcurrentDictionary<TKey, TEntity> DataSource;
        private readonly Timer _saveTimer;
        private readonly object _fileLock = new();
        private bool _disposed;

        private readonly Func<TEntity, TKey> _keySelector;

        public JsonGenericRepository(string filePath, Func<TEntity, TKey> keySelector)
        {
            _filePath = filePath;
            _keySelector = keySelector;
            _saveTimer = new Timer(SaveToDisk, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));

            DataSource = LoadStatistics();
        }

        public abstract void Add(TEntity entity);

        public void Delete(TKey id)
        {
            DataSource.Remove(id, out var entity);
        }

        public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            return DataSource.Values.ToList().AsQueryable().Where(predicate);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return DataSource.Values;
        }

        public TEntity? GetById(TKey id)
        {
            return DataSource[id];
        }


        private ConcurrentDictionary<TKey, TEntity> LoadStatistics()
        {
            try
            {
                lock (_fileLock)
                {
                    if (!File.Exists(_filePath))
                        return new ConcurrentDictionary<TKey, TEntity>();

                    var json = File.ReadAllText(_filePath);
                    var entities = JsonConvert.DeserializeObject<List<TEntity>>(json);
                    return new ConcurrentDictionary<TKey, TEntity>(
                        entities?.ToDictionary(_keySelector) ?? new Dictionary<TKey, TEntity>());
                }
            }
            catch
            {
                return new ConcurrentDictionary<TKey, TEntity>();
            }
        }


        private void SaveToDisk(object? state)
        {
            if (_disposed) return;

            try
            {
                lock (_fileLock)
                {
                    var json = JsonConvert.SerializeObject(DataSource.Values.ToList());
                    File.WriteAllText(_filePath, json);
                }
            }
            catch
            {
                // Логирование ошибки
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            SaveToDisk(null);
            _saveTimer?.Dispose();
            _disposed = true;
        }
    }
}
