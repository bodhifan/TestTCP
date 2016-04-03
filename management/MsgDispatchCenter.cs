using Common.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Management
{
    /**
    * 消息分发中心
    */
    public class MsgDispatchCenter
    {

        ILog log = LogManager.GetLogger(typeof(MsgDispatchCenter));
        // 用于接受消息的消息队列
        private MessageQueue<string> receiveMsgQueue;
        private ConnectContext context;

        public event MsgHandler OnMsgReceived;

        // 消息分发线程
        Thread dispatchThread;
        public MsgDispatchCenter(ConnectContext context)
        {
            this.context = context;
            this.receiveMsgQueue = context.receivedQueue;
        }


        // 启动分发线程
        public void DispatchMsg()
        {
            dispatchThread = new Thread(run);
            dispatchThread.Start();
        }

        private void run()
        {
            while (true)
            {
                string msg = receiveMsgQueue.Fetch();
               
                if(null != OnMsgReceived)
                {
                    log.Debug("触发消息处理事件" );
                    OnMsgReceived(context, msg);
                }
                //MsgHandler handler = null;
                //log.Debug("查找处理器:" + msgType);
                //if (handlerMap.ContainsKey(msgType))
                //{
                //    handler = handlerMap[msgType];
                //}

                //if (handler == null)
                //{
                //    log.Debug("没找到对应的处理器，使用默认的处理器");
                //    handler = handlerMap["default"];
                //}
                //handler.handler(context, msg);
            }
        }
    }
}
