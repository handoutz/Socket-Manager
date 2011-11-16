using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace SocketManager
{
    public delegate void NetEvent(object sender, NetEventArgs args);
    public class NetEventArgs
    {
        private EndPoint _Who;
        private Guid _ID;
        private byte[] _Buffer;
        private Encoding Encoder;

        public EndPoint ClientIP { get { return _Who; } }
        public Guid ClientUUID { get { return _ID; } }
        public byte[] RawBuffer { get { return _Buffer; } }
        public NetEventArgs() { }
        public object State;

        public string GetPacketAsString(Encoding Encoder)
        {
            return Encoder.GetString(_Buffer);
        }
        public string GetPacketAsString()
        {
            return GetPacketAsString(Encoder);
        }

        internal NetEventArgs(Encoding enc,EndPoint from, Guid fromID, object stat = null,
            byte[] buf = null)
        {
            if (enc == null) enc = Encoding.ASCII;
            _Who = from;
            _ID = fromID;
            State = stat;
            _Buffer = buf;
            Encoder = enc;
        }
    }
    public enum SocketProfile
    {
        Server, Client,
        MultiUserServer
    }
}
