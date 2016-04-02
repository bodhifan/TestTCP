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
        private MessageQueue<Message> receiveMsgQueue;
        private ConnectContext context;

        Dictionary<string,MsgHandler> handlerMap = new Dictionary<string,MsgHandler>();

        // 消息分发线程
        Thread dispatchThread;

        public MsgDispatchCenter(ConnectContext context)
        {
            this.context = context;
            this.receiveMsgQueue = context.receivedQueue;

            // 注册默认的消息处理器
            RegisterHandler("default", new DefaultHandler());

            // 启动分发线程
            dispatchThread = new Thread(run);
            dispatchThread.Start();
            
        }

        public void RegisterHandler(string name, MsgHandler handler)
        {
            handlerMap.Add(name, handler);
        }


        private void run()
        {
            while (true)
            {
                Message msg = receiveMsgQueue.Fetch();
                string msgType = msg.msgType.ToString();
                
                MsgHandler handler = null;
                log.Debug("查找处理器:" + msgType);
                if (handlerMap.ContainsKey(msgType))
                {
                    handler = handlerMap[msgType];
                }

                if (handler == null)
                {
                    log.Debug("没找到对应的处理器，使用默认的处理器");
                    handler = handlerMap["default"];
                }
                handler.handler(context, msg);
            }
        }
    }
}
