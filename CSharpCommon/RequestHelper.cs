using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Web;

namespace Utility
{
    public static class RequestHelper
    {
        /// <summary>
        /// 将From表单值转JObjcet
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        public static JObject ToJObject(this HttpRequest Request)
        {
            JObject obj = new JObject();
            foreach (var item in Request.Form.AllKeys)
            {
                obj.Add(item, Request.Form[item]);
            }
            return obj;
        }

        /// <summary>
        /// 将From表单值转Json
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        public static string ToJsonString(this HttpRequest Request)
        {
            return Request.ToJObject().ToString();
        }
    }
}
