using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using log4net;
using Common.Log;
using Common.Core;
using Common.Management;
using Common.Mulator;
using Common.Utility;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TestServer
{
    
    class Program
    {
        private static byte[] result = new byte[1024];
       // private static int myProt = 8885;   //端口  
        private static int myProt = 42223;   //端口  
        static Socket serverSocket;
        static Mulator mulator;
        static void Main(string[] args)
        {

            log4net.Config.XmlConfigurator.ConfigureAndWatch(
 new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "Log4Net.config"));

            /**
             * 启动一次完整的模拟器测试
             */
            MulatorManager manager = new MulatorManager();
            mulator = manager.Setup("MEmu", true);
            mulator.OnMsgReceived += Mulator_OnMsgReceived;
            mulator.StartServer();

            Console.ReadLine();
        }

        private static bool Mulator_OnMsgReceived(ConnectContext ctx, string msg)
        {
            // 在这里解析消息
            LogHelper.Info("获取到消息体：" + msg);
            return true;
        }
    }
}
