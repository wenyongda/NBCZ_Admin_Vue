using System;
using System.Collections.Generic;
using SqlSugar;

namespace NBCZ.DBUtility.DbHelper
{
    public class SqlSugarCache : ICacheService
    {
        public void Add<V>(string key, V value)
        {
            // RedisServer.Cache.Set(key, value, 3600 + RedisHelper.RandomExpired(5, 30));
            CacheHelper.SetCache(key, value);
        }

        public void Add<V>(string key, V value, int cacheDurationInSeconds)
        {
            // RedisServer.Cache.Set(key, value, cacheDurationInSeconds);
            CacheHelper.SetCaches(key, value, cacheDurationInSeconds);
        }

        public bool ContainsKey<V>(string key)
        {
            // return RedisServer.Cache.Exists(key);
            return CacheHelper.Exists(key);
        }

        public V Get<V>(string key)
        {
            // return RedisServer.Cache.Get<V>(key);
            return (V)CacheHelper.Get(key);
        }

        public IEnumerable<string> GetAllKey<V>()
        {
            // var keys = RedisServer.Cache.Keys("cache:SqlSugarDataCache.*");
            // // return RedisServer.Cache.Keys("cache:SqlSugarDataCache.*");
            // keys = keys.Select(it => it.Replace("cache:", "")).ToArray();
            // return keys;
            return CacheHelper.GetCacheKeys();
        }

        public V GetOrCreate<V>(string cacheKey, Func<V> create, int cacheDurationInSeconds = int.MaxValue)
        {
            if (ContainsKey<V>(cacheKey))
            {
                var result = Get<V>(cacheKey);
                if (result == null)
                {
                    return create();
                }
                else
                {
                    return result;
                }
            }
            else
            {
                var result = create();

                Add(cacheKey, result, cacheDurationInSeconds);
                return result;
            }
        }

        public void Remove<V>(string key)
        {
            // // key = key.Split("cache:")[1];
            // RedisServer.Cache.Del(key);
            CacheHelper.Remove(key);
        }
    }
}