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
            // 服务器IP地址
            //IPAddress ip = IPAddress.Parse("127.0.0.1");
            //serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //serverSocket.Connect(ip, myProt);
            //string localIPAddr = "127.0.0.1:21503";
            //string logMsg = ProcessUtility.ExecAndWait(Constants.CMD_PATH, string.Format("{0} -s {3} push {1} {2}", "adb", Constants.JAR_PATH, Constants.TEMP_PATH, "127.0.0.1:21503"));
            //logMsg = ProcessUtility.ExecAndWait(Constants.CMD_PATH, string.Format("{0} -s {2} shell uiautomator runtest {1} -c com.test.TestRegister", Constants.ADB_PATH, Constants.JAR_FILE, localIPAddr));

            MulatorManager manager = new MulatorManager();
            Mulator mulator = manager.Setup("MEmu");

            mulator.StartServer();
            Console.WriteLine("helo");
        //    logMsg = ProcessUtility.ExecAndWait(Constants.CMD_PATH, string.Format("{0} -s {2} shell uiautomator runtest {1} -c com.test.TestRegister", Constants.ADB_PATH, Constants.JAR_FILE, localIPAddr));


            //ConnectContext ctx = new ConnectContext("127.0.0.1",myProt);

            //for (int i = 0; i < 10; i++)
            //{
            //    ctx.writtingQueue.Push(new Message("hello" + i));
            //}
            //byte[] result = new byte[1024];
            //string msg = string.Format("length:{0:00000}", 10);
            //string msgstr = "higliagzigleeige";
            //msgstr = string.Format("length:{0:00000}{1}", msgstr.Length, msgstr);
            //Console.WriteLine(msgstr);
            //result = Encoding.UTF8.GetBytes(msg);
            //Console.WriteLine(result.Length.ToString());

            //StringBuilder sb = new StringBuilder();
            //sb.Append("hello,thisisjzuege");
            //string hi = sb.ToString().Substring(0, 12).ToString();
            //Console.WriteLine(hi);

            Console.ReadLine();
        }
    }
}
