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
        private IPEndPoint _ip;
        #endregion

        #region Events
        public event ServerEvent OnClientConnect;
        public event ServerEvent OnDataRecieved;
        public event ServerEvent OnClientDisconnect;
        public event ServerEvent OnSuccessSend;
        #endregion

        #region Constructors
        public ServerSocket(IPEndPoint e, AddressFamily fam = AddressFamily.InterNetwork,
            SocketType type = SocketType.Stream, ProtocolType ptype = ProtocolType.Tcp)
        {
            SetEncoding(Encoding.ASCII);
            _ip = e;
            _ListenerSocket = new Socket(fam, type, ptype);
        }
        public void SetEncoding(Encoding e)
        {
            Encoder = e;
        }
        #endregion

        #region Public Methods
        public void Start()
        {
            _ListenerSocket.Bind(_ip);
            _ListenerSocket.Listen(10);
            _ListenerSocket.BeginAccept(AcceptCB, _ListenerSocket);
        }
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
            OnSuccessSend(new Result(new byte[1], cli));
        }
        private void AcceptCB(IAsyncResult r)
        {
            var sock = (Socket)r.AsyncState;
            Socket cli = sock.EndAccept(r);
            Client c = new Client(cli);
            _Clients.Add(c.ID, c);
            c.RemoteSocket.BeginReceive(c.RecieveBuffer, 0, R.BUFFER_SIZE, SocketFlags.None, RecieveCB, c);
            if(OnClientConnect!=null)
                OnClientConnect(new Result(null, c));
            _ListenerSocket.BeginAccept(AcceptCB, sock);
        }
        private bool IsConnected(Socket socket)
        {
            try
            {
                return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch (SocketException) { return false; }
        }
        private void RecieveCB(IAsyncResult r)
        {
            
            var cli = (Client)r.AsyncState;
            try
            {
                cli.RemoteSocket.EndReceive(r);
                if (IsConnected(cli.RemoteSocket))
                {
                    DisconnectClient(cli);
                    return;
                }
                OnDataRecieved(new Result(cli.RecieveBuffer, cli));
                cli.RecieveBuffer = new byte[R.BUFFER_SIZE];
                cli.RemoteSocket.BeginReceive(cli.RecieveBuffer, 0, R.BUFFER_SIZE, SocketFlags.None, out cli.Errors, RecieveCB, cli);
            }
            catch (Exception)
            {
                Console.WriteLine(string.Format("{0}: RECV FAIL CLIENT DISCONNECT", cli.ID));
                DisconnectClient(cli);
                
            }
        }
        private void DisconnectClient(Client cli)
        {
            OnClientDisconnect(new Result(null, cli));
            cli.RemoteSocket.Close();
            _Clients.Remove(cli.ID);
        }
        #endregion
    }
}
