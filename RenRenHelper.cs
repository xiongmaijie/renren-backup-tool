using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Kaisir.Net;

namespace Kaisir.RenRen
{
    class RenRenHelper
    {
        private static int num = 0;
        private static int filename = 0;
        private static string html = "";
        private static string url = "http://www.renren.com/PLogin.do";

        /// <summary>
        /// 人人网登录
        /// </summary>
        /// <returns>登录成功：True;登录失败：False</returns>
        public static bool Login()
        {
            string username = "";
            string password = "";
            Console.Write("请输入用户名:");
            username = Console.ReadLine();
            Console.Write("请输入密码:");
            password = Console.ReadLine();
            string postData = string.Format("email={0}&password={1}&origURL=http%3A%2F%2Fwww.renren.com%2FHome.do&domain=renren.com", username, password);
            Console.WriteLine("==================开始登陆==================");
            html=HttpHelper.GetHtml(url, postData, true);
            //判断登录是否成功 2011.01.29
            Regex r = new Regex("登录帐号或密码错误，请重试");
            Match m = r.Match(html);
            if (m.Success)
            {
                Console.WriteLine("用户名密码错！登录失败");
                return false;
            }
            else
            {
                Console.WriteLine("登录成功！");
                return true;
            }
        }
        /// <summary>
        /// 获取日志总页数
        /// </summary>
        /// <returns>int 总页数</returns>
        public static int getTotalPage()
        {
            url = "http://blog.renren.com/blog/0?from=homeleft&__view=async-html";
            string referer = "http://blog.renren.com/ajaxproxy.htm";
            string html = HttpHelper.GetHtml(url, referer);
            Console.WriteLine("==================正在获取总页数==================");
            Regex regex = new Regex("\"http://blog.renren.com/blog/0\\?curpage=(\\d*)&amp;year=0&amp;month=0&amp;selitem=\">最后页");
            foreach (Match match in regex.Matches(html))
            {
                num = int.Parse(match.Groups[1].Value);
            }
            Console.WriteLine("总页数:{0}", num + 1);//页数从1开始
            return num;
        }
        /// <summary>
        /// 获取日志列表
        /// </summary>
        /// <returns>String：列表html</returns>
        public static string getList(int page)
        {
            url = "http://blog.renren.com/blog/0?from=homeleft&__view=async-html";
            string referer = "http://blog.renren.com/ajaxproxy.htm";
            string html = HttpHelper.GetHtml(url, referer);
            Console.WriteLine("==================正在获取列表页-{0}==================",page+1);
            //Console.WriteLine("正在获取列表");
            //每次获取一页 不再需要循环
//            for (int i = 1; i <= page; i++) 
//           {
                //http://blog.renren.com/blog/0?curpage={0}&amp;year=0&amp;month=0&amp;selitem="
                url = string.Format("http://blog.renren.com/blog/0?curpage={0}&year=0&month=0&selitem=&__view=async-html", page);
                //Console.WriteLine("正在获取列表-第{0}页", page+1 );//页数从1开始
                //html = html + HttpHelper.GetHtml(url, referer);//错误！
                html = HttpHelper.GetHtml(url, referer);
//            }
            return html;
        }
        /// <summary>
        /// 获取日志
        /// </summary>
        /// <param name="html">要解析的html</param>
        public static int GetArt(string html)
        {
            Regex regex = new Regex("\"(http://.*/\\d{3,11}/\\d{3,11})\">阅读");
            foreach (Match match2 in regex.Matches(html))
            {
                Console.WriteLine("{0}", match2.Groups[1].Value);//显示文章路径
                Console.WriteLine("开始获取第{0}篇日志内容...", filename++);
                SaveArt(match2.Groups[1].Value, filename);
            }
            return filename;
        }
        /// <summary>
        /// 保存日志
        /// </summary>
        /// <param name="urls">要保存的日志URL</param>
        /// <param name="filename">保存的文件名</param>
        private static void SaveArt(string urls, int filename)
        {
            string input = "";
            HttpHelper.Delay();//2011-3-12 增加延时函数
            input = HttpHelper.GetHtml(urls);
            //判断是否是手机日志
            Regex regex = new Regex("这是一篇通过手机发布的日志");
            Match m = regex.Match(input);
            if (m.Success)
            {
                regex = new Regex("<strong>(.*)</strong>\\s<span class=\"timestamp\">(.*)<span class=\"group\">\\(分类:<a\\shref=.*'>(.*)</a>\\)</span>\\s*<div class=.*>[\\s\\S]*?<div id=\"blogContent\" class=\"text-article\">([\\s\\S]*?)</div>", RegexOptions.None);
            }
            else
            {
                //判断是否是导入的日志
                regex = new Regex("这是一篇被导入的日志");
                m = regex.Match(input);
                if (m.Success)
                {
                    regex = new Regex("<strong>(.*)</strong>\\s<span class=\"timestamp\">(.*)<span class=\"group\">\\(分类:<a\\shref=.*'>(.*)</a>\\)</span>\\s*<div class=.*>[\\s\\S]*?<div id=\"blogContent\" class=\"text-article\">([\\s\\S]*?)</div>", RegexOptions.None);
                }
                else //普通日志  方法不好 普通日志需要两次判断 效率低！
                {
                    regex = new Regex("<strong>(.*)</strong>\\s<span class=\"timestamp\">(.*)<span class=\"group\">\\(分类:<a\\shref=.*>(.*)</a>\\)</span>\\s</h3>\\s<div id=\"blogContent\" class=\"text-article\">([\\s\\S]*?)</div>", RegexOptions.None);
                }
            }
            //将文章内容临时保存到临时变量content
            Match match = regex.Match(input);
            input = match.Groups[4].Value;
            //Console.WriteLine(input);
            //foreach (Match match in regex.Matches(input))
            //{
            Regex picture = new Regex("<img src=\"http://[^\\s]*\\.jpg");//人人网日志中的图片后缀都为jpg
            foreach (Match matchimg in picture.Matches(input))
            {
                //寻找文章图片
                //Console.WriteLine(matchimg.Value);
                string tmp = matchimg.Value.Replace("<img src=\"", "");
                //Console.WriteLine(tmp);
                int t = tmp.LastIndexOf("/");
                string img = tmp.Substring(t + 1);

                //Console.WriteLine(img);
                download(tmp, img, filename);
                //执行地址替换
                input = Regex.Replace(input, tmp, "img\\" + filename + "\\" + img);

            }

            StreamWriter writer = new StreamWriter(@"d:\Kaisir\" + filename + ".html");
            //增加页面编码
            writer.WriteLine("<html><head><meta http-equiv=content-type content=text/html; charset=UTF-8></head>");
            writer.WriteLine("标题:{0},时间:{1},分类:{2},内容:{3}", new object[] { match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, input });
            Console.WriteLine(match.Groups[1].Value);
            writer.WriteLine("</html>");
            writer.Close();
            writer.Dispose();


            //}

        }

        /// <summary>
        /// 下载日志中的图片
        /// </summary>
        /// <param name="url">图片的URL地址</param>
        /// <param name="filename">保存的文件名</param>
        /// <param name="Addrurl">保存文件子目录</param>
        private static void download(string url, string filename, int Addrurl)
        {
            WebClient client = new WebClient();
            try
            {
                client.DownloadFile(url, filename);
            }
            catch (Exception ex)
            {
                Console.WriteLine("===================异常=======================");
                Console.WriteLine(ex.Message.ToString());
                Console.WriteLine("===================异常=======================");
            }
           
            HttpHelper.Delay();
            Stream str = client.OpenRead(url);
            byte[] mbyte = new byte[1000000];
            int allmybyte = (int)mbyte.Length;
            int startmbyte = 0;
            //写入到BYTE数组中，起缓冲作用
            while (allmybyte > 0)
            {
                int m = str.Read(mbyte, startmbyte, allmybyte);
                if (m == 0)
                    break;

                startmbyte += m;
                allmybyte -= m;
            }
            Kaisir.Common.Common.createFolder(@"D:\Kaisir\img\" + Addrurl + "\\");
            FileStream fstr = new FileStream(@"D:\Kaisir\img\" + Addrurl + "\\" + filename, FileMode.Create, FileAccess.Write);
            fstr.Write(mbyte, 0, startmbyte);
            str.Close();
            fstr.Close();
        }
        public static void savePoint(int totalpage,int currentpage,int artno)
        {
            StreamWriter sw = new StreamWriter(@"d:\Kaisir\savePoint.dat");
            sw.WriteLine(totalpage);
            sw.WriteLine(currentpage);
            sw.WriteLine(artno);
            sw.Close();
            sw.Dispose();
        }
        public static int loadPoint(int newTotal)
        {
            int totalpage, currentpage, artno;
            try
            {
                StreamReader sr = new StreamReader(@"d:\Kaisir\savePoint.dat");
                totalpage = int.Parse(sr.ReadLine());
                currentpage = int.Parse(sr.ReadLine());
                artno = int.Parse(sr.ReadLine());
                //增量备份
                //续传
                if (totalpage != currentpage)
                {
                    Console.WriteLine("系统检测到存在断点文件，开始续存！（{0}/{1}）", currentpage+1, totalpage+1);
                    //Console.ReadKey();
                    //确认续传，初始化变量
                    filename = artno + 1;//置保存文件名
                    return currentpage+1;//置当前页
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("============================================");
                Console.WriteLine(ex.Message.ToString());
                Console.WriteLine("============================================");
            }

            return 0;
        }
    }
}
