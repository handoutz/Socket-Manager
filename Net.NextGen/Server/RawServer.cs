using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Net.NextGen.Types;

namespace Net.NextGen.Server
{
    class RawServer : ServerBase
    {
        public RawServer(IPEndPoint endPoint, AddressFamily addressFamily = AddressFamily.InterNetwork,
            SocketType sockType = SocketType.Stream, ProtocolType protocolType = ProtocolType.Tcp,
            IDataConverter dataConverter = null)
            : base(endPoint, addressFamily, sockType, protocolType, dataConverter)
        {

        }

        #region ServerBase impl
        protected async override void Start()
        {
            var token = _cancelSource.Token;
            try
            {
                _listener.Bind(_endPoint);
                _listener.Listen(10);
                while (!token.IsCancellationRequested)
                {
                    var remoteSock = await Task<Socket>.Factory.StartNew(() => _listener.Accept(), token);
                    var cli = new RemoteClient(remoteSock);
                    _clients.AddOrUpdate(cli.Guid, cli, (guid, client) => cli);
                    OnClientConnected(this, cli);
                }
            }
            catch (SocketException se)
            {
                _log.Error("Listen loop error.", se);
            }
            catch (OperationCanceledException ce)
            {

            }
        }

        public override void Send(Types.RemoteClient client, object data)
        {
            throw new NotImplementedException();
        }

        public override void Send(Guid clientGuid, object data)
        {
            throw new NotImplementedException();
        } 
        #endregion

        #region Private Methods
        
        #endregion
    }
}
