using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using log4net;
using Common.Utility;

namespace Common.Core
{
    public class MsgReceiverEngine
    {
        ILog log = LogManager.GetLogger(typeof(MsgReceiverEngine));
        Socket myClientSocket;
        byte[] result = new byte[1024];

        MessageQueue<Message> receivedQueue;

        /**
         * 用于拉取消息的线程
         */
        Thread engine;
        public MsgReceiverEngine(Socket clientSocket,MessageQueue<Message> queue)
        {
            myClientSocket = clientSocket;
            receivedQueue = queue;
        }

        public void Start()
        {
            engine = new Thread(run);
            engine.Start();
        }

        private static int HEADER_LENGTH = 12;
        private void run()
        {
            StringBuilder sb = new StringBuilder();

            while (true)
            {
                try
                {
                    //通过clientSocket接收数据  
                    int receiveNumber = myClientSocket.Receive(result);
                    string msg = Encoding.UTF8.GetString(result, 0, receiveNumber);
                    sb.Append(msg);
                    log.Debug("接收到字符串 " + msg);

                    string currentStr = sb.ToString();
                    
                    // 12为：length:00000 的长度
                    while (currentStr.Length > HEADER_LENGTH)
                    {
                        // 获取标示头
                        string onsMsg = currentStr.Substring(0, HEADER_LENGTH);
                        int length = Convert.ToInt32(onsMsg.Substring(8).TrimStart(new char[] { '0'}));
                        currentStr = currentStr.Remove(0, HEADER_LENGTH);
                        if (currentStr.Length < length)
                        {
                            break;
                        }

                        // 获取真正的报文
                        onsMsg = currentStr.Substring(0, length);
                        Message message = Support.String2Message(onsMsg);
                        log.Info(string.Format("接收{0}消息{1}", myClientSocket.RemoteEndPoint.ToString(), message));
                        receivedQueue.Push(message);
                        currentStr = currentStr.Remove(0, length);
                    }

                    sb.Clear();
                    sb.Append(currentStr);

                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                    break;
                }
            }
        }
    }
}
