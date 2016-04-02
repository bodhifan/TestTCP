using Common.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Common.Management
{
    /**
    * 消息处理器
    */
    public interface MsgHandler
    {
        bool handler(ConnectContext ctx,Message msg);
    }

    /************************************************************************/
    /* 默认的消息处理器                                                     */
    /************************************************************************/
    public class DefaultHandler : MsgHandler
    {
        ILog log = LogManager.GetLogger(typeof(DefaultHandler));
        public bool handler(ConnectContext ctx, Message msg)
        {
            log.Info("开始处理 "+msg.msg);

            // 写入应答消息
            ctx.writtingQueue.Push(new Message(msg.msgType + 1, "this is repsonse for:" + msg));

            return true;
        }
    }
}
