using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace AuthNetExample.Features.Notifications.Services;

public class MemoryCacheService
{
    private readonly IDistributedCache _cache;

    public MemoryCacheService(IDistributedCache cache) => _cache = cache;

    public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
    {
        var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration };
        var jsonData = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, jsonData, options);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var jsonData = await _cache.GetStringAsync(key);
        return jsonData is null ? default : JsonSerializer.Deserialize<T>(jsonData);
    }

    public async Task RemoveAsync(string key) => await _cache.RemoveAsync(key);
}