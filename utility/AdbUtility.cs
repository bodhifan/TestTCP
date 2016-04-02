using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utility
{
    /**
    * adb 相关操作
    */
    public class AdbUtility
    {
        static string DIR_PATH = @"C:\Program Files\Microvirt\MEmu";
        static string ADB_PATH = string.Format("\"{0}\\adb.exe\"", DIR_PATH);
        static string CMD_PATH = "cmd.exe";
        /// <summary>
        /// 获取所有的安卓模拟器
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllDevices()
        {
            List<string> allActiveIps = new List<string>();
            string mulatorExecuName = "tasklist | findstr \"MEmuHeadless.exe\"";
            string allMEmuHeadless =  ProcessUtility.ExecAndWait(CMD_PATH, mulatorExecuName);

            // 逍遥模拟器IP地址模拟
            string[] defaultIPs = new string[] { "127.0.0.1:21503","127.0.0.1:21513","127.0.0.1:21523","127.0.0.1:21533"};
            Connect2Devices(defaultIPs);

            string allDevicesStr = string.Format("{0} devices",ADB_PATH);
            allDevicesStr = ProcessUtility.ExecAndWait(CMD_PATH, allDevicesStr);
            string[] lines = allDevicesStr.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            foreach (string line in lines)
            {
                // 如果该模拟器处于激活，既联通状态，则收集该IP地址
                if (line.Contains("device"))
                {
                    string[] cols = line.Split(new string[] { "\t", "      ", "     ", "    ", "   ", "  ", " " }, StringSplitOptions.None);
                    if (cols.Length != 2)
                    {
                        continue;
                    }
                    allActiveIps.Add(cols[0]);
                }
            }
            return allActiveIps;
        }

        /// <summary>
        /// 连接到模拟器
        /// </summary>
        /// <param name="defaultIPs"></param>
        private static void Connect2Devices(string[] defaultIPs)
        {
            string cmmStr = "{0} connect {1}";
            foreach (string ip in defaultIPs)
            {
                string cmm = string.Format(cmmStr, ADB_PATH, ip);
                ProcessUtility.ExecAndWait(CMD_PATH, cmm);
            }
        }

    }
}
