using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kaisir.Common
{
    class Common
    {
        private static bool isExists(string path)
        {
            if (!Directory.Exists(path))
                return false;
            else
                return true;
        }

        public static void createFolder(string path)
        {
            if (!isExists(path))
            {
                //CreateFolder
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception)
                {
                    Console.WriteLine(path+"创建错误！请检查D盘是否有写权限！");
                    throw;
                }
            }
        }
    }
}
