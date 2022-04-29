using System.Text;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Helpers;
using Microsoft.Extensions.Caching.Memory;

// TODO, circular dependencies made me put this here (into DeepReader.Types) which feels like the wrong place
namespace DeepReader.Types.Other
{
    public static class NameCache
    {
        private static readonly MemoryCache Cache = new(new MemoryCacheOptions()
        {
            ExpirationScanFrequency = TimeSpan.FromMinutes(5)
        });

        private static readonly MemoryCacheEntryOptions ExpiringCacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(1));

        private static readonly MemoryCacheEntryOptions AlwaysKeepExpirationOptions = new MemoryCacheEntryOptions()
            .SetPriority(CacheItemPriority.NeverRemove);

        public static Name GetOrCreate(string name, bool expire = true)
        {
            var key = SerializationHelper.ConvertNameToLong(name);
            if (!Cache.TryGetValue(key, out Name cacheEntry))// Look for cache key.
            {
                var bytes = BitConverter.GetBytes(key);
                // Key not in cache, so get data.
                cacheEntry = new Name(key, bytes.ToArray());
                // Save data in cache.
                Cache.Set(key, cacheEntry, expire ? ExpiringCacheEntryOptions : AlwaysKeepExpirationOptions);
            }
            return cacheEntry;
        }

        public static Name GetOrCreate(byte[] bytes)
        {
            var key = BitConverter.ToUInt64(bytes);
            if (!Cache.TryGetValue(key, out Name cacheEntry))// Look for cache key.
            {
                // Key not in cache, so get data.
                cacheEntry = new Name(key, bytes);

                // Save data in cache.
                Cache.Set(key, cacheEntry, ExpiringCacheEntryOptions);
            }
            return cacheEntry;
        }

        public static Name GetOrCreate(ulong key)
        {
            if (!Cache.TryGetValue(key, out Name cacheEntry))// Look for cache key.
            {
                var bytes = BitConverter.GetBytes(key);
                // Key not in cache, so get data.
                cacheEntry = new Name(key, bytes);

                // Save data in cache.
                Cache.Set(key, cacheEntry, ExpiringCacheEntryOptions);
            }
            return cacheEntry;
        }
    }
}
