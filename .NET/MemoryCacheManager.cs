public static class MemoryCacheManager
    {
        private const string CacheName = "cache-name";
        private static MemoryCache memoryCache = new MemoryCache(CacheName);

        public static T CacheAddOrGet<T>(string settingName, Lazy<T> newValueFactory)
        {
            // Get the existing cache member or if not available add new value. 
            // AddOrGetExisting is a thread-safe atomic operation which handles the locking mechanism for thread safty. 
            // To use the operation, the value is passes in as Lazy initialization to only be calculated when the key is not in the cache and to be atomic and thread-safe.
            Lazy<T> cachedValue = memoryCache.AddOrGetExisting(
                               settingName,
                               newValueFactory,
                               ObjectCache.InfiniteAbsoluteExpiration) as Lazy<T>;
            try
            {
                // For the first time adding the cache entry, cachedValue is set to null so the lazy object will be initialized
                return (cachedValue ?? newValueFactory).Value;
            }
            catch
            {
                // Evict from cache the secret that caused the exception to throw
                memoryCache.Remove(settingName);
                throw;
            }
        }
    }