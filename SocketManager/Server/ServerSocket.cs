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
        private Encoding Encoder;
        #endregion

        #region Events
        public event ServerEvent OnClientConnect;
        public event ServerEvent OnDataRecieved;
        #endregion

        #region Constructors
        public ServerSocket(IPEndPoint e, AddressFamily fam = AddressFamily.InterNetwork,
            SocketType type = SocketType.Stream, ProtocolType ptype = ProtocolType.Tcp)
        {
            SetEncoding(Encoding.ASCII);
            _ListenerSocket = new Socket(fam, type, ptype);
            _ListenerSocket.Bind(e);
            _ListenerSocket.Listen(10);
            _ListenerSocket.BeginAccept(AcceptCB, _ListenerSocket);
        }
        public void SetEncoding(Encoding e)
        {
            Encoder = e;
        }
        #endregion

        #region Public Methods
        public void Send(Guid cli, string data)
        {
            if (_Clients.ContainsKey(cli))
                Send(_Clients[cli], data);
        }
        public void Send(Client cli, string data)
        {
            Send(cli, Encoder.GetBytes(data));
        }
        public void Send(Client cli, IEnumerable<byte> buff)
        {
            var sock = cli.RemoteSocket;
            byte[] buffer = buff.ToArray();
            sock.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, SendCB, cli);
        }
        #endregion

        #region Private Methods
        private void SendCB(IAsyncResult r)
        {
            var cli = (Client)r.AsyncState;
            cli.RemoteSocket.EndSend(r);
        }
        private void AcceptCB(IAsyncResult r)
        {
            var sock = (Socket)r.AsyncState;
            Socket cli = sock.EndAccept(r);
            Client c = new Client(cli);
            _Clients.Add(c.ID, c);
            c.RemoteSocket.BeginReceive(c.RecieveBuffer, 0, R.BUFFER_SIZE, SocketFlags.None, RecieveCB, c);
            OnClientConnect(new Result(null, c));
            _ListenerSocket.BeginAccept(AcceptCB, sock);
        }
        private void RecieveCB(IAsyncResult r)
        {
            var cli = (Client)r.AsyncState;
            cli.RemoteSocket.EndReceive(r);
            OnDataRecieved(new Result(cli.RecieveBuffer, cli));
            cli.RemoteSocket.BeginReceive(cli.RecieveBuffer, 0, R.BUFFER_SIZE, SocketFlags.None, RecieveCB, cli);
        }
        #endregion
    }
}
