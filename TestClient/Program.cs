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
            serverSocket.Connect(ip, myProt);

            ConnectContext ctx = new ConnectContext(serverSocket);

            for (int i = 0; i < 10; i++)
            {
                ctx.writtingQueue.Push(new Message("hello" + i));
            }
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
