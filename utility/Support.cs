using Common.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utility
{
    /***
     * 公用的工具类
     */
    public class Support
    {
        // 字符串转化为消息
        public static Message String2Message(string msg)
        {

            MessageJson msgJson = parse<MessageJson>(msg);
            Message message = new Message((MessageType)msgJson.type,msg);
            return message;
        }

        // 消息转化为字符串
        public static string Message2String(Message msg)
        {
            MessageJson msgJson = new MessageJson();
            msgJson.type = (int)msg.msgType;
            msgJson.msg = msg.msg;

            return stringify(msgJson);
        }

        public static T parse<T>(string jsonString)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
            }
        }

        public static string stringify(object jsonObject)
        {
            using (var ms = new MemoryStream())
            {
                new DataContractJsonSerializer(jsonObject.GetType()).WriteObject(ms, jsonObject);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
}
