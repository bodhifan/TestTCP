using Common.Core;
using Common.Management;
using Common.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Mulator
{
    /// <summary>
    /// 表示一个安卓模拟器
    /// </summary>
    public class Mulator
    {


        public int remotePort { get; set; }       // 模拟器中TCP端口
        public string localIPAddr { get; set; }   // 该模拟器对应的本地IP地址与端口
        public int localPort { get; set; }        // 模拟器的本地端口
        public string name { get; set; }          // 模拟器名称


        public ConnectContext cxt { get; set; }

        private MsgDispatchCenter dispathCenter;


        public event MsgHandler OnMsgReceived;

        /// <summary>
        /// 启动安卓服务器
        /// </summary>
        public void StartServer()
        {
            // 1. push jar 文件到安卓模拟器
            // adb push  "C:\Users\bod\eclipse\java-mars\eclipse\RegisterAutomator\bin\RegisterTest.jar" data/local/tmp
            ProcessUtility.ExecAndWait(Constants.CMD_PATH, string.Format("{0} push {1} {2}", Constants.ADB_PATH, Constants.JAR_PATH, Constants.TEMP_PATH));

            // 2. 启动 jar 文件
            // adb shell uiautomator runtest RegisterTest.jar -c com.test.TestRegister
            ProcessUtility.ExecAndWait(Constants.CMD_PATH, string.Format("{0} shell uiautomator runtest {1} -c com.test.TestRegister",Constants.ADB_PATH,Constants.JAR_FILE));

            /*** 此时服务器已经启动 ****/

            // 3. 转发端口
            // adb -s <模拟器> forward tcp:localPort tcp:remotePort
            ProcessUtility.ExecAndWait(Constants.CMD_PATH, string.Format("{0} -s {1} forward tcp:{2} tcp:{3}", Constants.ADB_PATH, localIPAddr, localPort, remotePort));

            // 4.开始连接服务器
            cxt = new ConnectContext("127.0.0.1", localPort);

            // 5.分发消息
            dispathCenter = new MsgDispatchCenter(cxt);
            dispathCenter.OnMsgReceived += this.OnMsgReceived;
            dispathCenter.DispatchMsg();
        }
    }
}
