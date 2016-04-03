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

namespace Common.Mulator
{
    /// <summary>
    /// 表示一个安卓模拟器
    /// </summary>
    public class Mulator
    {

        ILog log = LogManager.GetLogger(typeof(Mulator));
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
        private bool IsServerSetuped;


        /// <summary>
        /// 启动安卓服务器
        /// </summary>
        public void StartServer()
        {
            // 1. push jar 文件到安卓模拟器
            // adb push  "C:\Users\bod\eclipse\java-mars\eclipse\RegisterAutomator\bin\RegisterTest.jar" data/local/tmp
            //  Constants.JAR_PATH = @"C:\Users\bod\eclipse\java-mars\eclipse\RegisterAutomator\bin\RegisterTest.jar";
           // Constants.ADB_PATH = "adb";

            string logMsg = ProcessUtility.ExecAndWait(Constants.CMD_PATH, string.Format("{0} -s {3} push {1} {2}", Constants.ADB_PATH, Constants.JAR_PATH, Constants.TEMP_PATH,localIPAddr));

            /*** 此时服务器已经启动 ****/

            // 2. 转发端口
            // adb -s <模拟器> forward tcp:localPort tcp:remotePort
            logMsg = ProcessUtility.ExecAndWait(Constants.CMD_PATH, string.Format("{0} -s {1} forward tcp:{2} tcp:{3}", Constants.ADB_PATH, localIPAddr, localPort, remotePort));


            // 3. 启动 jar 文件
            // adb shell uiautomator runtest RegisterTest.jar -c com.test.TestRegister
            IsServerSetuped = false;
            mulatorProcess = ProcessUtility.Exec(Constants.CMD_PATH, string.Format("{0} shell uiautomator runtest {1} -c com.test.TestRegister", 
                Constants.ADB_PATH,Constants.JAR_FILE),ProcessNormalOutputReceived,ProcessErrorOutputReceived, ProcessExitHanlder);
           
            // 4. 等待服务器启动成功！！
            while(true)
            {
                if (IsServerSetuped)
                {
                    break;
                }

                log.Info("等待安卓模拟器服务端启动....");
                Thread.Sleep(500);
            }

            // 5.开始连接服务器
            cxt = new ConnectContext("127.0.0.1", localPort);

            // 6.分发消息
            dispathCenter = new MsgDispatchCenter(cxt);
            dispathCenter.OnMsgReceived += this.OnMsgReceived;
            dispathCenter.DispatchMsg();
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

            log.Info(e.Data.ToString());
        }

        /// <summary>
        /// 模拟器的错误文本接收处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessErrorOutputReceived(object sender, DataReceivedEventArgs e)
        {
            log.Error(e.Data.ToString());
        }

        /// <summary>
        /// 模拟器退出时触发该方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessExitHanlder(object sender, EventArgs e)
        {
            log.Info("进程退出" + mulatorProcess.ProcessName);
            //IsServerSetuped = false;
        }
    }
}
