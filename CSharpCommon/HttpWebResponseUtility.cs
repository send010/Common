using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace Utility
{
    public class HttpWebResponseUtility
    {

        public HttpWebResponseUtility()
        {
        }
        #region HTTP POST

        public string Post(string url, string data, System.Text.Encoding encoding, Dictionary<string, string> headParams = null)
        {
            try
            {
                HttpWebRequest request = null;
                //如果是发送HTTPS请求  
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    request = WebRequest.Create(url) as HttpWebRequest;
                    request.ProtocolVersion = HttpVersion.Version10;
                }
                else
                {
                    request = WebRequest.Create(url) as HttpWebRequest;
                }

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = "User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.153 Safari/537.36";
                if (headParams != null)
                    foreach (var item in headParams)
                        request.Headers.Add(item.Key, item.Value);

                byte[] bdata = encoding.GetBytes(data);
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(bdata, 0, bdata.Length);
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), encoding);

                string callBackHtml = reader.ReadToEnd();

                reader.Close();
                reader.Dispose();
                return callBackHtml;
            }
            catch (Exception e)
            {
                return "<?xml version=\"1.0\" encoding=\"utf-8\"?><error><message>异常信息：" + e.Message + "</message></error>";
            }
        }

        public string Get(string url, string data, System.Text.Encoding encoding, Dictionary<string, string> headParams = null)
        {
            try
            {
                HttpWebRequest req = null;
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    req = WebRequest.Create(url) as HttpWebRequest;
                    req.ProtocolVersion = HttpVersion.Version10;
                }
                else
                {
                    req = WebRequest.Create(url) as HttpWebRequest;
                }
                req.AllowAutoRedirect = false;
                req.ServicePoint.Expect100Continue = false;
                req.Method = "GET";
                req.ContentType = "application/x-www-form-urlencoded";
                req.UserAgent = "User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.153 Safari/537.36";
                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";

                if (!string.IsNullOrEmpty(data))
                {
                    byte[] tmpData = encoding.GetBytes(data);
                    using (Stream stream = req.GetRequestStream())
                    {
                        stream.Write(tmpData, 0, tmpData.Length);
                    }
                }
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();// as HttpWebResponse;
                //Stream stmResp = resp.GetResponseStream();
                StreamReader stmReader = new StreamReader(resp.GetResponseStream(), encoding);
                return stmReader.ReadToEnd();
            }
            catch
            {
                return null;
            }
        }


        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; // 总是接受    
        }
        #endregion

        #region HTTP SendRequest

        public HttpWebResponse SendRequest(bool allowAutoRedirect, string url, string refer, string sendData, List<string> headers, CookieCollection cookies, string method, out string resultData, out CookieCollection resultCookie, Encoding encoding, string userAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.153 Safari/537.36")
        {
            return SendRequest(allowAutoRedirect, url, refer, sendData, headers, cookies, method, out resultData, out resultCookie, encoding, 0, userAgent);
        }

        public HttpWebResponse SendRequest(bool allowAutoRedirect, string url, string refer, string sendData, List<string> headers, CookieCollection cookies, string method, out string resultData, out CookieCollection resultCookie, Encoding encoding, int timeout, string userAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.153 Safari/537.36")
        {
            resultData = "";
            resultCookie = new CookieCollection();
            try
            {
                HttpWebRequest req = null;
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    req = WebRequest.Create(url) as HttpWebRequest;
                    req.ProtocolVersion = HttpVersion.Version10;
                }
                else
                {
                    req = WebRequest.Create(url) as HttpWebRequest;
                }
                if (timeout != 0)
                    req.Timeout = timeout;
                req.AllowAutoRedirect = allowAutoRedirect;
                req.ServicePoint.Expect100Continue = false;
                req.Method = method;
                req.ContentType = "application/x-www-form-urlencoded";
                req.UserAgent = userAgent;
                req.Referer = refer;
                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";


                if (headers != null && headers.Count > 0)
                {
                    foreach (string item in headers)
                    {
                        req.Headers.Add(item);
                    }
                }
                req.CookieContainer = new CookieContainer();
                if (cookies != null && cookies.Count > 0)
                {
                    req.CookieContainer.Add(cookies);
                }
                if (!string.IsNullOrEmpty(sendData))
                {
                    byte[] tmpData = encoding.GetBytes(sendData);
                    using (Stream stream = req.GetRequestStream())
                    {
                        stream.Write(tmpData, 0, tmpData.Length);
                    }
                }
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();// as HttpWebResponse;
                //Stream stmResp = resp.GetResponseStream();
                StreamReader stmReader = new StreamReader(resp.GetResponseStream(), encoding);
                resultData = stmReader.ReadToEnd();
                resultCookie = resp.Cookies.Count > 0 ? resp.Cookies : cookies;
                //stmResp.Close();
                return resp;
            }
            catch
            {
                return null;
            }
        }

        public HttpWebResponse SendRequest(string url, string refer, string sendData, List<string> headers, CookieCollection cookies, string method, out string resultData, out CookieCollection resultCookie, Encoding encoding, bool AllowAutoRedirect, string userAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.153 Safari/537.36")
        {
            return SendRequest(url, refer, sendData, headers, cookies, method, out resultData, out resultCookie, encoding, 0, AllowAutoRedirect, userAgent);
        }

        public HttpWebResponse SendRequest(string url, string refer, string sendData, List<string> headers, CookieCollection cookies, string method, out string resultData, out CookieCollection resultCookie, Encoding encoding, string userAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.153 Safari/537.36")
        {
            return SendRequest(url, refer, sendData, headers, cookies, method, out resultData, out resultCookie, encoding, 0, userAgent);
        }


        public HttpWebResponse SendRequest(string url, string refer, string sendData, string contentType, List<string> headers, CookieCollection cookies, string method, out string resultData, out CookieCollection resultCookie, Encoding encoding, string userAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.153 Safari/537.36")
        {
            resultData = "";
            resultCookie = new CookieCollection();
            try
            {
                HttpWebRequest req = null;
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    req = WebRequest.Create(url) as HttpWebRequest;
                    req.ProtocolVersion = HttpVersion.Version10;
                }
                else
                {
                    req = WebRequest.Create(url) as HttpWebRequest;
                }
                req.AllowAutoRedirect = false;
                req.ServicePoint.Expect100Continue = false;
                req.Method = method;
                req.ContentType = contentType;
                req.UserAgent = userAgent;
                req.Referer = refer;
                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                req.Timeout = 1000 * 60;

                if (headers != null && headers.Count > 0)
                {
                    foreach (string item in headers)
                    {
                        req.Headers.Add(item);
                    }
                }
                req.CookieContainer = new CookieContainer();
                if (cookies != null && cookies.Count > 0)
                {
                    req.CookieContainer.Add(cookies);
                }
                if (!string.IsNullOrEmpty(sendData))
                {
                    byte[] tmpData = encoding.GetBytes(sendData);
                    using (Stream stream = req.GetRequestStream())
                    {
                        stream.Write(tmpData, 0, tmpData.Length);
                    }
                }
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();// as HttpWebResponse;
                //Stream stmResp = resp.GetResponseStream();
                StreamReader stmReader = new StreamReader(resp.GetResponseStream(), encoding);
                resultData = stmReader.ReadToEnd();
                resultCookie = resp.Cookies.Count > 0 ? resp.Cookies : cookies;
                //stmResp.Close();
                return resp;
            }
            catch
            {
                return null;
            }
        }

        public HttpWebResponse SendRequest(string url, string refer, string sendData, List<string> headers, CookieCollection cookies, string method, out string resultData, out CookieCollection resultCookie, Encoding encoding, int timeout, bool AllowAutoRedirect, string userAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.153 Safari/537.36")
        {
            resultData = "";
            resultCookie = new CookieCollection();
            try
            {
                HttpWebRequest req = null;
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    req = WebRequest.Create(url) as HttpWebRequest;
                    req.ProtocolVersion = HttpVersion.Version10;
                }
                else
                {
                    req = WebRequest.Create(url) as HttpWebRequest;
                }
                if (timeout != 0)
                    req.Timeout = timeout;
                req.AllowAutoRedirect = AllowAutoRedirect;
                req.ServicePoint.Expect100Continue = false;
                req.Method = method;
                req.ContentType = "application/x-www-form-urlencoded";
                req.UserAgent = userAgent;
                req.Referer = refer;
                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";


                if (headers != null && headers.Count > 0)
                {
                    foreach (string item in headers)
                    {
                        req.Headers.Add(item);
                    }
                }
                req.CookieContainer = new CookieContainer();
                if (cookies != null && cookies.Count > 0)
                {
                    req.CookieContainer.Add(cookies);
                }
                if (!string.IsNullOrEmpty(sendData))
                {
                    byte[] tmpData = encoding.GetBytes(sendData);
                    using (Stream stream = req.GetRequestStream())
                    {
                        stream.Write(tmpData, 0, tmpData.Length);
                    }
                }
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();// as HttpWebResponse;
                //Stream stmResp = resp.GetResponseStream();
                StreamReader stmReader = new StreamReader(resp.GetResponseStream(), encoding);
                resultData = stmReader.ReadToEnd();
                resultCookie = resp.Cookies.Count > 0 ? resp.Cookies : cookies;
                //stmResp.Close();
                return resp;
            }
            catch
            {
                return null;
            }
        }

        public HttpWebResponse SendRequest(string url, string refer, string sendData, List<string> headers, CookieCollection cookies, string method, out string resultData, out CookieCollection resultCookie, Encoding encoding, int timeout, string userAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.153 Safari/537.36")
        {
            resultData = "";
            resultCookie = new CookieCollection();
            try
            {
                HttpWebRequest req = null;
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    req = WebRequest.Create(url) as HttpWebRequest;
                    req.ProtocolVersion = HttpVersion.Version10;
                }
                else
                {
                    req = WebRequest.Create(url) as HttpWebRequest;
                }
                if (timeout != 0)
                    req.Timeout = timeout;
                req.AllowAutoRedirect = false;
                req.ServicePoint.Expect100Continue = false;
                req.Method = method;
                req.ContentType = "application/x-www-form-urlencoded";
                req.UserAgent = userAgent;
                req.Referer = refer;
                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";


                if (headers != null && headers.Count > 0)
                {
                    foreach (string item in headers)
                    {
                        req.Headers.Add(item);
                    }
                }
                req.CookieContainer = new CookieContainer();
                if (cookies != null && cookies.Count > 0)
                {
                    req.CookieContainer.Add(cookies);
                }
                if (!string.IsNullOrEmpty(sendData))
                {
                    byte[] tmpData = encoding.GetBytes(sendData);
                    using (Stream stream = req.GetRequestStream())
                    {
                        stream.Write(tmpData, 0, tmpData.Length);
                    }
                }
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();// as HttpWebResponse;
                //Stream stmResp = resp.GetResponseStream();
                StreamReader stmReader = new StreamReader(resp.GetResponseStream(), encoding);
                resultData = stmReader.ReadToEnd();
                resultCookie = resp.Cookies.Count > 0 ? resp.Cookies : cookies;
                //stmResp.Close();
                return resp;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region HTTP IMAGE LOAD
        
        public System.Drawing.Image Load(string url, string refer, List<string> headers, CookieCollection cookies, out CookieCollection res_cookie, string userAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.153 Safari/537.36")
        {
            try
            {

                HttpWebRequest req = null;
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    req = WebRequest.Create(url) as HttpWebRequest;
                    req.ProtocolVersion = HttpVersion.Version10;
                }
                else
                {
                    req = WebRequest.Create(url) as HttpWebRequest;
                }
                req.AllowAutoRedirect = false;
                req.ServicePoint.Expect100Continue = false;
                req.Method = "GET";
                req.ContentType = "application/x-www-form-urlencoded";
                req.UserAgent = userAgent;

                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";


                req.CookieContainer = new CookieContainer();
                if (cookies != null && cookies.Count > 0)
                {
                    req.CookieContainer.Add(cookies);
                }
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();// as HttpWebResponse;
                if (resp.Cookies != null && resp.Cookies.Count > 0)
                    res_cookie = resp.Cookies;
                else
                    res_cookie = cookies;
                System.Drawing.Image img = System.Drawing.Image.FromStream(resp.GetResponseStream());
                return img;
            }
            catch (Exception e)
            {
                res_cookie = null;
                return null;
            }
        }


        public System.Drawing.Image Load(string url, string refer, List<string> headers, CookieCollection cookies, string userAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.153 Safari/537.36")
        {
            return Load(url, refer, headers, cookies, out cookies, userAgent);
        }

        public System.Drawing.Image Load(string url, CookieCollection cookie)
        {
            return Load(url, null, null, cookie);
        }

        public System.Drawing.Image Load(string url, string cookie)
        {
            try
            {
                WebClient web = new WebClient();
                web.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                web.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/33.0.1750.154 Safari/537.36");
                web.Headers.Add("Cookie", cookie);
                byte[] arr = web.DownloadData(url);
                System.IO.MemoryStream streamBitmap = new System.IO.MemoryStream(arr);
                System.Drawing.Image img = System.Drawing.Image.FromStream(streamBitmap);
                return img;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        #endregion


    }
}
