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
using Common.Utility;

namespace TestServer
{
    class Program
    {
        private static byte[] result = new byte[1024];
        private static int myProt = 8885;   //端口  
        static Socket serverSocket;

        static void Main(string[] args)
        {

            log4net.Config.XmlConfigurator.ConfigureAndWatch(
 new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "Log4Net.config"));
            // 服务器IP地址
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(ip, myProt));  //绑定IP地址：端口  
            serverSocket.Listen(10);    //设定最多10个排队连接请求  
            Console.WriteLine("启动监听{0}成功", serverSocket.LocalEndPoint.ToString());
            //通过Clientsoket发送数据  
            Thread myThread = new Thread(ListenClientConnect);
            myThread.Start();

            //  AdbUtility.GetAllDevices();
            Console.ReadLine();
        }
        /// <summary>  
        /// 监听客户端连接  
        /// </summary>  
        private static void ListenClientConnect()
        {
            while (true)
            {
                Socket clientSocket = serverSocket.Accept();
                ConnectContext ctx = new ConnectContext(clientSocket);

                MsgDispatchCenter dispatchCenter = new MsgDispatchCenter(ctx);
                dispatchCenter.OnMsgReceived += DispatchCenter_OnMsgReceived;
                dispatchCenter.DispatchMsg();
            }
        }

        private static bool DispatchCenter_OnMsgReceived(ConnectContext ctx, string msg)
        {
           // log.Info("开始处理1111 " + msg.msg);

            // 写入应答消息
            ctx.writtingQueue.Push("this is response for" + msg);

            return true;
        }
    }
}
