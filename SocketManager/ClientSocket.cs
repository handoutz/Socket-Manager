using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using T = System.Timers;

namespace SocketManager
{
    public delegate void SocketEvent(object State, byte[] RawBuffer);
    public class ClientSocket
    {
        #region Field Vars
        private IPEndPoint _EndPoint;
        private Socket _Socket;
        private byte[] _RecieveBuffer = new byte[R.BUFFER_SIZE];
        private byte[] _SendBuffer = new byte[R.BUFFER_SIZE];
        private Encoding Encoder;
        private T.Timer _TimedPoll;
        #endregion

        #region Constructors
        public ClientSocket(IPEndPoint e, AddressFamily fam = AddressFamily.InterNetwork,
            SocketType type = SocketType.Stream, ProtocolType ptype = ProtocolType.Tcp)
        {
            SetEncoding(Encoding.ASCII);
            _EndPoint = e;
            _Socket = new Socket(fam, type, ptype);
            _Socket.BeginConnect(e, ConnectAsync, _Socket);

            _TimedPoll = new T.Timer(2000);
            _TimedPoll.Elapsed += new T.ElapsedEventHandler(_TimedPoll_Elapsed);
            _TimedPoll.Start();
        }
        public ClientSocket(IPAddress a, int port) : this(new IPEndPoint(a, port)) { }
        public ClientSocket(string a, int port) : this(new IPEndPoint(IPAddress.Parse(a), port))  { }
        #endregion

        #region Pub Methods
        public void SetEncoding(Encoding e)
        {
            Encoder = e;
        }
        public void Disconnect()
        {
            _Socket.BeginDisconnect(false, DisconnectAsync, _Socket);
        }
        #endregion

        #region Events
        public event SocketEvent OnConnected;
        public event SocketEvent OnRecieve;
        public event SocketEvent OnSuccessSend;
        public event SocketEvent OnError;
        public event SocketEvent OnDisconnect; 
        #endregion

        #region AsyncAndPrivateMethods
        private void _TimedPoll_Elapsed(object sender, T.ElapsedEventArgs e)
        {
            if (!_Socket.Poll(1000, SelectMode.SelectWrite))
                _Socket.Close();
        }
        private void DisconnectAsync(IAsyncResult r)
        {
            ((Socket)r.AsyncState).EndDisconnect(r);
            OnDisconnect("requested", null);
            _Socket.Close();
            _Socket.Dispose();
        }

        private void ConnectAsync(IAsyncResult r)
        {
            var sock = (Socket)r.AsyncState;
            sock.EndConnect(r);
            OnConnected("connected", null);
            sock.BeginReceive(_RecieveBuffer, 0, R.BUFFER_SIZE, SocketFlags.None, RecieveAsync, sock);
        }
        private void RecieveAsync(IAsyncResult r)
        {
            var sock = (Socket)r.AsyncState;
            sock.EndReceive(r);
            OnRecieve("recieve", _RecieveBuffer);
            sock.BeginReceive(_RecieveBuffer, 0, R.BUFFER_SIZE, SocketFlags.None, RecieveAsync, sock);
        }
        private void SendAsync(IAsyncResult r)
        {
            var sock = (Socket)r.AsyncState;
            sock.EndSend(r);
            OnSuccessSend(true, null);
        } 
        #endregion
    }
}
