using TrfrtSbmt.Domain;

namespace TrfrtSbmt.Api.Features.Search;

public class ResultCache
{
    private readonly Dictionary<string, CacheItem> _cache = new Dictionary<string, CacheItem>();

    public void Store(string key, List<BaseEntity> value, TimeSpan expiresAfter)
    {
        _cache[key] = new CacheItem(value, expiresAfter);
    }

    public List<BaseEntity>? Get(string key)
    {
        if (!_cache.ContainsKey(key)) return null;
        var cached = _cache[key];
        if (DateTimeOffset.Now - cached.Created >= cached.ExpiresAfter)
        {
            _cache.Remove(key);
            return null;
        }
        return cached.Value;
    }
}

public class CacheItem
{
    public CacheItem(List<BaseEntity> value, TimeSpan expiresAfter)
    {
        Value = value;
        ExpiresAfter = expiresAfter;
    }
    public List<BaseEntity> Value { get; }
    internal DateTimeOffset Created { get; } = DateTimeOffset.Now;
    internal TimeSpan ExpiresAfter { get; }
}
