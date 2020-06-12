namespace Interlex.BusinessLayer
{
    using System;
    using System.Runtime.Caching;

    public static class CacheProvider
    {
        private static InMemoryCache instance;

        public static InMemoryCache Provider 
        {
            get
            {
                if (instance == null)
                {
                    instance = new InMemoryCache();
                }
                return instance;
            }
        }
    }
}