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
            NetController cont = new NetController();
            cont.NewConnectionEvent += new NetEvent(cont_NewConnectionEvent);
            cont.RecieveEvent += new NetEvent(cont_RecieveEvent);
            cont.Start(51215, IPAddress.Any);
            while (Console.Read() != 'q') ;
        }

        static void cont_RecieveEvent(object sender, NetEventArgs args)
        {
            Console.WriteLine("RECV: {0}",args.GetPacketAsString());
        }

        static void cont_NewConnectionEvent(object sender, NetEventArgs args)
        {
            Console.WriteLine("NEW!");
        }
    }
}
