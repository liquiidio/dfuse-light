using DeepReader.Types.EosTypes;
using DeepReader.Types.Helpers;
using Microsoft.Extensions.Caching.Memory;

namespace DeepReader.Classes
{
    public static class NameCache 
    {
        private static MemoryCache _cache = new MemoryCache(new MemoryCacheOptions()
        {
            SizeLimit = 1024,
            ExpirationScanFrequency = TimeSpan.FromMinutes(5)
        });

        private static readonly MemoryCacheEntryOptions _cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(1));

        public static Name GetOrCreate(ReadOnlyMemory<char> chars)
        {
            var bytes = Decoder.HexToBytes(chars.Span);
            var key = BitConverter.ToUInt64(bytes);
            if (!_cache.TryGetValue(key, out Name cacheEntry))// Look for cache key.
            {
                // Key not in cache, so get data.
                cacheEntry = new Name(key, bytes.ToArray());

                // Save data in cache.
                _cache.Set(key, cacheEntry, _cacheEntryOptions);
            }
            return cacheEntry;
        }

        public static Name GetOrCreate(byte[] bytes)
        {
            var key = BitConverter.ToUInt64(bytes);
            if (!_cache.TryGetValue(key, out Name cacheEntry))// Look for cache key.
            {
                // Key not in cache, so get data.
                cacheEntry = new Name(key, bytes);

                // Save data in cache.
                _cache.Set(key, cacheEntry, _cacheEntryOptions);
            }
            return cacheEntry;
        }

        public static Name GetOrCreate(ulong key)
        {
            if (!_cache.TryGetValue(key, out Name cacheEntry))// Look for cache key.
            {
                var bytes = BitConverter.GetBytes(key);
                // Key not in cache, so get data.
                cacheEntry = new Name(key, bytes);

                // Save data in cache.
                _cache.Set(key, cacheEntry, _cacheEntryOptions);
            }
            return cacheEntry;
        }
    }
}
