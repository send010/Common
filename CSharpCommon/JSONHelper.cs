using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Utility
{
    /// <summary>
    ///JSONHelper 的摘要说明
    /// </summary>
    public static class JSONHelper
    {
        //Newtonsoft.Json.JsonConvert.SerializeObject(postData)


        /// <summary>
        /// 将对象序列化成JSON格式字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJSON(this object obj)
        {
            //JsonSerializerSettings microsoftDateFormatSettings = new JsonSerializerSettings
            //{
            //    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
            //};
            //return Newtonsoft.Json.JsonConvert.SerializeObject(obj,microsoftDateFormatSettings);
            IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
            timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, timeFormat);
        }

        /// <summary>
        /// 将JSON格式字符串反序列化成相应的T类型对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T ParseJSON<T>(this string str)
        {
            JsonSerializerSettings microsoftDateFormatSettings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
            };
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(str, microsoftDateFormatSettings);
        }

        
        /// <summary>
        /// 将JSON格式字符串反序列化成相应的T类型对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Newtonsoft.Json.Linq.JObject ParseJSONJObject(this string str)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(str);
        }

        /// <summary>
        /// 将JSON格式字符串反序列化成相应的T类型对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Newtonsoft.Json.Linq.JArray ParseJSONJArray(this string str)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JArray>(str);
        }
        
        
        /// <summary>
        /// 将JSON格式字符串反序列化成相应的T类型的List泛型集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<T> ParseJSONList<T>(this string str)
        {
            List<T> lst = new List<T>();
            if (str.Substring(0, 1) == "[")
            {
                string s1 = str.Remove(str.Length - 1, 1).Remove(0, 1);
                str = s1.Replace("},{", "};{");
            }
            string[] strs = str.Split(new string[] { "};{" }, StringSplitOptions.RemoveEmptyEntries);
            strs[0] = strs[0].TrimStart('{');
            strs[strs.Length - 1] = strs[strs.Length - 1].TrimEnd('}');
            for (int i = 0; i < strs.Length; i++)
            {
                T t = ("{" + strs[i] + "}").ParseJSON<T>();
                lst.Add(t);
            }
            return lst;
        }

        /// <summary>
        /// 将JSON 反序化成 List<T>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<T> DeserializeList<T>(this string str)
        {
            List<T> lst = new List<T>();
            if (str.Substring(0, 1) == "[")
            {
                string s1 = str.Remove(str.Length - 1, 1).Remove(0, 1);
                str = s1.Replace("},{", "};{");
            }
            string[] strs = str.Split(';');
            for (int i = 0; i < strs.Length; i++)
            {
                T t = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(strs[i]);
                lst.Add(t);
            }
            return lst;
        }

    }
}
