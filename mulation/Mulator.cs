using Common.Core;
using Common.Management;
using Common.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Diagnostics;
using System.Threading;
using Win32Api;
using System.Windows.Forms;
using MouseKeyboardLibrary;
using Common.Log;
using System.Drawing;

namespace Common.Mulation
{
    /// <summary>
    /// 表示一个安卓模拟器
    /// </summary>
    public class Mulator
    {

        ILog log = LogManager.GetLogger(typeof(Mulator));

        ILog consoleLog = LogManager.GetLogger("FROM客户端控制台");

        public int remotePort { get; set; }       // 模拟器中TCP端口
        public string localIPAddr { get; set; }   // 该模拟器对应的本地IP地址与端口
        public int localPort { get; set; }        // 模拟器的本地端口
        public string name { get; set; }          // 模拟器名称


        public ConnectContext cxt { get; set; }

        private MsgDispatchCenter dispathCenter;


        public event MsgHandler OnMsgReceived;

        /// <summary>
        /// 标示模拟器进程
        /// </summary>
        private Process mulatorProcess;

        /// <summary>
        /// 服务器是否已经启动
        /// </summary>
        public bool IsServerSetuped { set; get; }

        /// <summary>
        /// 启动安卓服务器
        /// </summary>
        public void StartServer()
        {
            // 1. push jar 文件到安卓模拟器
            // adb push  "C:\Users\bod\eclipse\java-mars\eclipse\RegisterAutomator\bin\RegisterTest.jar" data/local/tmp
           //  Constants.JAR_PATH = @"C:\Users\bod\eclipse\java-mars\eclipse\RegisterAutomator\bin\RegisterTest.jar";
           // Constants.ADB_PATH = "adb";
       //     string logMsg = ProcessUtility.ExecAndWait(Constants.CMD_PATH, string.Format("{0} -s {1} push {2} {3}", Constants.ADB_PATH, localIPAddr, Constants.JAR_PATH, Constants.TEMP_PATH));

            string[] cmmlines= new string[3];
            int index = 0;
            string cmm0 = "CHCP 65001";
           // cmmlines[index++] = cmm0;

            string cmm1 = string.Format("{0} -s {1} push {2} {3}", Constants.ADB_PATH, localIPAddr, Constants.JAR_PATH, Constants.TEMP_PATH);

            cmmlines[index++] = cmm1;
            /*** 此时服务器已经启动 ****/

            // 2. 转发端口
            // adb -s <模拟器> forward tcp:localPort tcp:remotePort
            //logMsg = ProcessUtility.ExecAndWait(Constants.CMD_PATH, string.Format("{0} -s {1} forward tcp:{2} tcp:{3}", Constants.ADB_PATH, localIPAddr, localPort, remotePort));

            string cmm2 = string.Format("{0} -s {1} forward tcp:{2} tcp:{3}", Constants.ADB_PATH, localIPAddr, localPort, remotePort);
            cmmlines[index++] = cmm2;

            // 3. 启动 jar 文件
            // adb shell uiautomator runtest RegisterTest.jar -c com.test.TestRegister
            IsServerSetuped = false;

            string cmm3 = string.Format("{0} -s {1} shell uiautomator runtest {2} -c com.test.TestRegister", Constants.ADB_PATH, localIPAddr, Constants.JAR_FILE);
            cmmlines[index++] = cmm3;

            mulatorProcess = ProcessUtility.Exec(Constants.CMD_PATH, cmmlines ,ProcessNormalOutputReceived,ProcessErrorOutputReceived, ProcessExitHanlder);
           
            // 4. 等待服务器启动成功！！
            while(true)
            {
                if (IsServerSetuped)                {
                    break;
                }

                log.Info("等待安卓模拟器服务端启动....");
                Thread.Sleep(1000);
            }

            // 5.开始连接服务器
            cxt = new ConnectContext("127.0.0.1", localPort);

            // 6.分发消息
            dispathCenter = new MsgDispatchCenter(cxt);
            dispathCenter.OnMsgReceived += this.OnMsgReceived;
            dispathCenter.DispatchMsg();
        }

        /// <summary>
        /// 获取模拟器窗口句柄
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetMonitorWindowHandler()
        {
           IntPtr handler = Win32API.FindWindow(null, "逍遥安卓 2.5.0 - MEmu");
           int times = 4;
            while(handler == null && times-- > 0)
            {
                LogHelper.Warn("获取模拟器句柄失败,重新获取...");
                handler = Win32API.FindWindow(null, "逍遥安卓 2.5.0 - MEmu");
            }

            return handler;
        }
        public static void SendCmm(string key,int delay = 2000)
        {
            bool flag = ImageValidation.IsHotKeyStarting();
            if (!flag)
            {
                LogHelper.Debug("开启热键...");
                SetupHotKey();
            }
            SendDDKey(key);

            Thread.Sleep(delay);
        }

        public static void ActiveHotkey()
        {
            ActiveMulator();
            RECT rect = ImageValidation.GetRaidoRect();
            int offsetX = ConfigFileManager.Instance().GetConfigFile().ReadInteger("HOTKEY验证码", "OFFSET_L", 640);
            int offsetY = ConfigFileManager.Instance().GetConfigFile().ReadInteger("HOTKEY验证码", "OFFSET_T", 85);

            offsetX += 8;
            offsetY += 8;
            KeyboardUtility.Instance().MouseMove(rect.Left + offsetX, rect.Top + offsetY);
            Thread.Sleep(500);
            KeyboardUtility.Instance().SendButton(Utility.MouseButton.Left);
            Thread.Sleep(1000);
            KeyboardUtility.Instance().SendButton(Utility.MouseButton.Left);

            KeyboardUtility.Instance().MouseMove(rect.Left+200, rect.Top+700);
            Thread.Sleep(500);
            KeyboardUtility.Instance().SendButton(Utility.MouseButton.Left);
            
        }

        /// <summary>
        /// 激活模拟器窗口使其至于最前端
        /// </summary>
        /// <returns></returns>
        public static IntPtr ActiveMulator()
        {
            IntPtr handler = Mulator.GetMonitorWindowHandler();
            Win32API.ShowWindow(handler, Win32API.SW_RESTORE);
            Win32API.SetActiveWindow(handler);
            Win32API.SetForegroundWindow(handler);
            return handler;
        }

        public static void ClearBrowser()
        {
            LogHelper.Debug("==========================清理浏览器 开始========================");
            AdbUtility.ClearBrowser();
            LogHelper.Debug("==========================清理浏览器 结束========================");
        }
        public static void SetupHotKey()
        {
            ActiveMulator();
            // 按下ALT键
            KeyboardUtility.Instance().SendKey(602, 1);
            // 按下A键
            KeyboardUtility.Instance().SendKey(401, 1);
            // 松开A键
            KeyboardUtility.Instance().SendKey(401, 2);
            // 松开ALT键
            KeyboardUtility.Instance().SendKey(602, 2);
        }

        public static void SendDDKey(string key, bool focused = false, int maxSpeedMills = 200)
        {
            IntPtr handler = ActiveMulator();
            KeyboardUtility.Instance().SendKey(key, maxSpeedMills);
            LogHelper.Info("输入：" + key);
            if (focused)
            {
                LostFocus();
            }
            Thread.Sleep(500);
        }

        public static void SendKey(string key,bool focused=false,int maxSpeedMills = 200)
        {
            bool flag = ImageValidation.IsHotKeyStarting();
            if (flag)
            {
                LogHelper.Debug("关闭热键...");
                SetupHotKey();
            }
            SendDDKey(key, focused, maxSpeedMills);
        }

        public static void LostFocus()
        {
            LogHelper.Debug("lost focus");
            RECT rect = ImageValidation.GetRaidoRect();
            LogHelper.Info(string.Format("LEFT:{0} TOP:{1} RIGHT:{2} BUTTOM:{3}", rect.Left, rect.Top, rect.Right, rect.Bottom));
            int x = rect.Left + 150;
            int y = rect.Top + 600;
            KeyboardUtility.Instance().MouseMove(x, y);
            Thread.Sleep(500);
            KeyboardUtility.Instance().SendButton(Common.Utility.MouseButton.Left);
        }







        /// <summary>
        /// 关闭模拟器实例
        /// </summary>
        public void KillServer()
        {
            // 正常的流应该是发送 CTRL + C 命令
            // 这里直接KILL process
            mulatorProcess.Close();
        }

        /// <summary>
        /// 模拟器的正常文本接收处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessNormalOutputReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
            {
                return;
            }
            string msg = e.Data.ToString();
            if (msg.Contains("begin listen"))
            {
                IsServerSetuped = true;
            }

            consoleLog.Info(msg);

            //byte[] gbk = Encoding.GetEncoding(437).GetBytes(msg);
            //msg = Encoding.UTF8.GetString(gbk);
            //consoleLog.Info(msg);
        }

        /// <summary>
        /// 模拟器的错误文本接收处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessErrorOutputReceived(object sender, DataReceivedEventArgs e)
        {

            consoleLog.Error(e.Data.ToString());
        }

        /// <summary>
        /// 模拟器退出时触发该方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessExitHanlder(object sender, EventArgs e)
        {
            log.Info("模拟器进程退出");
            IsServerSetuped = false;
        }
    }
}
