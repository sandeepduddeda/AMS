using System;
using System.Collections.Generic;
using System.Web.Caching;
using System.Web;

namespace AMS.Web.Core
{
    public class CacheService : ICacheService
    {
        private static volatile CacheService _instance;
        private static readonly object _syncRoot = new Object();

        private CacheService()
        {
        }

        public static CacheService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new CacheService();
                    }
                }

                return _instance;
            }
        }

        public T Get<T>(string cacheId, Func<T> getItemCallback)
        {
            if (HttpRuntime.Cache == null)
                return getItemCallback();

            var item = HttpRuntime.Cache.Get(cacheId);
            if (item is T)
                return (T)item;

            item = getItemCallback();
            if (item == null)
                return default(T);

            HttpRuntime.Cache.Insert(cacheId, item, null, DateTime.UtcNow.AddHours(1), Cache.NoSlidingExpiration);
            return (T)item;
        }

        public void Remove(string cacheId)
        {
            if (HttpRuntime.Cache != null)
                HttpRuntime.Cache.Remove(cacheId);
        }
    }

    internal interface ICacheService
    {
        T Get<T>(string cacheId, Func<T> getItemCallback);
    }
}