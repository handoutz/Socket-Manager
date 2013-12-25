using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Net.NextGen.Types;

namespace Net.NextGen.Server
{
    public abstract class ServerBase
    {
        #region Fields
        protected Socket _listener;
        protected ConcurrentDictionary<Guid, RemoteClient> _clients = new ConcurrentDictionary<Guid, RemoteClient>();
        protected Encoding _encoder;
        protected IPEndPoint _endPoint;
        protected IDataConverter _converter;
        protected Task _serverTask;
        protected CancellationTokenSource _cancelSource;
        protected ILog _log;
        #endregion

        #region Events
        public event ServerEvents.ClientConnected ClientConnected;
        public event ServerEvents.ClientDiconnect ClientDisconnected;
        public event ServerEvents.RawDataReceived RawDataReceived;
        public event ServerEvents.SendSuccess SendSuccess;
        #region Invocators
        protected virtual void OnClientDisconnected(ServerBase server, RemoteClient client)
        {
            ServerEvents.ClientDiconnect handler = ClientDisconnected;
            if (handler != null) handler(server, client);
        }

        protected virtual void OnRawDataReceived(ServerBase server, RemoteClient client, byte[] buffer)
        {
            ServerEvents.RawDataReceived handler = RawDataReceived;
            if (handler != null) handler(server, client, buffer);
        }

        protected virtual void OnSendSuccess(ServerBase server, RemoteClient client)
        {
            ServerEvents.SendSuccess handler = SendSuccess;
            if (handler != null) handler(server, client);
        }

        protected virtual void OnClientConnected(ServerBase server, RemoteClient client)
        {
            ServerEvents.ClientConnected handler = ClientConnected;
            if (handler != null) handler(server, client);
        }
        #endregion
        #endregion

        #region Constructors

        protected ServerBase()
        {
            _cancelSource = new CancellationTokenSource();
            _log = LogManager.GetLogger(GetType());
        }
        protected ServerBase(IPEndPoint endPoint, AddressFamily addressFamily = AddressFamily.InterNetwork,
            SocketType sockType = SocketType.Stream, ProtocolType protocolType = ProtocolType.Tcp, IDataConverter dataConverter = null)
            : this()
        {
            _encoder = Encoding.ASCII;
            _endPoint = endPoint;
            _listener = new Socket(addressFamily, sockType, protocolType);
            _converter = dataConverter ?? new StringDataConverter();
        }
        #endregion

        #region Server Methods

        public void StartListening()
        {
            _serverTask = Task.Factory.StartNew(Start, _cancelSource.Token);
        }
        #region Abstract
        protected abstract void Start();
        public abstract void Send(RemoteClient client, object data);
        public abstract void Send(Guid clientGuid, object data);
        #endregion

        #endregion
    }
}
