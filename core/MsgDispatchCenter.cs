using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTCP.core
{
    /**
    * 消息分发中心
    */
    public class MsgDispatchCenter
    {
        // 用于接受消息的消息队列
        private MessageQueue<Message> receiveMsgQueue;
        public MsgDispatchCenter(MessageQueue<Message> receiveMsgQueue)
        {
            this.receiveMsgQueue = receiveMsgQueue;
        }
    }
}
