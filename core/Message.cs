using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Core
{  
    /**
      消息类型
    */
    public enum MessageType
    {
        DEFAULT = 1,
        DEFAULT_ACK = 2,

        PHONENUMBER =3, // 电话号码
        PHONENUMBER_ACK=4,

        SMSCODE = 5,   // 短信验证码
        SMSCODE_ACK = 6,

        IMGCODE = 7,   // 图片验证码
        IMGCODE_ACK=8,

    }
    public class Message
    {
        public MessageType msgType;
        public string msg;

        public Message(string msg):this(MessageType.DEFAULT,msg)
        {

        }

        public Message(MessageType type, string msg)
        {
            this.msgType = type;
            this.msg = msg;
        }
    }

    [DataContract]
    public class MessageJson
    {
        [DataMember(Order = 0, IsRequired = true)]
        public int type { get; set; }

        [DataMember(Order = 1)]
        public string msg { get; set; }
    }
}
