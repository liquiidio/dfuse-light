using DeepReader.Types.EosTypes;
using DeepReader.Types.Helpers;
using Microsoft.Extensions.Caching.Memory;

namespace DeepReader.Classes
{
    internal class NameCache 
    {
        private MemoryCache _cache = new MemoryCache(new MemoryCacheOptions()
        {
            SizeLimit = 1024,
            ExpirationScanFrequency = TimeSpan.FromMinutes(1)
        });

        private MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(2));

        public Name GetOrCreate(ReadOnlyMemory<char> chars)
        {
            var bytes = Decoder.HexToBytes(chars.Span);
            var key = BitConverter.ToUInt64(bytes);
            if (!_cache.TryGetValue(key, out Name cacheEntry))// Look for cache key.
            {
                // Key not in cache, so get data.
                cacheEntry = new Name(key, chars.ToString(), bytes.ToArray());

                // Save data in cache.
                _cache.Set(key, cacheEntry, cacheEntryOptions);
            }
            return cacheEntry;
        }

        public Name GetOrCreate(byte[] bytes)
        {
            var key = BitConverter.ToUInt64(bytes);
            if (!_cache.TryGetValue(key, out Name cacheEntry))// Look for cache key.
            {

                // Key not in cache, so get data.
                cacheEntry = new Name(key, SerializationHelper.ByteArrayToNameString(bytes), bytes);

                // Save data in cache.
                _cache.Set(key, cacheEntry, cacheEntryOptions);
            }
            return cacheEntry;
        }

        public Name GetOrCreate(ulong key)
        {
            if (!_cache.TryGetValue(key, out Name cacheEntry))// Look for cache key.
            {
                var bytes = BitConverter.GetBytes(key);
                // Key not in cache, so get data.
                cacheEntry = new Name(key, SerializationHelper.ByteArrayToNameString(bytes), bytes);

                // Save data in cache.
                _cache.Set(key, cacheEntry, cacheEntryOptions);
            }
            return cacheEntry;
        }
    }
}
