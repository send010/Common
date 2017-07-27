using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Configuration;

namespace Utility
{
    public static class ConfigHelper
    {
        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetAppSettings(string key)
        {
            Object tValue = CacheHelper.GetCache(key);
            if (tValue == null)
            {
                string configValue = ConfigurationSettings.AppSettings[key];
                if (!string.IsNullOrEmpty(configValue))
                {
                    CacheHelper.InsertCacheTimeSpan(key, configValue);
                    return configValue;
                }
                else
                    return "";
            }
            else
                return tValue.ToString();
        }
    }
}
