namespace Interlex.BusinessLayer
{
    using System;
    using System.Runtime.Caching;

    public class InMemoryCache : ICacheService
    {
        public T GetOrSet<T>(string cacheKey, Func<T> getItemCallback, int cacheMinutes) where T : class
        {
            T item = MemoryCache.Default.Get(cacheKey) as T;

            if (item == null)
            {
                item = getItemCallback();
                MemoryCache.Default.Add(cacheKey, item, DateTime.Now.AddMinutes(cacheMinutes));
            }

            return item;
        }

        public T GetOrSetForever<T>(string cacheKey, Func<T> getItemCallback) where T : class
        {
            T item = MemoryCache.Default.Get(cacheKey) as T;
            if (item == null)
            {
                item = getItemCallback();
                MemoryCache.Default.Add(cacheKey, item, MemoryCache.InfiniteAbsoluteExpiration);
            }

            return item;
        }

        public void DeleteCacheItem(string cacheKey)
        {
            MemoryCache.Default.Remove(cacheKey);
        }
    }

    interface ICacheService
    {
        T GetOrSet<T>(string cacheKey, Func<T> getItemCallback, int cacheMinutes) where T : class;
    }
}
