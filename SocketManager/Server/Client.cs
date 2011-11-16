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

        public Guid ID { get { return _ID; } }
        public Socket RemoteSocket { get { return _Sock; } set { _Sock = value; } }
        public byte[] RecieveBuffer = new byte[R.BUFFER_SIZE];

        public Client(Socket s)
        {
            _ID = Guid.NewGuid();
            _Sock = s;
        }
    }
}
