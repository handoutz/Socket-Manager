using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace SocketManager
{
    /// <summary>
    /// The main class.
    /// </summary>
    public class NetController
    {
        private const int BUFFER_SIZE = 512;
        public static NetController Instance;
        #region vars
        private SocketProfile _Profile;
        private SocketInformation _SockInfo;
        private Socket _Sock;
        private Thread _ListenThread;
        private Dictionary<Guid, Target> _Threads;
        private Encoding Encoder;
        private Socket _ClientProfSocket;

        public event NetEvent RecieveEvent;
        public event NetEvent NewConnectionEvent;
        public event NetEvent DisconnectionEvent;


        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new controller with profile p.
        /// </summary>
        /// <param name="p">What profile this controller should assume</param>
        public NetController(SocketProfile p)
        {
            _Profile = p;
            _Threads = new Dictionary<Guid, Target>();
            NetController.Instance = this;
        }
        /// <summary>
        /// Default constructor, defaults to SocketProfile.Client
        /// </summary>
        public NetController() : this(SocketProfile.Client) { }
        #endregion

        #region Public Methods
        public NetworkStream GetStream()
        {
            if (_Profile == SocketProfile.Client)
                return new NetworkStream(_ClientProfSocket);
            else
                return null;
        }
        public void Connect(IPEndPoint p,
            AddressFamily family = AddressFamily.InterNetwork,
            SocketType type = SocketType.Stream,
            ProtocolType protoType = ProtocolType.Tcp)
        {
            _ClientProfSocket = new Socket(family, type, protoType);
            _ClientProfSocket.Connect(p);
        }
        public void Connect(IPEndPoint p)
        {
            Connect(p);
        }
        public void Connect(IPAddress a, int port)
        {
            Connect(new IPEndPoint(a, port));
        }
        public void Connect(string ip, int port)
        {
            Connect(IPAddress.Parse(ip), port);
        }
        public void SetEncoding(Encoding e)
        {
            Encoder = e;
        }
        public void Start(SocketInformation info)
        {
            SetEncoding(Encoding.ASCII);
            _SockInfo = info;
            _Sock = new Socket(info);
        }
        public void Start(int port,
            IPAddress ToBind,
            AddressFamily family = AddressFamily.InterNetwork,
            SocketType type = SocketType.Stream,
            ProtocolType protoType = ProtocolType.Tcp)
        {
            if (ToBind == null) ToBind = IPAddress.Any;
            _Sock = new Socket(family, type, protoType);
            _Sock.Bind(new IPEndPoint(ToBind, port));
            _ListenThread = new Thread(ListenThread);
            _ListenThread.IsBackground = true;
            _ListenThread.Start();
        }
        public void Send(Guid who, IEnumerable<byte> b)
        {
            if (!_Threads.ContainsKey(who)) return;
            Target targ = _Threads[who];
            targ.Send(b.ToArray());
        }
        public void Send(Guid who, string what) { Send(who, Encoder.GetBytes(what)); }
        #endregion

        #region Private Methods
        internal void ClientPingFail(Guid who)
        {
            DisconnectionEvent(this, new NetEventArgs(null, null, who, null));
        }
        private void ListenThread()
        {
            _Sock.Listen(25);
            while (true)
            {
                Socket cli = _Sock.Accept();
                Target targ = new Target(cli.RemoteEndPoint, cli);
                _Threads.Add(targ.ID, targ);
                NewConnectionEvent(this, new NetEventArgs(Encoding.ASCII, cli.RemoteEndPoint, targ.ID, null, null));
                targ.Buffer = new byte[BUFFER_SIZE];
                cli.BeginReceive(targ.Buffer, 0, BUFFER_SIZE, SocketFlags.None, Recieve, targ);
            }
        }
        private void Recieve(IAsyncResult r)
        {
            var targ = (Target)r.AsyncState;
            var sock = targ.AssocSock;
            sock.EndReceive(r);
            RecieveEvent(this, new NetEventArgs(Encoder, null, targ.ID, null, targ.Buffer));
            sock.BeginReceive(targ.Buffer, 0, BUFFER_SIZE, SocketFlags.None, Recieve, targ);
        }
        #endregion
    }
}
