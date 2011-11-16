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
    internal class Target : IDisposable
    {
        public Guid ID;
        public Socket AssocSock;
        public EndPoint IPAddr;
        public byte[] Buffer = null;
        private T.Timer _PollTimer;
        private List<Thread> SendThreads = new List<Thread>();
        public Target(EndPoint d, Socket @ref)
        {
            ID = Guid.NewGuid();
            IPAddr = d;
            AssocSock = @ref;
            _PollTimer = new T.Timer(1000);
            _PollTimer.Elapsed += new T.ElapsedEventHandler(_PollTimer_Elapsed);
            _PollTimer.Start();
        }
        void _PollTimer_Elapsed(object sender, T.ElapsedEventArgs e)
        {
            if(!AssocSock.Poll(1000, SelectMode.SelectWrite))
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
            this.Dispose();
            foreach (Thread t in SendThreads)
                t.Abort();
        }

        public void Dispose()
        {
            _PollTimer.Close();
            _PollTimer.Dispose();
            AssocSock.Close();
            AssocSock.Dispose();
        }
    }
}
