using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTCP.core;

namespace TestTCP.management
{
    /**
    * 一个模拟器连接实例
    */
    public class TCPConnectorInstance
    {
        string name;
        MsgReceiverEngine msgRcv;
        MsgSenderEngine msgSender;
        ConcurrentQueue<Message> receiveQueue; //保存接受mulator的消息

        public TCPConnectorInstance()
        {

        }
    }

    public class Manager
    {

    }
}
