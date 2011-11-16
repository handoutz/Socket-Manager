using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketManager;
using System.Net;

namespace SocketTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerSocket sock = new ServerSocket(new IPEndPoint(IPAddress.Any, 51215));
            sock.OnClientConnect += new ServerEvent(sock_OnClientConnect);
            sock.OnDataRecieved += new ServerEvent(sock_OnDataRecieved);
            sock.Start();
            while (Console.ReadLine() != "q") ;
        }

        static void sock_OnDataRecieved(Result res)
        {
            Console.WriteLine("RECV: FROM: {0} DATA: {1}", res.RemoteClient.ID, res.AsString());
        }

        static void sock_OnClientConnect(Result res)
        {
            Console.WriteLine("NEW CLIENT: {0}" + res.RemoteClient.ID);
        }
    }
}
