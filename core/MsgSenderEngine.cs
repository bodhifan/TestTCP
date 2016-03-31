using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TestTCP
{
    class MsgSenderEngine
    {
        Socket myClientSocket;
        byte[] result = new byte[1024];

        public MsgSenderEngine(Socket clientSocket)
        {
            myClientSocket = clientSocket;
        }

        public void Start()
        {
            while (true)
            {

                try
                {
                    string newTypedLine = Console.ReadLine();
                    myClientSocket.Send(Encoding.UTF8.GetBytes(newTypedLine + "\n"));
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
