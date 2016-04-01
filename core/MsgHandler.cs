using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTCP.core
{
    /**
    * 消息处理器
    */
    public interface MsgHandler
    {
        bool handler(Message msg);
    }
}
