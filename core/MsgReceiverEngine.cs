using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TestTCP
{
    class MsgReceiverEngine
    {
        Socket myClientSocket;
        byte[] result = new byte[1024];
        public MsgReceiverEngine(Socket clientSocket)
        {
            myClientSocket = clientSocket;
        }

        public void Start()
        {
            while (true)
            {
                try
                {
                    //通过clientSocket接收数据  
                    int receiveNumber = myClientSocket.Receive(result);
                    Console.WriteLine("接收客户端{0}消息{1}", myClientSocket.RemoteEndPoint.ToString(), Encoding.UTF8.GetString(result, 0, receiveNumber));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                    break;
                }
            }
        }
    }
}
