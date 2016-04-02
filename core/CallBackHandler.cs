using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Core
{
    /***
    *  定义一系列的委托集合
    */

    // 新增队列元素时的回调
    public delegate void QueueAddHander<T>(MessageQueue<T> queue, T msg);
}
