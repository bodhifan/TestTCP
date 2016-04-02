using Common.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Management
{
    //用于消息处理的委托
    public delegate bool MsgHandler(ConnectContext ctx, Message msg);

    
}
