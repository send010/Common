using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Utility
{
    public static class CookieHelper
    {
        public static CookieCollection GetCookie(this string cookieStr)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            CookieCollection restoredCookies = new CookieCollection();
            byte[] cookiesBytes = System.Convert.FromBase64String(cookieStr);
            MemoryStream restoredMemoryStream = new MemoryStream(cookiesBytes);
            restoredCookies = (CookieCollection)binaryFormatter.Deserialize(restoredMemoryStream);
            return restoredCookies;
        }

        public static string GetCookiesStr(this CookieCollection cookie)
        {
            MemoryStream memoryStreamIn = new MemoryStream();
            BinaryFormatter binaryFormatterIn = new BinaryFormatter();
            binaryFormatterIn.Serialize(memoryStreamIn, cookie); //skydriveCookies is CookieCollection
            string CookiesStr = System.Convert.ToBase64String(memoryStreamIn.ToArray());
            return CookiesStr;
        }
    }
}
