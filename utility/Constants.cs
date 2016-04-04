using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utility
{
    public abstract class Constants
    {
        // 逍遥模拟器安装目录
       public static string DIR_PATH = @"D:\Program Files\Microvirt\MEmu";

        // 逍遥模拟器控制台路径
       public static string MULATOR_CONSOLE_PATH = string.Format("\"{0}\\MEmuConsole.exe\"",DIR_PATH);

        public static string ADB_PATH = string.Format("\"{0}\\adb.exe\"", DIR_PATH);
        //public static string ADB_PATH = string.Format("adb");
        public static string CMD_PATH = "cmd.exe";

        // 服务器端 JAR文件位置
        public static string JAR_FILE = "RegisterTest.jar";
        public static string JAR_PATH = System.AppDomain.CurrentDomain.BaseDirectory + @"\library\" + JAR_FILE;

       // 服务器端存放 JAR文件的文件位置
       public static string TEMP_PATH = "data/local/tmp";

        // 辅助文件名称
        public static string TEMP_FILE = "config.properties";

    }
}
