using Realtorist.Services.Abstractions.Cache;

namespace Realtorist.Services.Implementations.Default.Cache
{
    /// <summary>
    /// Provides factory for creating in-memory cache provider with LFU strategy
    /// </summary>
    public class InMemoryLFUCacheFactory: ICacheFactory
    {
        public ICache<T, V> GetCache<T, V>(int capacity)
        {
            return new InMemoryLFUCache<T, V>(capacity);
        }
    }
}
