using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Core;
using log4net;
using System.Net;
using System.Net.Sockets;

namespace Common.Management
{
    /**
    * 一个模拟器连接实例
    */
    public class ConnectContext
    {
        /**
         * 日志 
         */
        ILog log = LogManager.GetLogger(typeof(ConnectContext));


        // 该连接器名称
        string name;


        /**
         * 获取消息引擎，获取消息后会写入到receivedQueue
         */
        public MsgReceiverEngine msgRcv;

        /**
         * 写入消息引擎,从writtingQueue中读入消息
         */
        public MsgSenderEngine msgSender;

        public MessageQueue<Message> receivedQueue; //保存接受mulator的消息
        public MessageQueue<Message> writtingQueue; //等待发送的消息

        Socket socket;

        string ipAddr; // IP地址
        int port;   // 端口号

        bool isCnnSuc; // 是否连接成功

        // 为服务器提供服务
        public ConnectContext(Socket socket):this("default",socket) 
        {

        }
        public ConnectContext(string name,Socket socket)
        {
            this.name = name;
            this.socket = socket;
            ipAddr = null;
            initConnect();
        }

        // 为客户端提供服务
        public ConnectContext(string ip,int port):this("default", ip, port)
        {

        }

        public ConnectContext(string name,string ip, int port)
        {
            this.name = name;
            ipAddr = ip;
            this.port = port;
            socket = null;
            initConnect();
        }

        private void initConnect()
        {

            isCnnSuc = true;

            // 如果是client模式则连接服务器
            if (socket == null && ipAddr != null)
            {
                try
                {
                    log.Info("开始连接服务器...");
                    IPAddress ip = IPAddress.Parse(ipAddr);
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.Connect(new IPEndPoint(ip, port)); //配置服务器IP与端口  
                    log.Info("连接服务器成功");
                }
                catch (Exception ex)
                {
                    log.Error("连接服务器失败" + ex.Message);
                    isCnnSuc = false;
                }
            }

            log.Info("启动接收引擎");
            receivedQueue = new MessageQueue<Message>();
            msgRcv = new MsgReceiverEngine(socket, receivedQueue);
            msgRcv.Start();

            log.Info("启动发送引擎");
            writtingQueue = new MessageQueue<Message>();
            msgSender = new MsgSenderEngine(socket, writtingQueue);
            msgSender.Start();

        }
    }
}
