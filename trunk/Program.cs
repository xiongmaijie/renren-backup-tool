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
            int startPage = 0;//开始页 为续存做准备
            int artNo = 0; //文章编号
            Console.WriteLine("======== 人人网日志备份工具 V0.6版 20110407,Programing By Kaisir =========");
            Console.WriteLine("================== 感谢Alin编写图片保存的模块 ============================");
            Console.WriteLine("============= 感谢使用，如有问题，可联系 Kaisir.Wang@Gmail.Com ===========");
            Console.WriteLine("======================= 我的小站 Http://Kaisir.Com ======================");
            //初始化存放目录
            Kaisir.Common.Common.createFolder(@"D:\Kaisir\");//初始化
            Kaisir.RenRen.RenRenHelper.Login();//登录
            int totalpage=Kaisir.RenRen.RenRenHelper.getTotalPage();//获取总页数
            startPage=Kaisir.RenRen.RenRenHelper.loadPoint(totalpage);//读取断点文件，并根据断点文件置当前页
            string html;
            for (int i=startPage; i <=totalpage ; i++)//修正保存函数，每次保存一页，方便实现断线续存！
            {
                html = Kaisir.RenRen.RenRenHelper.getList(i);
                artNo=Kaisir.RenRen.RenRenHelper.GetArt(html);
                html = "";
                Kaisir.RenRen.RenRenHelper.savePoint(totalpage, i,artNo);//为续存准备
            }
        }
    }
}
