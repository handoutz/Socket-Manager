using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace SocketManager
{
    public delegate void ServerEvent(Result res);
    public class ServerSocket
    {
        #region Fields
        private Socket _ListenerSocket;
        private Dictionary<Guid, Client> _Clients = new Dictionary<Guid, Client>();
        #endregion

        #region Events
        public event ServerEvent OnClientConnect;
        #endregion

        #region Constructors
        public ServerSocket(IPEndPoint e, AddressFamily fam = AddressFamily.InterNetwork,
            SocketType type = SocketType.Stream, ProtocolType ptype = ProtocolType.Tcp)
        {
            _ListenerSocket = new Socket(fam, type, ptype);
            _ListenerSocket.Bind(e);
            _ListenerSocket.BeginAccept(AcceptCB, _ListenerSocket);
        } 
        #endregion

        #region Private Methods
        private void AcceptCB(IAsyncResult r)
        {
            var sock = (Socket)r.AsyncState;
            Socket cli = sock.EndAccept(r);

            Client c = new Client(cli);
            _Clients.Add(c.ID, c);
            OnClientConnect(new Result(null, c));
        }
        #endregion
    }
}
