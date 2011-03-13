using System;
using System.IO;
using System.Net;
namespace Kaisir.Net
{
    class HttpHelper
    {
        private static string accept = "image/gif,image/x-xbitmap,image/jpeg,image/pjpeg,application/x-shockwave-flash";
        private static System.Net.CookieContainer cc = new CookieContainer();
        private static string contentType = "application/x-www-form-urlencoded";
        private static System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("utf-8");
        private static string userAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022)";
        /// <summary>
        /// 完整的html提交，支持GET,POST方法以及自定义cookie及Referer
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookiecontainer"></param>
        /// <param name="referer"></param>
        /// <param name="postData"></param>
        /// <param name="isPost"></param>
        /// <returns>html代码</returns>
        public static string GetHtml(string url, CookieContainer cookiecontainer, string referer, string postData, bool isPost)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.CookieContainer = cookiecontainer;
                request.ContentType = contentType;
                //如果referer为空，则使用当前页为Referer,否则传入自定义地址。
                if (!string.IsNullOrEmpty(referer))
                    request.Referer = referer;
                else
                    request.Referer = url;
                request.Accept = accept;
                request.UserAgent = userAgent;
                request.Method = isPost ? "POST" : "GET";
                //如果提交数据不为空 则进行Request
                if (!string.IsNullOrEmpty(postData))
                {
                    byte[] bytes = System.Text.Encoding.Default.GetBytes(postData);
                    request.ContentLength = bytes.Length;
                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                }
                response = (HttpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();

                StreamReader reader = new StreamReader(responseStream, encoding);
                string str = reader.ReadToEnd();
                reader.Close();
                responseStream.Close();
                response.Close();
                return str;
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message.ToString());
                throw Ex;
            }
        }
        /// <summary>
        /// 获取指定地址的文件。Cookie自动管理
        /// </summary>
        /// <param name="url"></param>
        /// <returns>html源文件</returns>
        public static string GetHtml(string url)
        {
            return GetHtml(url, cc, null, null, false);
        }
        /// <summary>
        /// 通过Post/Get方法提交数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="isPost"></param>
        /// <returns>html源文件</returns>
        public static string GetHtml(string url, string postData, bool isPost)
        {
            return GetHtml(url, cc, null, postData, isPost);
        }
        /// <summary>
        /// 自定义指定页面的Referer
        /// </summary>
        /// <param name="url"></param>
        /// <param name="referer"></param>
        /// <returns>html源文件</returns>
        public static string GetHtml(string url, string referer)
        {
            return GetHtml(url, cc, referer, null, false);
        }
        /// <summary>
        /// 产生一个随机的延迟时间
        /// </summary>
        public static void Delay()
        {
            Random ran = new Random();
            int randkey = ran.Next(100, 900);
            System.Threading.Thread.Sleep(randkey);
        }

    }
}
