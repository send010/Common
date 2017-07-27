using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Utility
{
    public static class CacheHelper
    {

        /// <summary>
        /// 设置当前应用程序指定包含相对过期时间Cache值
        /// </summary>
        public static void InsertCacheTimeSpan(string key, object value, long timeSpan = 1 * 60 * 60)
        {
            HttpRuntime.Cache.Insert(key, value, null, DateTime.MaxValue, TimeSpan.FromSeconds(timeSpan));
        }

        /// <summary>
        /// 设定绝对的过期时间
        /// </summary>
        public static void InsertCache(string key, object value, long seconds = 1*60*60)
        {
            HttpRuntime.Cache.Insert(key, value, null, DateTime.Now.AddSeconds(seconds), TimeSpan.Zero);
        }

        public static Object GetCache(string key)
        {
            return HttpRuntime.Cache.Get(key);
        }

        public static void RemoreCache(string key)
        {
            HttpRuntime.Cache.Remove(key);
        }
    }
}
