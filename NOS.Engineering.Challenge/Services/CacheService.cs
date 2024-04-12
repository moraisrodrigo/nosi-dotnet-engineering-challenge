using Microsoft.Extensions.Caching.Memory;
using NOS.Engineering.Challenge.Interfaces;

namespace NOS.Engineering.Challenge.Cache;

public class CacheService<T> : ICacheService<T>
{
  private readonly IMemoryCache _cache;

  public CacheService(IMemoryCache cache)
  {
    _cache = cache;
  }

  public async Task<T?> Get(Guid id)
  {
    return await Task.FromResult(_cache.Get<T>(id));
  }

  public async Task Set(Guid id, T item)
  {
    _cache.Set(id, item);
    await Task.CompletedTask;
  }

  public async Task Remove(Guid id)
  {
    _cache.Remove(id);
    await Task.CompletedTask;
  }
}
