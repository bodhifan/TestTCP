using Common.Utility;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Core
{
    public class MsgSenderEngine
    {

        ILog log = LogManager.GetLogger(typeof(MsgSenderEngine));
        Socket myClientSocket;
        byte[] result = new byte[1024];

        MessageQueue<string> writtingQueue;

        Thread engine;

        public MsgSenderEngine(Socket clientSocket, MessageQueue<string> queue)
        {
            myClientSocket = clientSocket;
            myClientSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            writtingQueue = queue;
        }

        public void Start()
        {
            engine = new Thread(run);
            engine.Start();
        }

        private void run()
        {
            while (true)
            {
                string msg = writtingQueue.Fetch();
                try
                {
                    string msgstr = string.Format("length:{0:00000}{1}", msg.Length, msg);
                    log.Debug("发送消息：" + msgstr);
                    result = Encoding.UTF8.GetBytes(msgstr);
                    
                    myClientSocket.Send(result);
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
