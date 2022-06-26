using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script
{
    class getPath
    {
        public static string fileRoute(string file)
        {
            string route = file;
            for (int i = file.Length - 1; route[i] != '\\'; i--)
            {
                route = file.Remove(i);
            }
            return route;
        }

        public static string scriptPath()
        {
            return System.Windows.Forms.Application.StartupPath + @"\PyRPA-main\PyRPA-main\Source";
        }

        //获取该目录所有文件夹
        public static List<string> scriptList(string sourcePath)
        {
            List<string> folder = new List<string>();
            DirectoryInfo theFolder = new DirectoryInfo(sourcePath);
            DirectoryInfo[] dirInfo = theFolder.GetDirectories();
            foreach (DirectoryInfo NextFolder in dirInfo)
            {
                folder.Add(NextFolder.Name);
            }
            return folder;
        }
    }
}
