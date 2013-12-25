using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Net.NextGen.Server;

namespace Net.NextGen.Types
{
    public class ServerEvents
    {
        public delegate void ClientConnected(ServerBase server, RemoteClient client);

        public delegate void RawDataReceived(ServerBase server, RemoteClient client, byte[] buffer);

        public delegate void ClientDiconnect(ServerBase server, RemoteClient client);

        public delegate void SendSuccess(ServerBase server, RemoteClient client);
    }
}
