using System;
using Kaisir.Common;
using Kaisir.Net;
using Kaisir.RenRen;
namespace RenRen
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("======== 人人网日志备份工具 V0.5版 20110312,Programing By Kaisir =========");
            Console.WriteLine("================== 感谢Alin编写图片保存的模块 ============================");
            Console.WriteLine("============= 感谢使用，如有问题，可联系 Kaisir.Wang@Gmail.Com ===========");
            Console.WriteLine("======================= 我的小站 Http://Kaisir.Com ======================");
            //初始化存放目录
            Kaisir.Common.Common.createFolder(@"D:\Kaisir\");//初始化
            Kaisir.RenRen.RenRenHelper.Login();//登录
            string html=Kaisir.RenRen.RenRenHelper.getList();//获取文章列表
            Kaisir.RenRen.RenRenHelper.GetArt(html);//获取日志
        }
    }
}
