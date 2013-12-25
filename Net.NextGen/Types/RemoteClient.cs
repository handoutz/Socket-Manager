using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Net.NextGen.Types
{
    public class RemoteClient
    {
        private Guid _guid;
        public Guid Guid { get { return _guid == Guid.Empty ? (_guid = Guid.NewGuid()) : _guid; } }
        public Socket RawSocket { get; set; }
        public byte[] ReceiveBuffer { get; set; }

        public RemoteClient(Socket rawSocket)
        {
            RawSocket = rawSocket;
            ReceiveBuffer = new byte[Properties.Settings.Default.buffer_size];
        }
    }
}
