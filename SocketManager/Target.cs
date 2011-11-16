using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using T = System.Timers;

namespace SocketManager
{
    internal class Target
    {
        public Guid ID;
        public Thread AssocThread;
        public Socket AssocSock;
        public EndPoint IPAddr;
        private T.Timer _PollTimer;
        private List<Thread> SendThreads = new List<Thread>();
        public Target(Thread t, EndPoint d, Socket @ref)
        {
            ID = Guid.NewGuid();
            AssocThread = t;
            IPAddr = d;
            AssocSock = @ref;
            _PollTimer = new T.Timer(1000);
            _PollTimer.Elapsed += new T.ElapsedEventHandler(_PollTimer_Elapsed);
            _PollTimer.Start();
        }

        void _PollTimer_Elapsed(object sender, T.ElapsedEventArgs e)
        {
            NetController.Instance.ClientPingFail(ID);
        }
        public void Send(byte[] data)
        {
            var sock = AssocSock;
            Thread t = new Thread(delegate()
            {
                sock.Send(data);
            });
            t.IsBackground = true;
            t.Start();
            SendThreads.Add(t);
        }
        ~Target()
        {
            foreach (Thread t in SendThreads)
                t.Abort();
        }
    }
}
