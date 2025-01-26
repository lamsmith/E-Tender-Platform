using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace RFQService.Infrastructure.Cache
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisCacheService> _logger;

        public RedisCacheService(
            IDistributedCache cache,
            ILogger<RedisCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            try
            {
                var cachedValue = await _cache.GetStringAsync(key);
                if (string.IsNullOrEmpty(cachedValue))
                    return default;

                return JsonSerializer.Deserialize<T>(cachedValue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving from cache for key: {Key}", key);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expirationTime = null)
        {
            try
            {
                var options = new DistributedCacheEntryOptions();
                if (expirationTime.HasValue)
                    options.AbsoluteExpirationRelativeToNow = expirationTime;

                var serializedValue = JsonSerializer.Serialize(value);
                await _cache.SetStringAsync(key, serializedValue, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache for key: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _cache.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing from cache for key: {Key}", key);
            }
        }
    }
}