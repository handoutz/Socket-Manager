using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace SocketManager
{
    public class Client
    {
        private Guid _ID;
        private Socket _Sock;
        private List<byte[]> _Queue;

        public Guid ID { get { return _ID; } }
        public Socket RemoteSocket { get { return _Sock; } set { _Sock = value; } }

        public Client(Socket s)
        {
            _ID = Guid.NewGuid();
            _Sock = s;
            _Queue = new List<byte[]>();
        }

        public void AddToQueue(byte[] b)
        {
            _Queue.Add(b);
        }
        public void AddToQueue(string t)
        {
            AddToQueue(t, Encoding.ASCII);
        }
        public void AddToQueue(string t, Encoding e)
        {
            _Queue.Add(e.GetBytes(t));
        }
    }
}
