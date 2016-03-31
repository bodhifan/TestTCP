using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTCP.core
{  
    /**
      消息类型
    */
    public enum MessageType
    {
        PHONENUMBER=1, // 电话号码
        SMSCODE = 2,   // 短信验证码
        IMGCODE = 3,   // 图片验证码

    }
    public class Message
    {
        public MessageType msgType;
        public string msg;
    }
}
