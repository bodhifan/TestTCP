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

        MessageQueue<Message> writtingQueue;

        Thread engine;

        public MsgSenderEngine(Socket clientSocket, MessageQueue<Message> queue)
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
                Message msg = writtingQueue.Fetch();
                try
                {
                    string msgstr = Support.Message2String(msg);
                    msgstr = string.Format("length:{0:00000}{1}", msgstr.Length,msgstr);
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
