using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using log4net;
using Common.log;
using Common.Core;
using Common.Management;
using Common.Mulator;
using Common.Utility;

namespace TestServer
{
    
    class Program
    {
        private static byte[] result = new byte[1024];
       // private static int myProt = 8885;   //端口  
        private static int myProt = 42223;   //端口  
        static Socket serverSocket;

        static void Main(string[] args)
        {

            log4net.Config.XmlConfigurator.ConfigureAndWatch(
 new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "Log4Net.config"));

            /**
             * 启动一次完整的模拟器测试
             */
            MulatorManager manager = new MulatorManager();
            Mulator mulator = manager.Setup("MEmu");
            mulator.OnMsgReceived += Mulator_OnMsgReceived;
            mulator.StartServer();
            Console.WriteLine("helo");




            //logMsg = ProcessUtility.ExecAndWait(Constants.CMD_PATH, string.Format("{0} -s {2} shell uiautomator runtest {1} -c com.test.TestRegister", Constants.ADB_PATH, Constants.JAR_FILE, localIPAddr));


            //ConnectContext ctx = new ConnectContext("127.0.0.1", myProt);

            for (int i = 0; i < 10; i++)
            {
                mulator.cxt.writtingQueue.Push("hello" + i);
            }



            /***
             *  编码方式测试
             */
            //// 编译utf-8
            //  string msg = "[length:00016======鎴戞槸瀹㈡湇绔?====]";
            //  msg = "鎴戞帴鏀跺埌瀹㈡埛绔紶鏉ョ殑娑堟伅锛?=====鎴戞槸瀹㈡湇绔?====";
            ////  byte[] utf = Encoding.Convert(Encoding.GetEncoding("GBK"), Encoding.UTF8, Encoding.GetEncoding("GBK").GetBytes(msg));
            //  byte[] gbk = Encoding.GetEncoding(936).GetBytes(msg);
            //  msg = Encoding.UTF8.GetString(gbk);

            //  Console.WriteLine(msg);

            Console.ReadLine();
        }

        private static bool Mulator_OnMsgReceived(ConnectContext ctx, string msg)
        {
            ctx.writtingQueue.Push("======我是客服端=====");
            return true;
        }
    }
}
