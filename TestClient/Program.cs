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
        [DllImport("user32.dll")]
        public extern static int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

         
        
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
            //MulatorManager manager = new MulatorManager();
            //mulator = manager.Setup("MEmu", true);
            //mulator.OnMsgReceived += Mulator_OnMsgReceived;
            //mulator.StartServer();

            //Process mulatorProcess = ProcessUtility.nameProcessMap["MEmu"];
            //Console.WriteLine("======title=====  " + mulatorProcess.MainWindowHandle);

            //Mulator.SendKey('A');
            // Console.WriteLine("helo");


            //ProcessUtility.ExecAync(@"C:\WINDOWS\system32\notepad.exe", "","NOTE");

            ////logMsg = ProcessUtility.ExecAndWait(Constants.CMD_PATH, string.Format("{0} -s {2} shell uiautomator runtest {1} -c com.test.TestRegister", Constants.ADB_PATH, Constants.JAR_FILE, localIPAddr));
            Console.ReadLine();
            StringBuilder s = new StringBuilder(512);
            int ii = GetWindowText(Mulator.GetMonitorWindowHandler(), s, s.Capacity);
            Console.WriteLine("title:" + s.ToString());
            Console.ReadLine();
            Mulator.SendKey(Mulator.GetMonitorWindowHandler(),Keys.A);

            //Thread.Sleep(1000 * 5);
            ////ConnectContext ctx = new ConnectContext("127.0.0.1", myProt);
            //Mulator.SendKey(ProcessUtility.nameProcessMap["NOTE"].MainWindowHandle, Keys.A);
            //StringBuilder s = new StringBuilder(512);
            //int i = GetWindowText(ProcessUtility.nameProcessMap["NOTE"].MainWindowHandle, s, s.Capacity); //把this.handle换成你需要的句柄  
            //Console.WriteLine("zheshiwo: " + s.ToString());
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
            LogHelper.Info(msg);
           if (msg == "1click me")
           {
                // 点击A
                //mulator.GetMonitorWindowHandler();
                //mulator.SendKeys('A');
                
                StringBuilder s = new StringBuilder(512);
                int ii = GetWindowText(ProcessUtility.nameProcessMap["MEmu"].MainWindowHandle, s, s.Capacity);
                Console.WriteLine("title:" + s.ToString());

                Mulator.SendKey(ProcessUtility.nameProcessMap["MEmu"].MainWindowHandle, Keys.A);
                for (int i = 0;i < 20;i++)
                {
                    Mulator.SendKey(ProcessUtility.nameProcessMap["MEmu"].MainWindowHandle, Keys.A);
                    Thread.Sleep(1000);
                    Console.WriteLine("send k");
                }
                //SendKeys.Send("{A}");
            }
            return true;
        }
    }
}
