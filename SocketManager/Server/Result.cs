using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketManager
{
    public class Result
    {
        private byte[] _buff;
        public byte[] RawBuffer { get { return _buff; } }

        private Client cli;
        public Client RemoteClient { get { return cli; } }

        private Encoding Encoder;

        public Result() : this(null, null, Encoding.ASCII) { }
        public Result(byte[] b, Client c, Encoding e)
        {
            _buff = b;
            cli = c;
            Encoder = e;
        }
        public Result(byte[] b, Client c) : this(b, c, Encoding.ASCII) { }
        public string AsString()
        {
            return Encoder.GetString(_buff).Trim();
        }
        public override string ToString()
        {
            return AsString();
        }
    }
}
