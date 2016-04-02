using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Utility;
namespace Common.Utility
{
    /**
    * adb 相关操作
    */
    public class AdbUtility
    {
        /// <summary>
        /// 获取所有的安卓模拟器
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllDevices()
        {
            List<string> allActiveIps = new List<string>();
            string mulatorExecuName = "tasklist | findstr \"MEmuHeadless.exe\"";
            string allMEmuHeadless =  ProcessUtility.ExecAndWait(Constants.CMD_PATH, mulatorExecuName);

            // 逍遥模拟器IP地址模拟
            string[] defaultIPs = new string[] { "127.0.0.1:21503","127.0.0.1:21513","127.0.0.1:21523","127.0.0.1:21533"};
            Connect2Devices(defaultIPs);

            /**
            * 解析命令返回文本，获取激活的模拟器IP地址
            */
            string allDevicesStr = string.Format("{0} devices", Constants.ADB_PATH);
            allDevicesStr = ProcessUtility.ExecAndWait(Constants.CMD_PATH, allDevicesStr);
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
        /// 端口数据转发：
        /// PC上所有localPort端口通信数据将被重定向到手机端remotePort端口server上
        /// </summary>
        /// <param name="localPort"></param>
        /// <param name="remotePort"></param>
        public static void ForwardConnect(int localPort, int remotePort)
        {
            ProcessUtility.ExecAndWait(Constants.CMD_PATH, string.Format("{0} forward tcp:{1} tcp:{2}", Constants.ADB_PATH, localPort, remotePort));
        }


        /// <summary>        /// 连接到模拟器
        /// </summary>
        /// <param name="defaultIPs"></param>
        private static void Connect2Devices(string[] defaultIPs)
        {
            string cmmStr = "{0} connect {1}";
            foreach (string ip in defaultIPs)
            {
                string cmm = string.Format(cmmStr, Constants.ADB_PATH, ip);
                ProcessUtility.ExecAndWait(Constants.CMD_PATH, cmm);
            }
        }

    }
}
