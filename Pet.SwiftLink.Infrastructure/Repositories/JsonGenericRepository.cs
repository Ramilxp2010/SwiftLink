using Newtonsoft.Json;
using Pet.SwiftLink.Domain.Interfaces;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Pet.SwiftLink.Infrastructure.Repositories;

public abstract class JsonGenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>, IAsyncDisposable
    where TKey : notnull
{
    private readonly string _filePath;
    protected readonly ConcurrentDictionary<TKey, TEntity> DataSource;
    private readonly Timer _saveTimer;
    private readonly SemaphoreSlim _fileSemaphore = new(1, 1);
    private bool _disposed;

    private readonly Func<TEntity, TKey> _keySelector;

    protected JsonGenericRepository(string filePath, Func<TEntity, TKey> keySelector)
    {
        _filePath = filePath;
        _keySelector = keySelector;
        DataSource = LoadDataFromDiskAsync().GetAwaiter().GetResult(); // можно заменить на ленивую загрузку
        _saveTimer = new Timer(_ => SaveDataToDisk(), null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
    }

    public abstract void Add(TEntity entity);

    public void Delete(TKey id)
    {
        if (!DataSource.TryRemove(id, out _))
        {
            Log($"Entity with key {id} not found for deletion.");
        }
    }

    public IEnumerable<TEntity> GetAll()
    {
        return DataSource.Values;
    }

    public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
    {
        return DataSource.Values.AsQueryable().Where(predicate);
    }

    public TEntity? GetById(TKey id)
    {
        return DataSource.TryGetValue(id, out var entity) ? entity : default;
    }

    public async Task SaveAsync()
    {
        if (_disposed) return;

        try
        {
            await _fileSemaphore.WaitAsync();
            var json = JsonConvert.SerializeObject(DataSource.Values.ToList(), Formatting.Indented);
            await File.WriteAllTextAsync(_filePath, json);
        }
        catch (Exception ex)
        {
            Log($"Async save failed: {ex.Message}");
        }
        finally
        {
            _fileSemaphore.Release();
        }
    }

    private async Task<ConcurrentDictionary<TKey, TEntity>> LoadDataFromDiskAsync()
    {
        try
        {
            await _fileSemaphore.WaitAsync();

            if (!File.Exists(_filePath))
                return new ConcurrentDictionary<TKey, TEntity>();

            var json = File.ReadAllText(_filePath);
            var entities = JsonConvert.DeserializeObject<List<TEntity>>(json);
            return new ConcurrentDictionary<TKey, TEntity>(
                entities?.ToDictionary(_keySelector) ?? new Dictionary<TKey, TEntity>());
        }
        catch (Exception ex)
        {
            Log($"Failed to load data from disk: {ex.Message}");
            return new ConcurrentDictionary<TKey, TEntity>();
        }
        finally
        {
            _fileSemaphore.Release();
        }
    }

    private void SaveDataToDisk()
    {
        // Используется таймером: sync-версия
        if (_disposed) return;

        try
        {
            _fileSemaphore.Wait();
            var json = JsonConvert.SerializeObject(DataSource.Values.ToList(), Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            Log($"Sync save failed: {ex.Message}");
        }
        finally
        {
            _fileSemaphore.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;

        _saveTimer?.Dispose();
        await SaveAsync();
        _fileSemaphore.Dispose();
        GC.SuppressFinalize(this);
    }

    protected virtual void Log(string message)
    {
        Console.WriteLine($"[{DateTime.Now}] [JsonRepository] {message}");
    }
}
