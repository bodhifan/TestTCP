using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTCP.core
{
    /**
    * 消息队列
    */
     public class MessageQueue<T>
    {
        ConcurrentQueue<T> queue; //保存接受mulator的消息
        QueueAddHander<T> addHander; // 当队列中新增元素时调用该回调函数
        public MessageQueue(QueueAddHander<T> handler = null)
        {
            addHander = handler;
            queue = new ConcurrentQueue<T>();
        }

        public void Push(T msg)
        {
            queue.Enqueue(msg);
            if (null != addHander)
                addHander.Invoke(this, msg);
        }

        public T Fetch()
        {
            T t;
            if(!queue.TryDequeue(out t))
            {

            }

            return t;
        }
    }
}
